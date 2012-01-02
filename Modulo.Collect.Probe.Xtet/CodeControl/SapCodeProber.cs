using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Modulo.Collect.OVAL.Common;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.variableEvaluator;
using Modulo.Collect.OVAL.SystemCharacteristics;
using Modulo.Collect.Probe.Common;
using Definitions = Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Definitions.SapCode;
using Modulo.Collect.OVAL.SystemCharacteristics.SapCode;
using Modulo.Collect.Probe.Common.BasicClasses;
using System;
using Modulo.Collect.Probe.Common.Helpers;
using Modulo.Collect.Probe.Xtet.CodeControlWS;
using ObjectType = Modulo.Collect.OVAL.Definitions.ObjectType;

namespace Modulo.Collect.Probe.CodeControl
{
    [ProbeCapability(OvalObject="sapcode", PlataformName=FamilyEnumeration.undefined)]
    public class SapCodeProberWindows : ProbeBase, IProbe
    {
        public TargetInfo TargetInfo { get; set; }

        protected override void OpenConnectionProvider(IList<IConnectionProvider> connectionContext, TargetInfo target)
        {
            this.TargetInfo = target;
            this.ConnectionProvider = this.ConnectionManager.Connect<CodeControlConnectionProvider>(connectionContext, target);
        }

        protected override void ConfigureObjectCollector()
        {
            if (ObjectCollector == null)
                ObjectCollector = 
                    new SapCodeObjectCollector()
                    {
                        connectionProvider = ((CodeControlConnectionProvider)ConnectionProvider).connectionProvider,
                        AuthUser = TargetInfo.credentials.GetUserName(),
                        AuthPassword = TargetInfo.credentials.GetPassword()
                    };

            if (ItemTypeGenerator == null)
                ItemTypeGenerator = new SapCodeItemTypeGenerator();
               
        }

        
                
        protected override set GetSetElement(Definitions.ObjectType objectType)
        {
            return null;
        }

        protected override IEnumerable<Definitions.ObjectType> GetObjectsOfType(IEnumerable<Definitions.ObjectType> objectTypes)
        {
            return objectTypes.OfType<sapcode_object>();
        }

        protected override ItemType CreateItemTypeWithErrorStatus(string errorMessage)
        {
            return new sapcode_item() { status = StatusEnumeration.error, message = MessageType.FromErrorString(errorMessage) };
        }
    }

    public class SapCodeObjectCollector : BaseObjectCollector
    {
        public ScanWS connectionProvider;

        public bool isSapInitialized = false;

        public override IList<string> GetValues(Dictionary<string, object> parameters)
        {
            return new List<string>();
        }

        private ScanResultDTO issueResult;
        public string AuthPassword;
        public string AuthUser;

