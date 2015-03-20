#region License
/* * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2015, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 *   
 * - Redistributions in binary form must reproduce the above copyright 
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 *   
 * - Neither the name of Modulo Security, LLC nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific  prior written permission.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 * */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

namespace Modulo.Collect.GraphicalConsole
{
    public class ConfigurationController
    {
        #region Private members
        private const string sectionName = "ConfigurationSection";
        private IConfigurationView view;
        private string programFolder;
        private string configFile;
        private string defaultConfigFile;
        #endregion

        #region Public Members
        public bool OnReadConfigurationCalled { get; set; }
        public bool OnWriteConfigurationCalled { get; set; }
        public ConfigurationSection Section { get; private set; }
        public Exception Exception { get; set; }
        #endregion

        #region Constructor
        public ConfigurationController(IConfigurationView _view)
        {
            string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string productName = Application.ProductName;
            string myDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) ?? @"C:\";
            string ApplicationDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) ?? myDocumentsFolder;
            programFolder = Path.Combine(ApplicationDataFolder, productName);
            defaultConfigFile = String.Format("{0}.exe.config", assemblyName);
            configFile = Path.Combine(programFolder, defaultConfigFile);

            view = _view;

            view.OnReadConfiguration += new EventHandler(view_OnReadConfiguration);
            view.OnWriteConfiguration += new EventHandler(view_OnWriteConfiguration);
        }
        #endregion

        #region View Events
        public void view_OnWriteConfiguration(object sender, EventArgs e)
        {
            OnWriteConfigurationCalled = true;

            try
            {
                CreateProgramFolder();

                if (view.Server == null || view.Target == null)
                {
                    view.ShowErrorMessage(Resource.InvalidConfiguration);
                    return;
                }

                ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = configFile;
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

                Section = (ConfigurationSection)config.GetSection(sectionName);
                if (Section == null)
                {
                    Section = new ConfigurationSection();
                    config.Sections.Add(sectionName, Section);
                }

                Section.server.Address = view.Server.Address ?? String.Empty;
                Section.server.Username = view.Server.Username ?? String.Empty;
                Section.server.Password = view.Server.Password ?? String.Empty;
                Section.server.Port = view.Server.Port ?? String.Empty;

                Section.target.Address = view.Target.Address ?? String.Empty;
                Section.target.Username = view.Target.Username ?? String.Empty;
                Section.target.Password = view.Target.Password ?? String.Empty;
                Section.target.AdministrativePassword = view.Target.AdministrativePassword ?? String.Empty;
                Section.target.SSHPort = view.Target.SSHPort ?? String.Empty;

                Section.file.SaveFolder = view.DestinationFolder ?? String.Empty;
                Section.file.DefinitionFilename = view.DefinitionFilename ?? String.Empty;

                SaveConfiguration(config);
            }
            catch (Exception ex)
            {
                view.ShowErrorMessage(Util.FormatExceptionMessage(ex));
            }
        }

        public void view_OnReadConfiguration(object sender, EventArgs e)
        {
            OnReadConfigurationCalled = true;
            
            view.Server = new ServerConfiguration();
            view.Target = new TargetConfiguration();

            try
            {
                string filename = FileExists(configFile) ? configFile : (FileExists(defaultConfigFile) ? defaultConfigFile : null);
                if (filename != null)
                {
                    ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                    configFileMap.ExeConfigFilename = filename;
                    Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

                    ConfigurationSection section = ReadConfigurationSection(config);

                    if (section != null)
                    {
                        var server = section.server;
                        view.Server.Address = server.Address;
                        view.Server.Username = server.Username;
                        view.Server.Password = server.Password;
                        view.Server.Port = server.Port;

                        var target = section.target;
                        view.Target.Address = target.Address;
                        view.Target.Username = target.Username;
                        view.Target.Password = target.Password;
                        view.Target.AdministrativePassword = target.AdministrativePassword;
                        view.Target.SSHPort = target.SSHPort;

                        var file = section.file;
                        view.DestinationFolder = file.SaveFolder;
                        view.DefinitionFilename = file.DefinitionFilename;
                    }
                }
            }
            catch (Exception ex)
            {
                view.ShowErrorMessage(Util.FormatExceptionMessage(ex));
            }
        }
        #endregion

        #region Useful Methods for Tests
        public virtual bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        public virtual void SaveConfiguration(Configuration config)
        {
            config.Save(ConfigurationSaveMode.Full);
            ConfigurationManager.RefreshSection(sectionName);
        }

        public virtual void CreateProgramFolder()
        {
            if (!Directory.Exists(programFolder))
            {
                Directory.CreateDirectory(programFolder);
            }
        }

        public virtual ConfigurationSection ReadConfigurationSection(Configuration config)
        {
            ConfigurationSection section = (ConfigurationSection)config.GetSection(sectionName);
            return section;
        }
        #endregion
    }
}
