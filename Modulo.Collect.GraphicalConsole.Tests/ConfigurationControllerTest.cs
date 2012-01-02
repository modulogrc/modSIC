using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Modulo.Collect.GraphicalConsole.Tests
{
    [TestClass]
    public class ConfigurationControllerTest
    {
        public class ConfigurationView : IConfigurationView
        {
            public String LastErrorMessage { get; set; }

            #region IConfigurationView Members
            public ServerConfiguration Server { get; set; }
            public TargetConfiguration Target { get; set; }
            public string DestinationFolder { get; set; }
            public string DefinitionFilename { get; set; }

            public void ShowErrorMessage(string text)
            {
                LastErrorMessage = text;
            }

            public event EventHandler OnReadConfiguration;
            public event EventHandler OnWriteConfiguration;
            #endregion
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_get_a_valid_configuration_from_a_section_object()
        {
            MockRepository mocks = new MockRepository();

            var fakeView = MockRepository.GenerateStub<IConfigurationView>();
            var fakeController = mocks.DynamicMock<ConfigurationController>(new object[] { fakeView });

            var fakeConfigurationSection = new ConfigurationSection();
            fakeConfigurationSection.server.Address = "http://localhost";
            fakeConfigurationSection.server.Port = "1024";
            fakeConfigurationSection.server.Username = "admin";
            fakeConfigurationSection.server.Password = "PassworD";

            fakeConfigurationSection.target.Address = "10.0.0.1";
            fakeConfigurationSection.target.Username = "administrator";
            fakeConfigurationSection.target.Password = "P@asswOrd";
            fakeConfigurationSection.target.AdministrativePassword = "qwerty";

            fakeConfigurationSection.file.SaveFolder = "C:\\Temp\\";
            fakeConfigurationSection.file.DefinitionFilename = "C:\\Temp\\definitions.xml";

            using (mocks.Record())
            {
                Expect.Call(fakeController.FileExists(null)).IgnoreArguments().Return(true);
                Expect.Call(fakeController.ReadConfigurationSection(null)).IgnoreArguments().Return(fakeConfigurationSection);
            }

            mocks.ReplayAll();

            fakeController.view_OnReadConfiguration(this, EventArgs.Empty);

            mocks.VerifyAll();

            Assert.IsNotNull(fakeView.Server);
            Assert.AreEqual("http://localhost", fakeView.Server.Address);
            Assert.AreEqual("1024", fakeView.Server.Port);
            Assert.AreEqual("admin", fakeView.Server.Username);
            Assert.AreEqual("PassworD", fakeView.Server.Password);

            Assert.IsNotNull(fakeView.Target);
            Assert.AreEqual("10.0.0.1", fakeView.Target.Address);
            Assert.AreEqual("administrator", fakeView.Target.Username);
            Assert.AreEqual("P@asswOrd", fakeView.Target.Password);
            Assert.AreEqual("qwerty", fakeView.Target.AdministrativePassword);

            Assert.IsNotNull(fakeView.DestinationFolder);
            Assert.AreEqual("C:\\Temp\\", fakeView.DestinationFolder);
            Assert.IsNotNull(fakeView.DefinitionFilename);
            Assert.AreEqual("C:\\Temp\\definitions.xml", fakeView.DefinitionFilename);
        }

       [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_get_a_valid_server_configuration_from_a_section_object()
        {
            MockRepository mocks = new MockRepository();

            var fakeView = MockRepository.GenerateStub<IConfigurationView>();
            var fakeController = mocks.DynamicMock<ConfigurationController>(new object[] { fakeView });

            var fakeConfigurationSection = new ConfigurationSection();
            fakeConfigurationSection.server.Address = "http://localhost";
            fakeConfigurationSection.server.Port = "1024";
            fakeConfigurationSection.server.Username = "admin";
            fakeConfigurationSection.server.Password = "PassworD";

            using (mocks.Record())
            {
                Expect.Call(fakeController.FileExists(null)).IgnoreArguments().Return(true);
                Expect.Call(fakeController.ReadConfigurationSection(null)).IgnoreArguments().Return(fakeConfigurationSection);
            }

            mocks.ReplayAll();

            fakeController.view_OnReadConfiguration(this, EventArgs.Empty);

            mocks.VerifyAll();

            Assert.IsNotNull(fakeView.Server);
            Assert.AreEqual("http://localhost", fakeView.Server.Address);
            Assert.AreEqual("1024", fakeView.Server.Port);
            Assert.AreEqual("admin", fakeView.Server.Username);
            Assert.AreEqual("PassworD", fakeView.Server.Password);

            Assert.AreEqual(String.Empty, fakeView.Target.Address);
            Assert.AreEqual(String.Empty, fakeView.Target.Username);
            Assert.AreEqual(String.Empty, fakeView.Target.Password);
            Assert.AreEqual(String.Empty, fakeView.Target.AdministrativePassword);

            Assert.AreEqual(String.Empty, fakeView.DestinationFolder);
            Assert.AreEqual(String.Empty, fakeView.DefinitionFilename);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_get_a_valid_target_configuration_from_a_section_object()
        {
            MockRepository mocks = new MockRepository();

            var fakeView = MockRepository.GenerateStub<IConfigurationView>();
            var fakeController = mocks.DynamicMock<ConfigurationController>(new object[] { fakeView });

            var fakeConfigurationSection = new ConfigurationSection();

            fakeConfigurationSection.target.Address = "10.0.0.1";
            fakeConfigurationSection.target.Username = "administrator";
            fakeConfigurationSection.target.Password = "P@asswOrd";
            fakeConfigurationSection.target.AdministrativePassword = "qwerty";

            using (mocks.Record())
            {
                Expect.Call(fakeController.FileExists(null)).IgnoreArguments().Return(true);
                Expect.Call(fakeController.ReadConfigurationSection(null)).IgnoreArguments().Return(fakeConfigurationSection);
            }

            mocks.ReplayAll();

            fakeController.view_OnReadConfiguration(this, EventArgs.Empty);

            mocks.VerifyAll();

            Assert.AreEqual(String.Empty, fakeView.Server.Address);
            Assert.AreEqual(String.Empty, fakeView.Server.Port);
            Assert.AreEqual(String.Empty, fakeView.Server.Username);
            Assert.AreEqual(String.Empty, fakeView.Server.Password);

            Assert.IsNotNull(fakeView.Target);
            Assert.AreEqual("10.0.0.1", fakeView.Target.Address);
            Assert.AreEqual("administrator", fakeView.Target.Username);
            Assert.AreEqual("P@asswOrd", fakeView.Target.Password);
            Assert.AreEqual("qwerty", fakeView.Target.AdministrativePassword);

            Assert.AreEqual(String.Empty, fakeView.DestinationFolder);
            Assert.AreEqual(String.Empty, fakeView.DefinitionFilename);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_get_a_valid_file_configuration_from_a_section_object()
        {
            MockRepository mocks = new MockRepository();

            var fakeView = MockRepository.GenerateStub<IConfigurationView>();
            var fakeController = mocks.DynamicMock<ConfigurationController>(new object[] { fakeView });

            var fakeConfigurationSection = new ConfigurationSection();
            fakeConfigurationSection.file.SaveFolder = "C:\\Temp\\";
            fakeConfigurationSection.file.DefinitionFilename = "C:\\Temp\\definitions.xml";

            using (mocks.Record())
            {
                Expect.Call(fakeController.FileExists(null)).IgnoreArguments().Return(true);
                Expect.Call(fakeController.ReadConfigurationSection(null)).IgnoreArguments().Return(fakeConfigurationSection);
            }

            mocks.ReplayAll();

            fakeController.view_OnReadConfiguration(this, EventArgs.Empty);

            mocks.VerifyAll();

            Assert.AreEqual(String.Empty, fakeView.Server.Address);
            Assert.AreEqual(String.Empty, fakeView.Server.Port);
            Assert.AreEqual(String.Empty, fakeView.Server.Username);
            Assert.AreEqual(String.Empty, fakeView.Server.Password);

            Assert.AreEqual(String.Empty, fakeView.Target.Address);
            Assert.AreEqual(String.Empty, fakeView.Target.Username);
            Assert.AreEqual(String.Empty, fakeView.Target.Password);
            Assert.AreEqual(String.Empty, fakeView.Target.AdministrativePassword);

            Assert.AreEqual("C:\\Temp\\", fakeView.DestinationFolder);
            Assert.AreEqual("C:\\Temp\\definitions.xml", fakeView.DefinitionFilename);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_try_gettting_a_configuration_without_a_configuration_file()
        {
            MockRepository mocks = new MockRepository();

            var fakeView = MockRepository.GenerateStub<IConfigurationView>();
            var fakeController = mocks.DynamicMock<ConfigurationController>(new object[] { fakeView });

            using (mocks.Record())
            {
                Expect.Call(fakeController.FileExists(null)).IgnoreArguments().Return(false).Repeat.Twice();
            }

            mocks.ReplayAll();

            fakeController.view_OnReadConfiguration(this, EventArgs.Empty);

            mocks.VerifyAll();

            Assert.IsNotNull(fakeView.Server);
            Assert.IsNotNull(fakeView.Target);

            Assert.IsNull(fakeView.Server.Address);
            Assert.IsNull(fakeView.Server.Port);
            Assert.IsNull(fakeView.Server.Username);
            Assert.IsNull(fakeView.Server.Password);

            Assert.IsNull(fakeView.Target.Address);
            Assert.IsNull(fakeView.Target.Username);
            Assert.IsNull(fakeView.Target.Password);
            Assert.IsNull(fakeView.Target.AdministrativePassword);

            Assert.IsNull(fakeView.DestinationFolder);
            Assert.IsNull(fakeView.DefinitionFilename);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_raising_a_configuration_event_on_a_view()
        {
            MockRepository mocks = new MockRepository();

            var fakeView = mocks.DynamicMock<IConfigurationView>();

            mocks.ReplayAll();

            var controller = new ConfigurationController(fakeView);

            fakeView.Raise(x => x.OnReadConfiguration += null, this, EventArgs.Empty);
            fakeView.Raise(x => x.OnWriteConfiguration += null, this, EventArgs.Empty);

            mocks.VerifyAll();

            Assert.IsTrue(controller.OnReadConfigurationCalled);
            Assert.IsTrue(controller.OnWriteConfigurationCalled);
        }

        [TestMethod, Owner("cpaiva")]
        public void If_there_are_errors_getting_a_configuration_an_error_message_is_expected()
        {
            MockRepository mocks = new MockRepository();

            var view = new ConfigurationView();
            var fakeController = mocks.DynamicMock<ConfigurationController>(new object[] { view });

            using (mocks.Record())
            {
                Expect.Call(fakeController.FileExists(null)).IgnoreArguments().Return(true);
                Expect.Call(fakeController.ReadConfigurationSection(null)).IgnoreArguments().Throw(new Exception("Exception handling test", new Exception("Inner Exception test")));
            }

            mocks.ReplayAll();

            fakeController.view_OnReadConfiguration(this, EventArgs.Empty);

            mocks.VerifyAll();

            Assert.AreEqual("Exception handling test\r\nInner Exception:\r\nInner Exception test", view.LastErrorMessage);
        }
        
        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_save_a_empty_configuration()
        {
            MockRepository mocks = new MockRepository();

            var fakeView = MockRepository.GenerateStub<IConfigurationView>();
            fakeView.Server = new ServerConfiguration();
            fakeView.Target = new TargetConfiguration();

            var fakeController = mocks.DynamicMock<ConfigurationController>(new object[] { fakeView });

            using (mocks.Record())
            {
                Expect.Call(() => fakeController.CreateProgramFolder()).IgnoreArguments();
            }

            mocks.ReplayAll();

            fakeController.view_OnWriteConfiguration(this, EventArgs.Empty);

            mocks.VerifyAll();

            var section = fakeController.Section;
            Assert.IsNotNull(section);

            Assert.IsNotNull(section.server);
            Assert.AreEqual(String.Empty, section.server.Address);
            Assert.AreEqual(String.Empty, section.server.Port);
            Assert.AreEqual(String.Empty, section.server.Username);
            Assert.AreEqual(String.Empty, section.server.Password);

            Assert.IsNotNull(section.target);
            Assert.AreEqual(String.Empty, section.target.Address);
            Assert.AreEqual(String.Empty, section.target.Username);
            Assert.AreEqual(String.Empty, section.target.Password);
            Assert.AreEqual(String.Empty, section.target.AdministrativePassword);

            Assert.IsNotNull(section.file);
            Assert.AreEqual(String.Empty, section.file.DefinitionFilename);
            Assert.AreEqual(String.Empty, section.file.SaveFolder);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_not_be_possible_save_a_configuration_with_empty_server_or_target_configuration()
        {
            MockRepository mocks = new MockRepository();

            var fakeView = new ConfigurationView();
            var fakeController = mocks.DynamicMock<ConfigurationController>(new object[] { fakeView });

            using (mocks.Record())
            {
                Expect.Call(() => fakeController.CreateProgramFolder()).IgnoreArguments();
            }

            mocks.ReplayAll();

            fakeController.view_OnWriteConfiguration(this, EventArgs.Empty);

            mocks.VerifyAll();

            Assert.IsNull(fakeController.Section);
            Assert.AreEqual(Resource.InvalidConfiguration, fakeView.LastErrorMessage);
        }

        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_save_a_configuration()
        {
            MockRepository mocks = new MockRepository();

            var fakeView = MockRepository.GenerateStub<IConfigurationView>();
            
            fakeView.Server = new ServerConfiguration();  
            fakeView.Server.Address = "http://localhost";
            fakeView.Server.Port = "1024";
            fakeView.Server.Username = "admin";
            fakeView.Server.Password = "PassworD";

            fakeView.Target = new TargetConfiguration();
            fakeView.Target.Address = "10.0.0.1";
            fakeView.Target.Username = "administrator";
            fakeView.Target.Password = "P@asswOrd";
            fakeView.Target.AdministrativePassword = "qwerty";

            fakeView.DestinationFolder = "C:\\Temp\\";
            fakeView.DefinitionFilename = "C:\\Temp\\definitions.xml";

            var fakeController = mocks.StrictMock<ConfigurationController>(new object[] { fakeView });

            using (mocks.Record())
            {
                Expect.Call(() => fakeController.CreateProgramFolder()).IgnoreArguments();
                Expect.Call(() => fakeController.SaveConfiguration(null)).IgnoreArguments();
            }

            mocks.ReplayAll();

            fakeController.view_OnWriteConfiguration(this, EventArgs.Empty);

            mocks.VerifyAll();

            var section = fakeController.Section;
            Assert.IsNotNull(section);

            Assert.IsNotNull(section.server);
            Assert.AreEqual("http://localhost", section.server.Address);
            Assert.AreEqual("1024", section.server.Port);
            Assert.AreEqual("admin", section.server.Username);
            Assert.AreEqual("PassworD", section.server.Password);

            Assert.IsNotNull(section.target);
            Assert.AreEqual("10.0.0.1", section.target.Address);
            Assert.AreEqual("administrator", section.target.Username);
            Assert.AreEqual("P@asswOrd", section.target.Password);
            Assert.AreEqual("qwerty", section.target.AdministrativePassword);

            Assert.IsNotNull(section.file);
            Assert.AreEqual("C:\\Temp\\definitions.xml", section.file.DefinitionFilename);
            Assert.AreEqual("C:\\Temp\\", section.file.SaveFolder);
        }
    }
}
