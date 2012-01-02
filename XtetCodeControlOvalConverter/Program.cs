using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aspose.Cells;
using System.IO;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions.SapCode;

namespace XtetCodeControlOvalConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            Workbook workbook = new Workbook();
            //try
            //{
            var f = File.Open("C:\\Users\\mgaspar\\Documents\\RMKB - SAP AG - XTET_EN.xlsx", FileMode.Open);
            workbook.Open(f);
            var controlsSheet = workbook.Worksheets["KB Controls"];
            int nFirstRow = 3;
            int nDefinitionColumn = 11;
            int nNameColumn = 1;
            var refState = "oval:xtet:ste:1";
            var serviceVarName = "oval:xtet:var:1";
            oval_definitions oval = new oval_definitions();
            oval.generator = DocumentHelpers.GetDefaultGenerator();
            var definitionList = new List<DefinitionType>();
            var testList = new List<TestType>();
            var objectList = new List<ObjectType>();
            while (!string.IsNullOrWhiteSpace(controlsSheet.Cells[nFirstRow, nNameColumn].StringValue))
            {
                var defName = controlsSheet.Cells[nFirstRow, nNameColumn].StringValue;
                var defDescription = controlsSheet.Cells[nFirstRow, nNameColumn+1].StringValue;
                var defId = controlsSheet.Cells[nFirstRow, nDefinitionColumn].StringValue;
                var nDefinition = GetIssueFromDefinitionId(defId);
                var newdefinition = new DefinitionType() { id = defId, metadata = new MetadataType() { title = defName, description = defDescription}};
                var testName = string.Format("oval:xtet:tst:{0}", nDefinition);
                var objName = string.Format("oval:xtet:obj:{0}", nDefinition);
                newdefinition.criteria = new DefinitionCriteriaType() { Items = new[] { new DefinitionCriterionType() { negate = true, test_ref = testName } } };
                definitionList.Add(newdefinition); 
                
                var newTest = new sapcode_test() { 
                    id = testName,
                    @object = new ObjectRefType() { object_ref = objName}, 
                    state = new[]{ new StateRefType() { state_ref = refState}}
                };
                testList.Add(newTest);

                var newObject = new sapcode_object()
                {
                     id = objName, 
                };
                newObject.Items = new[] { 
                     new EntityObjectIntType() { var_ref = serviceVarName},
                     new EntityObjectIntType() { Value = nDefinition.ToString() }
                };
                newObject.ItemsElementName = new[] {
                    SapCodeObjectItemsChoices.system_name, 
                    SapCodeObjectItemsChoices.issue
                };  
                objectList.Add(newObject);

                nFirstRow++;
            }
            oval.definitions = definitionList.ToArray();
            oval.tests = testList.ToArray();
            oval.objects = objectList.ToArray();
            oval.states = new StateType[] 
                { 
                    new sapcode_state() {
                        id = refState, 
                        errors = new EntityStateIntType() { datatype = DatatypeEnumeration.@int, Value = "0" }
                    }
                };
            oval.variables = new [] 
                { 
                    new External_variable() {
                        id = serviceVarName, 
                         datatype = DatatypeEnumeration.@int,
                         comment = "System Id"
                    }
                };
            var ovalFileName = "c:\\Users\\mgaspar\\Documents\\oval.xml";
            File.WriteAllText(ovalFileName, oval.GetDefinitionsXml());
				
        }
        static int GetIssueFromDefinitionId(string definitionId)
        {
            return Convert.ToInt32(definitionId.Split(":".ToCharArray()).Last());
        }
    }
}
