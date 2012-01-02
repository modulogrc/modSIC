using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;
using Modulo.Collect.Service.Client;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using Modulo.Collect.Monitor;


namespace WindowsFormsApplication2
{
    
    public partial class optionsForm : Form
    {        
        private int timeoutServiceMilliseconds = 5000;
        private int threadUpdateDelayMilSec    = 3000;
        private int threadIconChangeMiliSec    = 500;

        private int qtdCollectsBeingPerfomed = 0;
        private int firstTimeCollects        = 0;

        private bool appExit     = false;
        private bool startEngine = false;       

        private string url;
        private string port;
        private string username;
        private string password;

        private const int _StartMenu_  =   0;
        private const int _StopMenu_ = 1;
        private const int _OptionMenu_ = 2;
        private const int _CloseMenu_ = 3;

        // Messages
        string STARTED               = "Running";
        string serviceName           = "modSIC";
        string collectsBeingPerfomed = " collect(s) being perfomed";
        string startingService       = "Starting service...";

        // Icons
        Icon runningIcon = new Icon("favicon_32x32.ico");
        Icon stoppedIcon = new Icon("favicon_32x32gs.ico");

        // States Icons
        Icon state1 = new Icon("trayicon_1.ico");
        Icon state2 = new Icon("trayicon_2.ico");
        Icon state3 = new Icon("trayicon_3.ico");
        Icon state4 = new Icon("trayicon_4.ico");
        Icon state5 = new Icon("trayicon_5.ico");

        // modSIc service
        private ServiceController service;        

        public optionsForm()
        {
            InitializeComponent();
                        
            tryContactService();

            //Lets read modSIC config
            try
            {
                var serializedConfig = System.IO.File.ReadAllText("c:\\temp\\modsicsrv_config.txt");
                var config = Newtonsoft.Json.JsonConvert.DeserializeObject<ModsicConfiguration>(serializedConfig);

                this.url        = config.Url ;
                this.port       = config.Port ;
                this.username   = config.Username ;
                this.password   = config.Password;
            }
            catch (Exception)
            {
                this.url        = "http://localhost";
                this.port       = "1024";
                this.username   = "admin";
                this.password   = "Pa$$w@rd";
            }
            
            Thread updateStatus = new Thread(this.loopUpdateStatus);
            updateStatus.Start();
            
            Thread updateIcon = new Thread(this.loopUpdateIcon);
            updateIcon.Start();            
        }