        public override void PrepareCollectionOfObjects(IEnumerable<ObjectType> allItemsToCollect, VariablesEvaluated variables)
        {
            base.PrepareCollectionOfObjects(allItemsToCollect, variables);
            if (allItemsToCollect.Count() > 0)
            {
                var variableEvaluator = new VariableEntityEvaluator(variables);

                var allSapObjects = allItemsToCollect.OfType<sapcode_object>().ToList();
                var issues = allSapObjects.SelectMany(x => variableEvaluator.EvaluateVariableForEntity(((EntitySimpleBaseType)(x.Items[x.ItemsElementName.ToList().IndexOf(SapCodeObjectItemsChoices.issue)])))).Distinct();
                var systemNames = allSapObjects.SelectMany(x => variableEvaluator.EvaluateVariableForEntity(((EntitySimpleBaseType)x.Items[x.ItemsElementName.ToList().IndexOf(SapCodeObjectItemsChoices.system_name)]))).Distinct();

                var systemIds = systemNames.Select(x => Convert.ToInt32(x));
                if (systemIds.Count() > 1)
                    throw new NotSupportedException("Only concurrent collections of a single system is supported!");

                ExecutionLogBuilder.AddInfo(string.Format("Authenticating at code control with user '{0}'.", AuthUser));
                var authResult = connectionProvider.authenticate(AuthUser, AuthPassword);
                if (authResult.error)
                {
                    ExecutionLogBuilder.AnErrorOccurred(string.Format("Error authenticating at code control : {0}.", authResult.errorMessage));
                }
                else
                {
                    ExecutionLogBuilder.AddInfo(string.Format("Successfully authenticated.", AuthUser));
                    int nSystem = systemIds.Single();
                    var allIssues = issues.Select(x => (long)Convert.ToInt32(x)).ToArray();
                    ExecutionLogBuilder.AddInfo(
                        string.Format("Starting scan request for system {0} and issues '{1}'.", nSystem, string.Join(",", allIssues)));
                    issueResult = connectionProvider.scanIssueListBySystem(authResult.token, nSystem,allIssues);
                                                                           
                    var scanCriteria = new ScanCriteriaDTO() {scanIdList = new[] {issueResult.scanId ?? 0}};

                    var waitTime = 0L;
                    const int timeOut = 600000;
                    while (((issueResult.status == "AWAITING") || (issueResult.status == "PROCESSING"))
                           && (waitTime <= timeOut)
                        )
                    {
                        Thread.Sleep(1000);
                        issueResult = connectionProvider.findScan(authResult.token, scanCriteria).FirstOrDefault();
                        // Wait time is desconsidering remote call duration, 
                        // should be done with a stop watch
                        waitTime += 1000;
                    }
                }


            }
        }
        protected override IEnumerable<CollectedItem> collectDataForSystemItem(ItemType systemItem)
        {
            base.ExecutionLogBuilder.CollectingDataFrom("SapCode Object");

            var sapCodeItem = ((sapcode_item)systemItem);

            //var systemId = Convert.ToInt32(sapCodeItem.system_name.Value);
            //var authResult = connectionProvider.authenticate("modulo", "modulo01");
            //if (!isSapInitialized)
            //{
            //    base.ExecutionLogBuilder.CollectingDataFrom(string.Format("Sap Connection for system {0} initialized.", systemId));

            //    isSapInitialized = true;
            //}
            //var issueResult = connectionProvider.scanIssueListBySystem(authResult.token, systemId, new[] { (long)Convert.ToInt32(sapCodeItem.issue.Value) });
            if (issueResult == null)
            {
                sapCodeItem.status = StatusEnumeration.notcollected;
            } else if (issueResult.status != "COMPLETE")
            {
                sapCodeItem.status = StatusEnumeration.error;
            }
            else
            {
                var evidencesForThisIssue =
                    issueResult.evidenceList.Where(x => x.issueId == Convert.ToInt32(sapCodeItem.issue.Value)).ToList();
                var nIssuesForThisId = evidencesForThisIssue.Count;

                sapCodeItem.total_issues_found =
                    OvalHelper.CreateItemEntityWithIntegerValue(nIssuesForThisId.ToString());
                sapCodeItem.total_programs_scanned =
                    OvalHelper.CreateItemEntityWithIntegerValue(issueResult.totalProgramsScanned.ToString());
                var formattedEvidences = new List<EntityItemStringType>();
                foreach (var programEvidences in evidencesForThisIssue.GroupBy(x => x.programName))
                {
                    formattedEvidences.Add(
                        OvalHelper.CreateItemEntityWithStringValue(string.Format("Program name: '{0}'",
                                                                                 programEvidences.Key)));
                    foreach (var programEvidence in programEvidences)
                    {
                        formattedEvidences.Add(
                            OvalHelper.CreateItemEntityWithStringValue(string.Format("Line number: {0}",
                                                                                     programEvidence.lineNumber)));
                        formattedEvidences.Add(
                            OvalHelper.CreateItemEntityWithStringValue(string.Format("Details: {0}",
                                                                                     programEvidence.details)));
                    }
                }
                sapCodeItem.evidences = formattedEvidences.ToArray();
            } 
            return new ItemTypeHelper().CreateCollectedItemsWithOneItem(systemItem, BuildExecutionLog());
            
        }
    }
}
