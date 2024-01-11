using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Elektroskandia.WorkflowStarter
{
    class WorkflowStarterJobInstaller : SPFeatureReceiver
    {
        const string WORKFLOW_STARTER_JOB_NAME = "WorkflowStarter";

        /// <summary>
        /// Occurs after a Feature is installed.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"></see> object that represents the properties of the event.</param>
        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
        }

        /// <summary>
        /// Occurs when a Feature is uninstalled.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"></see> object that represents the properties of the event.</param>
        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
        }

        /// <summary>
        /// Occurs after a Feature is activated.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"></see> object that represents the properties of the event.</param>
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            // register the the current web
            SPSite site = properties.Feature.Parent as SPSite;

            // make sure the job isn't already registered
            foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
            {
                if (job.Name == WORKFLOW_STARTER_JOB_NAME)
                    job.Delete();
            }

            // install the job
            WorkflowStarterJob workflowStarterJob = new WorkflowStarterJob(WORKFLOW_STARTER_JOB_NAME, site.WebApplication);

            SPDailySchedule schedule = new SPDailySchedule();
            schedule.BeginHour = 2;
            schedule.BeginMinute = 0;
            schedule.BeginSecond = 0;
            schedule.EndHour = 5;
            schedule.EndMinute = 59;
            schedule.EndSecond = 59;
            workflowStarterJob.Schedule = schedule;

            workflowStarterJob.Update();
        }

        /// <summary>
        /// Occurs when a Feature is deactivated.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPFeatureReceiverProperties"></see> object that represents the properties of the event.</param>
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSite site = properties.Feature.Parent as SPSite;

            // delete the job
            foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
            {
                if (job.Name == WORKFLOW_STARTER_JOB_NAME)
                    job.Delete();
            }
        }
    }
}