        private void tryContactService()
        {
            this.service = new ServiceController(serviceName);

            try
            {
                var _tryService = this.service.ServiceHandle;

            } catch(Exception) {
                
                    MessageBox.Show("Service not found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);                
                    System.Environment.Exit(-1);                
            }
        }
        
        private void updateStatus() 
        {            
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lock (this)
                    {
                        if (this.service.Status.ToString() == this.STARTED) //Service started
                        {
                            //We need more time to call GetCollectionInExecution() when service is restarted
                            if (this.startEngine)
                            {
                                notifyIcon1.Text = startingService;
                                this.Refresh();
                                System.Threading.Thread.Sleep(10000);                                
                            }

                            int nCollects = GetCollectionsInExecution();                            

                            notifyIcon1.Icon = runningIcon;                            

                            notifyIcon1.BalloonTipText = nCollects.ToString();

                            startServerMenu.Enabled = false;
                            stopServerMenu.Enabled  = true;
                            startEngine             = false;

                            if (firstTimeCollects != qtdCollectsBeingPerfomed)
                            {
                                firstTimeCollects = qtdCollectsBeingPerfomed;
                                notifyIcon1.ShowBalloonTip(500, "Information", qtdCollectsBeingPerfomed.ToString() + collectsBeingPerfomed, ToolTipIcon.Info);
                            }

                            notifyIcon1.Text = service.Status.ToString() + "...";
                            notifyIcon1.Text += "\r\n " + qtdCollectsBeingPerfomed.ToString() + collectsBeingPerfomed;
                        }
                        else  //Service stopped
                        {
                            startServerMenu.Enabled = true;
                            stopServerMenu.Enabled  = false;

                            notifyIcon1.Icon        = stoppedIcon;
                            notifyIcon1.Text        = service.Status.ToString() + "...";
                        }
                    }
                });
            }
        }   
        
        private int GetCollectionsInExecution()
        {            
            var guid = (GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0];
            var clientId = System.Environment.MachineName + "\\" + guid.Value;

            string url = this.url + ":" + this.port + "/CollectService";
            
            try { 

                ModSicConnection _modSicConnection = new ModSicConnection(url, this.username, this.password, null);            
            
                qtdCollectsBeingPerfomed = _modSicConnection.GetCollectionsInExecution().Count();

                return qtdCollectsBeingPerfomed;
            }
            catch (Exception)
            {
                return -1;
            }            
        }

        private void loopUpdateStatus()
        {
            while (true)
            {               
                this.service.Refresh();                
                                
                updateStatus();

                if (appExit)
                    break;

                //To avoid 100% cpu consuming 
                System.Threading.Thread.Sleep(threadUpdateDelayMilSec);                
            }
        }

        private void loopUpdateIcon()
        {
            while (true)
            {                
                while (qtdCollectsBeingPerfomed > 0) { 
                   
                        //Too many colors, let´s use just two to improve contrast

                        //notifyIcon1.Icon = state1;
                        //System.Threading.Thread.Sleep(500);
                       // notifyIcon1.Icon = state2;
                       // System.Threading.Thread.Sleep(500);
                        notifyIcon1.Icon = state3;
                        System.Threading.Thread.Sleep(threadIconChangeMiliSec);
                        notifyIcon1.Icon = state4;
                        System.Threading.Thread.Sleep(threadIconChangeMiliSec);
                }

                while (this.startEngine)
                {                    
                    notifyIcon1.Icon = state4;
                    System.Threading.Thread.Sleep(threadIconChangeMiliSec);
                    notifyIcon1.Icon = stoppedIcon;
                    System.Threading.Thread.Sleep(threadIconChangeMiliSec);
                }

                if (appExit)
                    break;

                //To avoid 100% cpu consuming
                System.Threading.Thread.Sleep(threadUpdateDelayMilSec);
            }
        }        

        private void startServerMenu_Click(object sender, EventArgs e)
        {
            ServiceController service = new ServiceController(serviceName);

            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(this.timeoutServiceMilliseconds);

                contextMenuStrip1.Hide();
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                service.Refresh();
                this.startEngine = true;
            }
            catch
            {
                // ...
            }
        }

        private void stopServerMenu_Click(object sender, EventArgs e)
        {
            ServiceController service = new ServiceController(serviceName);

            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(this.timeoutServiceMilliseconds);

                contextMenuStrip1.Hide();
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                service.Refresh();
            }
            catch
            {
                // ...
            }
        }

        private void optionsMenu_Click(object sender, EventArgs e)
        {
            var dialog = new OptionsDialog();

            int deskHeight  = Screen.PrimaryScreen.WorkingArea.Height;
            int deskWidth   = Screen.PrimaryScreen.WorkingArea.Width;

            deskWidth   -= dialog.Size.Width + 10;
            deskHeight  -= dialog.Size.Height + 10;          

            dialog.Location = new Point(deskWidth, deskHeight);                

            dialog.ShowDialog(this);
        }

        private void closeMenu_Click(object sender, EventArgs e)
        {
            appExit = true;

            notifyIcon1.Dispose();
            Application.Exit();
        }

        private void startServerMenu_MouseMove(object sender, MouseEventArgs e)
        {
            updateMenuColors(_StartMenu_);            
        }

        private void stopServerMenu_MouseMove(object sender, MouseEventArgs e)
        {
            updateMenuColors(_StopMenu_);            
        }

        private void optionsMenu_MouseMove(object sender, MouseEventArgs e)
        {
            updateMenuColors(_OptionMenu_);            
        }

        private void closeMenu_MouseMove(object sender, MouseEventArgs e)
        {
            updateMenuColors(_CloseMenu_);            
        }
        
        private void updateMenuColors(int menuSelected)
        {
            //We need it to force mouse pointer be a arrow
            Cursor.Current = Cursors.Default;

            switch (menuSelected)
            {
                case _StartMenu_ :

                    startServerMenu.BackColor = SystemColors.GradientActiveCaption;

                    stopServerMenu.BackColor = SystemColors.ControlLightLight;
                    closeMenu.BackColor = SystemColors.ControlLightLight;
                    optionsMenu.BackColor = SystemColors.ControlLightLight;

                    break;

                case _StopMenu_:
                    
                    stopServerMenu.BackColor = SystemColors.GradientActiveCaption;

                    startServerMenu.BackColor   = SystemColors.ControlLightLight;
                    closeMenu.BackColor         = SystemColors.ControlLightLight;
                    optionsMenu.BackColor       = SystemColors.ControlLightLight;
                    
                    break;

                case _OptionMenu_:

                    optionsMenu.BackColor = SystemColors.GradientActiveCaption;

                    startServerMenu.BackColor   = SystemColors.ControlLightLight;
                    stopServerMenu.BackColor    = SystemColors.ControlLightLight;
                    closeMenu.BackColor         = SystemColors.ControlLightLight;

                    break;

                case _CloseMenu_:

                    closeMenu.BackColor = SystemColors.GradientActiveCaption;

                    startServerMenu.BackColor   = SystemColors.ControlLightLight;
                    stopServerMenu.BackColor    = SystemColors.ControlLightLight;
                    optionsMenu.BackColor       = SystemColors.ControlLightLight;

                    break;    
            }            
        }
    }
}

                
               