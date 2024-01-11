using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;

namespace Elektroskandia.WorkflowStarter
{
    public class WorkflowStarterJob : SPJobDefinition
    {
        /// <summary>
        /// Initializes a new instance of the WorkflowStarterJob class.
        /// </summary>
        public WorkflowStarterJob()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the WorkflowStarterJob class.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="service">The service.</param>
        /// <param name="server">The server.</param>
        /// <param name="targetType">Type of the target.</param>
        public WorkflowStarterJob(string jobName, SPService service, SPServer server, SPJobLockType targetType)
            : base(jobName, service, server, targetType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WorkflowStarterJob class.
        /// </summary>
        /// <param name="jobName">Name of the job.</param>
        /// <param name="webApplication">The web application.</param>
        public WorkflowStarterJob(string jobName, SPWebApplication webApplication)
            : base(jobName, webApplication, null, SPJobLockType.ContentDatabase)
        {
            this.Title = "Workflow Starter";
        }

        /// <summary>
        /// Executes the specified content db id.
        /// </summary>
        /// <param name="contentDbId">The content db id.</param>
        public override void Execute(Guid contentDbId)
        {
            // get a reference to the current site collection's content database
            SPWebApplication webApplication = this.Parent as SPWebApplication;
            SPContentDatabase contentDb = webApplication.ContentDatabases[contentDbId];

            SPWeb spVeb = contentDb.Sites[0].RootWeb;
            StartWorkflows(spVeb);
        }

        /// <summary>
        /// Procedure for starting all processes named "DailyWork" on all sites 
        /// </summary>
        private void StartWorkflows(SPWeb MyWeb)
        {
            //сначала запускаем рабочие процессы на текущем сайте
            SPWorkflowManager WM = MyWeb.Site.WorkflowManager;
            for (int j = 0; j < MyWeb.Lists.Count; j++)
            {
                SPWorkflowAssociationCollection WAC = MyWeb.Lists[j].WorkflowAssociations;

                foreach (SPWorkflowAssociation WA in WAC)
                {
                    if (string.Compare(WA.Name.ToString(), "DailyWork") == 0)
                    {
                        if (WA.Enabled == true)
                        {
                            //тут надо чтобы был хоть 1 элемент
                            if (MyWeb.Lists[j].Items.Count > 0) 
                            {
                                WM.StartWorkflow(MyWeb.Lists[j].Items[0], WA, WA.AssociationData, true);
                            }
                        }
                    }
                }
            }

            //а потом проходим по всем подчиненным и для них запускаем то же самое
            for (int i = 0; i < MyWeb.Webs.Count; i++)
            {
                StartWorkflows(MyWeb.Webs[i]);
            }
        }
    }
}
