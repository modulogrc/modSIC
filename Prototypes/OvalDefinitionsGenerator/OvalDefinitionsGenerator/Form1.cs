using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Modulo.Collect.OVAL.Definitions;
using System.IO;

namespace OvalDefinitionsGenerator
{
    public partial class frmMain : Form
    {
        private Dictionary<String, IList<String>> DefinitionsMapping;

        public frmMain()
        {
            InitializeComponent();

            this.DefinitionsMapping = new Dictionary<string, IList<string>>();

            var mappedDefinitions = DefinitionsGeneratorConfiguration.Settings.Definitions;
            foreach (CollectConfigurationElement definition in mappedDefinitions)
            {
                var mapList = new List<String>();
                foreach (TrackElement track in definition.Tracks)
                    mapList.Add(track.Id);
                
                this.DefinitionsMapping.Add(definition.ID, mapList);
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            IEnumerable<String> errors = null;
            var sourceDefinitionsXML = File.ReadAllText(txtSourceDefinitionsFilename.Text);
            var sourceDefinitionsStream = new MemoryStream(Encoding.UTF8.GetBytes(sourceDefinitionsXML));
            
            var sourceDefinitions = oval_definitions.GetOvalDefinitionsFromStream(sourceDefinitionsStream, out errors);
            var newDefinitions = oval_definitions.GetOvalDefinitionsFromStream(sourceDefinitionsStream, out errors);
            var referencedTests = new Dictionary<String, String>();

            #region Update Definition (and criteria)
            foreach (var newDefinition in newDefinitions.definitions)
            {
                var newDefinitionID = this.GetNewDefinitionID(newDefinition.id);
                newDefinition.id = newDefinitionID;

                if (newDefinition.criteria.Items.Count() == 1)
                {
                    var criterion = (CriterionType)newDefinition.criteria.Items.Single();
                    UpdateCriterion(criterion, newDefinition.id, referencedTests);
                }
                else
                {
                    int i = 0;
                    foreach (var criteria in newDefinition.criteria.Items)
                    {
                        if (criteria is CriteriaType)
                            AddReferencedTests((CriteriaType)criteria, newDefinition.id, ref i, referencedTests);
                        else if (criteria is CriterionType)
                        {
                            i++;
                            var subdefinitionID = String.Format("{0}0{1}", newDefinitionID, i);
                            UpdateCriterion((CriterionType)criteria, newDefinition.id, referencedTests);
                        }
                    }
                }
            }
            #endregion

            #region Update Tests
            foreach (var test in referencedTests)
            {
                var oldTestId = test.Key;
                var newTestId = test.Value;

                var testToBeUpdated = sourceDefinitions.tests.Single(tst => tst.id.Equals(oldTestId));
                testToBeUpdated.id = newTestId;

                var objectProperty = testToBeUpdated.GetType().GetProperty("@object");
                var stateProperty = testToBeUpdated.GetType().GetProperty("state");
                var objectRef = (ObjectRefType)objectProperty.GetValue(testToBeUpdated, null);
                var stateRef = (StateRefType)stateProperty.GetValue(testToBeUpdated, null);

            //    objectRef
                    
            }
            #endregion


        }

        private void UpdateCriterion(
            CriterionType criterion, 
            string newDefinitionID, 
            Dictionary<string, string> referencedTests)
        {
            var oldTestId = criterion.test_ref;
            var newTestId = newDefinitionID.Replace("def", "tst");

            criterion.test_ref = newTestId;
            referencedTests.Add(oldTestId, newTestId);
        }

        private void AddReferencedTests(CriteriaType criteria, string newDefinitionID, ref int i, Dictionary<String, String> tests)
        {
            foreach (var item in criteria.Items)
                if (item is CriteriaType)
                    AddReferencedTests((CriteriaType)item, newDefinitionID, ref i, tests);
                else
                {
                    i++;
                    var subdefinitionID = String.Format("{0}0{1}", newDefinitionID, i);
                    UpdateCriterion((CriterionType)item, newDefinitionID, tests);
                }
        }

        private string GetNewDefinitionID(string sourceDefinitionID)
        {
            var definitionMapping = this.DefinitionsMapping[sourceDefinitionID];
            if (definitionMapping == null)
                return null;

            return definitionMapping.Single(map => map.Contains(txtNewDefinitionsShortName.Text));
        }



    }
}
