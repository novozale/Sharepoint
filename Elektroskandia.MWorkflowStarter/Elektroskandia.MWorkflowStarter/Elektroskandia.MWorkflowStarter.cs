using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;

using Microsoft.SharePoint.Administration;

namespace Elektroskandia.MWorkflowStarter
{
	public partial class MWorkflowStarterCLS: SequenceActivity
	{
		public MWorkflowStarterCLS()
		{
			InitializeComponent();
        }

        #region Properties

        public static DependencyProperty __ContextProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(MWorkflowStarterCLS));

        [Description("Context")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowContext __Context
        {
            get
            {
                return ((WorkflowContext)(base.GetValue(MWorkflowStarterCLS.__ContextProperty)));
            }
            set
            {
                base.SetValue(MWorkflowStarterCLS.__ContextProperty, value);
            }
        }

        public static DependencyProperty __ListIdProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__ListId", typeof(string), typeof(MWorkflowStarterCLS));
        [Description("__ListId")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string __ListId
        {
            get
            {
                return ((string)(base.GetValue(MWorkflowStarterCLS.__ListIdProperty)));
            }
            set
            {
                base.SetValue(MWorkflowStarterCLS.__ListIdProperty, value);
            }
        }

        public static DependencyProperty __ListItemProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__ListItem", typeof(int), typeof(MWorkflowStarterCLS));
        [Description("__ListItem")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int __ListItem
        {
            get
            {
                return ((int)(base.GetValue(MWorkflowStarterCLS.__ListItemProperty)));
            }
            set
            {
                base.SetValue(MWorkflowStarterCLS.__ListItemProperty, value);
            }
        }

        public static DependencyProperty MyWFNameProperty = DependencyProperty.Register("MyWFName", typeof(string), typeof(MWorkflowStarterCLS));
        [ValidationOption(ValidationOption.Required)]
        public string MyWFName
        {
            get
            {
                return (string)base.GetValue(MyWFNameProperty);
            }
            set
            {
                base.SetValue(MyWFNameProperty, value);
            }
        }

        #endregion


        private void MWSCode(object sender, EventArgs e)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate() //запуск с повышенными привилегиями (system account)
                    {
                        using (SPSite Site = new SPSite(__Context.Site.ID))
                        {
                            using (SPWeb spVeb = Site.OpenWeb(GetServerRelUrlFromFullUrl(__Context.Web.Url)))
                            {
                                StartWorkflows(spVeb);
                            }
                        }
                    });
            }
            catch (SPException ex)
            {
            }
        }


        private void StartWorkflows(SPWeb MyWeb)
        {
            //сначала запускаем рабочие процессы на текущем сайте
            SPWorkflowManager WM = MyWeb.Site.WorkflowManager;
            for (int j = 0; j < MyWeb.Lists.Count; j++)
            {
                SPWorkflowAssociationCollection WAC = MyWeb.Lists[j].WorkflowAssociations;

                foreach (SPWorkflowAssociation WA in WAC)
                {
                    if (string.Compare(WA.Name.ToString(), this.MyWFName.ToString()) == 0)
                    {
                        if (WA.Enabled == true)
                        {
                            //тут надо чтобы был хоть 1 элемент
                            if (MyWeb.Lists[j].Items.Count > 0)
                            {
                                try
                                {
                                    WM.StartWorkflow(MyWeb.Lists[j].Items[0], WA, WA.AssociationData, true);
                                }
                                catch (SPException ex)
                                {
                                }
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

        //Получение относительного URL из полного
        internal static string GetServerRelUrlFromFullUrl(string url)
        {
            int index = url.IndexOf("//");
            if ((index < 0) || (index == (url.Length - 2)))
            {
                throw new ArgumentException();
            }
            int startIndex = url.IndexOf('/', index + 2);
            if (startIndex < 0)
            {
                return "/";
            }
            string str = url.Substring(startIndex);
            if (str.IndexOf("?") >= 0)
                str = str.Substring(0, str.IndexOf("?"));

            if (str.IndexOf(".aspx") > 0)
                str = str.Substring(0, str.LastIndexOf("/"));

            if ((str.Length > 1) && (str[str.Length - 1] == '/'))
            {
                return str.Substring(0, str.Length - 1);
            }
            return str;
        } 
	}
}
