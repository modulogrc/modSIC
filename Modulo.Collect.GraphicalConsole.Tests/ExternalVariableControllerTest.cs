using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Modulo.Collect.OVAL.Definitions;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Modulo.Collect.GraphicalConsole.Tests
{
    [TestClass]
    public class ExternalVariableControllerTest
    {
        [TestMethod, Owner("cpaiva")]
        public void Should_be_possible_create_controls_from_external_variables()
        {
            MockRepository mocks = new MockRepository();

            var definitionFilename = "oval_external_variable_test.xml";
            var stream = GetResourceStream(definitionFilename);
            var fakeHelper = mocks.DynamicMock<ExternalVariableHelper>();
            var fakeView = MockRepository.GenerateStub<IExternalVariableView>();
            var fakeController = MockRepository.GenerateStub<ExternalVariableController>(new object[] { fakeView });

            using (mocks.Record())
            {
                Expect.Call(fakeHelper.FileExists(null)).IgnoreArguments().Return(true);
                Expect.Call(fakeHelper.GetStream(null)).IgnoreArguments().Return(stream);
            }

            mocks.ReplayAll();

            string errors;
            IEnumerable<VariablesTypeVariableExternal_variable> externalVariables = fakeHelper.GetExternalVariablesFromFile(definitionFilename, out errors);
            
            var args = new CreateControlsEventArgs();
            args.Variables = externalVariables;
            args.Values = new Dictionary<string, string>();
            fakeController.view_OnCreateControls(this, args);

            mocks.VerifyAll();

            Assert.IsNotNull(fakeController.Controls);
            Assert.AreEqual(9, fakeController.Controls.Count);
            
            var controls = fakeController.Controls.ToArray();           
            //Assert.AreEqual("Variable 1 (string)", controls[0].Text);
            Assert.AreEqual("oval:tutorial:var:1", controls[0].Name);
            Assert.IsTrue(controls[0] is TextBox);

            //Assert.AreEqual("Variable 2 (string)", controls[2].Text);
            Assert.AreEqual("oval:tutorial:var:2", controls[1].Name);
            Assert.IsTrue(controls[1] is TextBox);

            //Assert.AreEqual("Variable 3 (int)", controls[4].Text);
            Assert.AreEqual("oval:tutorial:var:3", controls[2].Name);
            Assert.IsTrue(controls[2] is NumericUpDown);

            //Assert.AreEqual("Variable 4 (int)", controls[6].Text);
            Assert.AreEqual("oval:tutorial:var:4", controls[3].Name);
            Assert.IsTrue(controls[3] is NumericUpDown);

            //Assert.AreEqual("Variable 5 (string)", controls[8].Text);
            Assert.AreEqual("oval:tutorial:var:5", controls[4].Name);
            Assert.IsTrue(controls[4] is ComboBox);
            var combo = controls[4] as ComboBox;
            Assert.AreEqual("Value 1", ((ComboBoxItem) combo.Items[0]).Display);
            Assert.AreEqual("AUDIT_NONE", ((ComboBoxItem)combo.Items[0]).Value);
            Assert.AreEqual("Value 2", ((ComboBoxItem)combo.Items[1]).Display);
            Assert.AreEqual("AUDIT_(SUCCESS|SUCCESS_FAILURE)", ((ComboBoxItem)combo.Items[1]).Value);
            Assert.AreEqual("Value 3", ((ComboBoxItem)combo.Items[2]).Display);
            Assert.AreEqual("AUDIT_(FAILURE|SUCCESS_FAILURE)", ((ComboBoxItem)combo.Items[2]).Value);
            Assert.AreEqual("Value 4", ((ComboBoxItem)combo.Items[3]).Display);
            Assert.AreEqual("AUDIT_SUCCESS_FAILURE", ((ComboBoxItem) combo.Items[3]).Value);

            //Assert.AreEqual("Variable 6 (boolean)", controls[10].Text);
            Assert.AreEqual("oval:tutorial:var:6", controls[5].Name);
            Assert.IsTrue(controls[5] is CheckBox);

            //Assert.AreEqual("Variable 7 (binary)", controls[11].Text);
            Assert.AreEqual("oval:tutorial:var:7", controls[6].Name);
            Assert.IsTrue(controls[6] is TextBox);

            //Assert.AreEqual("oval:tutorial:var:8", controls[13].Name);
            Assert.IsTrue(controls[7] is Panel);
            var panelControls = controls[7].Controls;
            Assert.AreEqual("Variable 8 (boolean)", panelControls[0].Text);
            Assert.IsTrue(panelControls[0] is Label);
            Assert.AreEqual("True", panelControls[1].Text);
            Assert.IsTrue(panelControls[0] is Label);
            Assert.AreEqual("False", panelControls[2].Text);
            Assert.IsTrue(panelControls[0] is Label);

            //Assert.AreEqual("Variable 9 (int)", controls[14].Text);
            Assert.AreEqual("oval:tutorial:var:9", controls[8].Name);
            Assert.IsTrue(controls[8] is ComboBox);
            combo = controls[8] as ComboBox;
            Assert.AreEqual("Ignore", ((ComboBoxItem)combo.Items[0]).Display);
            Assert.AreEqual("0", ((ComboBoxItem)combo.Items[0]).Value);
            Assert.AreEqual("Warning", ((ComboBoxItem)combo.Items[1]).Display);
            Assert.AreEqual("1", ((ComboBoxItem)combo.Items[1]).Value);
            Assert.AreEqual("Block", ((ComboBoxItem)combo.Items[2]).Display);
            Assert.AreEqual("2", ((ComboBoxItem)combo.Items[2]).Value);
        }

        private Stream GetResourceStream(string filename)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(String.Format("Modulo.Collect.GraphicalConsole.Tests.samples.{0}", filename));
            return stream;
        }
    }
}
