using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Modulo.Collect.OVAL.Variables;
using Definitions = Modulo.Collect.OVAL.Definitions;
using System.Windows.Forms;
using System.Text;

namespace Modulo.Collect.GraphicalConsole
{
    public class ExternalVariableHelper
    {
        public IEnumerable<Definitions.VariablesTypeVariableExternal_variable> GetExternalVariablesFromFile(string definitionFilename, out string errors)
        {
            try
            {
                if (definitionFilename == null)
                {
                    errors = Resource.EmptyDefinitionFilename;
                    return null;
                }

                if (!FileExists(definitionFilename))
                {
                    errors = Resource.OVALDefinitionsFileNotFound;
                    return null;
                }

                var definitions = GetOvalDefinitionsFromFile(definitionFilename, out errors);
                if (definitions == null)
                {
                    return null;
                }

                if (definitions.variables == null)
                {
                    return null;
                }

                return definitions.variables.OfType<Definitions.VariablesTypeVariableExternal_variable>();
            }
            catch (Exception ex)
            {
                errors = Util.FormatExceptionMessage(ex);
                return null;
            }
        }

        public bool IsThereExternalVariable(string definitionFilename, out string errors)
        {
            var externalVariables = GetExternalVariablesFromFile(definitionFilename, out errors);
            return ((externalVariables != null) && (externalVariables.Count() > 0));
        }

        public Definitions.oval_definitions GetOvalDefinitionsFromFile(string definitionFilename, out string errors)
        {
            IEnumerable<string> errorList;

            using (var stream = GetStream(definitionFilename))
            {
                var definitions = Definitions.oval_definitions.GetOvalDefinitionsFromStream(stream, out errorList);
                errors = errorList == null ? String.Empty : String.Join(String.Empty, errorList);
                return definitions;
            }
        }

        public string GetEvaluatedExternalVariables(Dictionary<String, String> externalVariablesValues, IEnumerable<Definitions.VariablesTypeVariableExternal_variable> externalVariables)
        {
            var variables = new oval_variables
            {
                generator = Collect.OVAL.Common.DocumentHelpers.GetDefaultGenerator(),
                variables = new List<VariableType>()
            };

            foreach (var item in externalVariables)
            {
                string defaultValue = externalVariablesValues.Where(n => n.Key == item.id).Select(m => m.Value).SingleOrDefault() ?? String.Empty;
                variables.variables.Add(new VariableType(item.datatype, item.id, defaultValue));
            }

            return variables.GetXmlDocument();
        }

        public String GetSelectItemValue(ComboBox combo)
        {
            if (combo.SelectedIndex >= 0)
            {
                return (combo.Items[combo.SelectedIndex] as ComboBoxItem).Value;
            }
            return String.Empty;
        }

        #region Useful Methods for Tests
        public virtual Stream GetStream(string filename)
        {
            string fileContent = File.ReadAllText(filename);
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            return stream;
        }

        public virtual bool FileExists(string filename)
        {
            return File.Exists(filename);
        }
        #endregion
    }
}
