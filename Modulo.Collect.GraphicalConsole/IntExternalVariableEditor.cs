using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Definitions;
using System.Windows.Forms;

namespace Modulo.Collect.GraphicalConsole
{
    class IntExternalVariableEditor : ExternalVariableEditor
    {
        public IntExternalVariableEditor(VariablesTypeVariableExternal_variable externalVariable)
            : base(externalVariable) { }

        public override Control GetEditor(string value)
        {
            if (this.externalVariable.HasPossibleValues() && (!this.externalVariable.HasPossibleRestriction()))
            {
                return this.configureComboBox(value);
            }
            else
            {
                return this.configureSpinEdit(value);
            }
        }

        private ComboBox configureComboBox(string value)
        {
            IEnumerable<PossibleValueType> possibleValues = this.externalVariable.GetPossibleValues();

            ComboBox comboBox = new ComboBox();
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Name = this.externalVariable.id;

            List<ComboBoxItem> items = new List<ComboBoxItem>();
            foreach (PossibleValueType possibleValue in possibleValues)
            {
                items.Add(new ComboBoxItem(!String.IsNullOrEmpty(possibleValue.hint) ? possibleValue.hint : possibleValue.Value, possibleValue.Value));
            }
            
            comboBox.Items.AddRange(items.ToArray());
            comboBox.DisplayMember = "Display";
            comboBox.ValueMember = "Value";

            if (!String.IsNullOrEmpty(value))
            {
                comboBox.SelectedIndex = items.IndexOf(items.Where(n => n.Value == value).FirstOrDefault());
            }
            
            return comboBox;
        }

        private NumericUpDown configureSpinEdit(string value)
        {
            NumericUpDown spinEdit = new NumericUpDown();
            spinEdit.Name = this.externalVariable.id;
            spinEdit.Maximum = decimal.MaxValue;
            spinEdit.Minimum = decimal.MinValue;

            Decimal result;
            if (Decimal.TryParse(value, out result))
            {
                spinEdit.Value = result;
            }

            return spinEdit;
        }
    }
}
