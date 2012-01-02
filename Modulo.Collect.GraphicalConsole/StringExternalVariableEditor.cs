using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Definitions;
using System.Windows.Forms;
using System.Collections;

namespace Modulo.Collect.GraphicalConsole
{
    public class ComboBoxItem
    {
        public ComboBoxItem(String _display, String _value)
        {
            Display = _display;
            Value = _value;
        }

        public String Display { get; set; }
        public String Value { get; set; }
    }

    class StringExternalVariableEditor : ExternalVariableEditor
    {
        public StringExternalVariableEditor(VariablesTypeVariableExternal_variable externalVariable)
            : base(externalVariable) { }

        public override Control GetEditor(string value)
        {
            if (this.externalVariable.HasPossibleValues() && (!this.externalVariable.HasPossibleRestriction()))
            {
                return this.ConfigureComboBox(value);
            }
            return this.ConfigureTextBox(value);
        }

        private ComboBox ConfigureComboBox(string value)
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

        private TextBox ConfigureTextBox(string value)
        {
            IEnumerable<PossibleValueType> possibleValues = this.externalVariable.GetPossibleValues();

            TextBox textBox = new TextBox();
            textBox.Name = this.externalVariable.id;
            if (!String.IsNullOrEmpty(value))
            {
                textBox.Text = value;
            }

            return textBox;
        }
    }
}
