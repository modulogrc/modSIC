using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using System.IO;
using System.Diagnostics;
using System.ServiceProcess;
using System.Net.Mail;
using System.Net;
using Modulo.Collect.OVAL.Results;

namespace modSICTest
{
    class Controller
    {
        private Options options;
        private Model model;
        private StringBuilder emailBody;
        private List<String> emailAttachments;

        const string server = "modsicsrv.exe";
        const string client = "modsic.exe";

        public Controller()
        {
            model = new Model();

            emailBody = new StringBuilder();
            emailAttachments = new List<String>();
        }

        public bool PrepareOptions(string[] args)
        {
            options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(System.Console.Error));
            if (!parser.ParseArguments(args, options))
            {
                return false;
            }

            return true;
        }

        public void Start(string[] args)
        {
            try
            {
                if (!PrepareOptions(args))
                    return;

                CopyFiles(@"\\mss-rj-328\Collector\ModSic");

                InstallServer();
                StartService("modSIC", 30000);

                var definitions = model.GetDefinitionsToCollect(options.testMode);
                foreach (var item in definitions)
                {
                    Console.WriteLine("Coletando \"{0}\"", item.Name);
                    AddDoubleLine();
                    emailBody.AppendLine(item.Filename);
                    AddDoubleLine();

                    try
                    {
                        var time = DateTime.Now;
                        if (!ExecuteCommand(String.Format(".\\modSIC\\{0}", client), item.Parameters))
                        {
                            continue;
                        }

                        var ticks = DateTime.Now.Subtract(time).Ticks;
                        var diff = new DateTime(ticks);
                        emailBody.AppendFormat("Início: {0}\n", time.ToLongTimeString());
                        emailBody.AppendFormat("Duração: {0:00}:{1:00}:{2:00}\n\n", diff.Hour, diff.Minute, diff.Second);

                        int nTrue = 0, nFalse = 0, nError = 0, nUnknown = 0;
                        Console.WriteLine("Analisando o results.");
                        Analyze(out nTrue, out nFalse, out nError, out nUnknown);

                        var filename = String.Format("{0}-results.xml", item.Filename.Replace(".xml", ""));
                        if (File.Exists(filename))
                        {
                            File.Delete(filename);
                        }
                        File.Move("results.xml", filename);
                        Zip(filename);

                        var lastExecution = model.GetLastExecution(item.Id);
                        model.InsertExecution(item.Id, time, ticks, nTrue, nFalse, nError, nUnknown);

                        if (lastExecution != null)
                        {
                            emailBody.AppendLine("Última execução:");
                            AddSingleLine();
                            diff = new DateTime((long)lastExecution.Duration);
                            emailBody.AppendFormat("Duração: {0:00}:{1:00}:{2:00}\n", diff.Hour, diff.Minute, diff.Second);
                            emailBody.AppendFormat("True: {0}\n", lastExecution.NTrue);
                            emailBody.AppendFormat("False: {0}\n", lastExecution.NFalse);
                            emailBody.AppendFormat("Error: {0}\n", lastExecution.NError);
                            emailBody.AppendFormat("Unknown: {0}\n\n", lastExecution.NUnknown);
                        }
                    }
                    catch (Exception ex)
                    {
                        CreateErrorMessage(ex);
                    }
                }

                StopService("modSIC", 30000);
            }
            catch (Exception ex)
            {
                CreateErrorMessage(ex);
            }
            finally
            {
                UninstallServer();
            }

            Console.WriteLine("Enviando o email");
            var subject = string.Format("modSIC - Teste automatizado");
            SendMail(subject, emailBody.ToString());
        }

        private void Zip(string filename)
        {
            var zipFilename = Path.ChangeExtension(filename, "zip");
            if (File.Exists(zipFilename))
            {
                File.Delete(zipFilename);
            }

            if (ExecuteCommand("C:\\Program Files\\7-Zip\\7z.exe", String.Format(" a {0} {1}", zipFilename, filename)))
            {
                emailAttachments.Add(zipFilename);
            }
        }

        private void AddSingleLine()
        {
            emailBody.AppendLine(new String('-', 25));
        }

        private void AddDoubleLine()
        {
            emailBody.AppendLine(new String('=', 40));
        }

        private void CreateErrorMessage(Exception ex)
        {
            var errors = new StringBuilder();

            var e = ex;
            while (e != null)
            {
                errors.AppendLine(e.Message);
                e = e.InnerException;
            }

            var s = errors.ToString();
            emailBody.AppendFormat("Erro: {0}\n", s);
            Console.WriteLine(s);
        }

