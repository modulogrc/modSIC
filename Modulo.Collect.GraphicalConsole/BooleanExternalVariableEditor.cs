using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Definitions;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;

namespace Modulo.Collect.GraphicalConsole
{
    class BooleanExternalVariableEditor : ExternalVariableEditor
    {
        public BooleanExternalVariableEditor(VariablesTypeVariableExternal_variable externalVariable)
            : base(externalVariable) { }

        public override Control GetEditor(string value)
        {
            if (!this.externalVariable.HasPossibleValues())
            {
                return this.ConfigureCheckBox(value);
            }
            else
            {
                return this.ConfigureRadioButtons(value);
            }
        }

        private CheckBox ConfigureCheckBox(string value)
        {
            CheckBox checkBox = new CheckBox() { Text = String.Format("{0}", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.externalVariable.comment)) };
            checkBox.Name = this.externalVariable.id;
            checkBox.AutoSize = true;

            if (!String.IsNullOrEmpty(value))
            {
                checkBox.Checked = bool.Parse(value);
            }
            return checkBox;
        }

        private Panel ConfigureRadioButtons(string value)
        {
            var ControlY = 0;
            int height;

            IEnumerable<PossibleValueType> possibleValues = this.externalVariable.GetPossibleValues();

            Panel panel = new Panel();
            panel.AutoSize = true;
            panel.Name = this.externalVariable.id;
            
            Label label = new Label() { Text = String.Format("{0}", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.externalVariable.comment)) };
            label.AutoSize = true;
            label.Location = new Point(0, ControlY);
            ControlY += 20;

            panel.Controls.Add(label);

            height = label.Height + 20;
            foreach (PossibleValueType possibleValue in possibleValues)
            {
                RadioButton radio = new RadioButton();
                radio.AutoSize = true;
                radio.Location = new Point(0, ControlY);
                ControlY += 25;

                radio.Name = String.Format("{0}-{1}", this.externalVariable.id, possibleValue.Value);
                radio.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(possibleValue.hint);
                radio.Tag = possibleValue.Value;

                if (possibleValue.Value == value)
                {
                    radio.Checked = true;
                }

                panel.Controls.Add(radio);
            }

            panel.MinimumSize = new Size(10, 10);
            panel.Height = 20 + possibleValues.Count() * 25;
            return panel;
        }
    }
}