        private void Analyze(out int nTrue, out int nFalse, out int nError, out int nUnknown)
        {
            nTrue = 0;
            nFalse = 0;
            nError = 0;
            nUnknown = 0;

            if (!File.Exists("results.xml"))
            {
                emailBody.AppendLine("Não foi criado o arquivo results.xml.");
                return;
            }

            var file = new FileStream("results.xml", FileMode.Open);
            IEnumerable<string> errors;
            oval_results ovalResults;

            try
            {
                ovalResults = oval_results.GetOvalResultsFromStream(file, out errors);
                foreach (var definition in ovalResults.results.FirstOrDefault().definitions)
                {
                    switch (definition.result)
                    {
                        case ResultEnumeration.@true:
                            nTrue++;
                            break;

                        case ResultEnumeration.@false:
                            nFalse++;
                            break;

                        case ResultEnumeration.error:
                            nError++;
                            break;

                        case ResultEnumeration.unknown:
                            nUnknown++;
                            break;
                    }
                }

                emailBody.AppendFormat("True: {0}\n", nTrue);
                emailBody.AppendFormat("False: {0}\n", nFalse);
                emailBody.AppendFormat("Error: {0}\n", nError);
                emailBody.AppendFormat("Unknown: {0}\n\n", nUnknown);

                file.Close();
            }
            catch (Exception ex)
            {
                emailBody.AppendFormat("Erro: {0}\n", ex.Message);
            }
        }

        public void StartService(string serviceName, int timeoutMilliseconds)
        {
            Console.WriteLine("Iniciando o {0}.", serviceName);
            ServiceController service = new ServiceController(serviceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }

        public void StopService(string serviceName, int timeoutMilliseconds)
        {
            Console.WriteLine("Parando o {0}.", serviceName);
            ServiceController service = new ServiceController(serviceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
        }

        private void InstallServer()
        {
            Console.WriteLine("Instalando o serviço.");
            ExecuteCommand(String.Format(".\\modSIC\\{0}", server), "install");
        }

        private void UninstallServer()
        {
            Console.WriteLine("Desinstalando o serviço.");
            ExecuteCommand(String.Format(".\\modSIC\\{0}", server), "uninstall");
        }

        private bool ExecuteCommand(string filename, string arguments)
        {
            Console.WriteLine("Executando {0} {1}", filename, arguments);

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = filename;
            p.StartInfo.Arguments = arguments;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            var output = p.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
            p.WaitForExit();
            if (p.ExitCode < 0)
            {
                emailBody.AppendLine(output.Replace((char)8, ' ').Replace(@"| / - \ ", ""));
                return false;
            }

            return true;
        }

        public bool CopyFiles(string Path)
        {
            Console.WriteLine("Copiando ultimo build.");

            var directories = Directory.EnumerateDirectories(Path);
            var list = directories.Select(n => new { Path = n, Date = GetDate(n) });
            var dir = list.OrderByDescending(n => n.Date).First().Path;

            string arguments = String.Format("/r /y \"{0}\\Modulo.Collect.Service.VS2010\" .\\modSIC\\", dir);
            return ExecuteCommand("xcopy.exe", arguments);
        }

        private DateTime GetDate(string n)
        {
            return Directory.GetCreationTime(n);
        }

        private void SendMail(string subject, string body)
        {
            var smtp = new SmtpClient
            {
                Host = "smtp2.modulo.com",
                Port = 25,
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                //Credentials = new NetworkCredential("modsic-test", "M0dul0-0v4l")
            };

            var fromAddress = new MailAddress("cpaiva@modulo.com", "cpaiva");
            var toAddress = new MailAddress("cpaiva@modulo.com.br");
            using (var message = new MailMessage(fromAddress, toAddress) { Subject = subject, Body = body })
            {
                if (!options.testMode)
                {
                    message.To.Add(new MailAddress("jcastro@modulo.com.br"));
                    message.To.Add(new MailAddress("lfernandes@modulo.com.br"));
                    message.To.Add(new MailAddress("iprata@modulo.com.br"));
                    message.To.Add(new MailAddress("dgomes@modulo.com.br"));
                    message.To.Add(new MailAddress("ebomfim@modulo.com.br"));
                }
                foreach (var item in emailAttachments)
                {
                    message.Attachments.Add(new Attachment(item));
                }

                smtp.Send(message);
            }
        }
    }
}
