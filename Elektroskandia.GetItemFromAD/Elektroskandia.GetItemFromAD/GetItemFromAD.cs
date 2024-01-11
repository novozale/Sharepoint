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

using System.DirectoryServices;

namespace Elektroskandia.GetItemFromAD
{
	public partial class GetItemFromAD: SequenceActivity
	{
		public GetItemFromAD()
		{
			InitializeComponent();
		}


        #region Properties

        public static DependencyProperty MyActionTypeProperty = DependencyProperty.Register("MyActionType", typeof(string), typeof(GetItemFromAD));
        [ValidationOption(ValidationOption.Required)]
        public string MyActionType
        {
            get
            {
                return (string)base.GetValue(MyActionTypeProperty);
            }
            set
            {
                base.SetValue(MyActionTypeProperty, value);
            }
        }

        public static DependencyProperty MyValueProperty = DependencyProperty.Register("MyValue", typeof(string), typeof(GetItemFromAD));
        [ValidationOption(ValidationOption.Required)]
        public string MyValue
        {
            get
            {
                return (string)base.GetValue(MyValueProperty);
            }
            set
            {
                base.SetValue(MyValueProperty, value);
            }
        }

        public static DependencyProperty MyOutValueProperty = DependencyProperty.Register("MyOutValue", typeof(string), typeof(GetItemFromAD));
        [ValidationOption(ValidationOption.Required)]
        public string MyOutValue
        {
            get
            {
                return (string)base.GetValue(MyOutValueProperty);
            }
            set
            {
                base.SetValue(MyOutValueProperty, value);
            }
        }

        public static DependencyProperty __ContextProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(GetItemFromAD));

        [Description("Context")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowContext __Context
        {
            get
            {
                return ((WorkflowContext)(base.GetValue(GetItemFromAD.__ContextProperty)));
            }
            set
            {
                base.SetValue(GetItemFromAD.__ContextProperty, value);
            }
        }

        public static DependencyProperty __ListIdProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__ListId", typeof(string), typeof(GetItemFromAD));
        [Description("__ListId")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string __ListId
        {
            get
            {
                return ((string)(base.GetValue(GetItemFromAD.__ListIdProperty)));
            }
            set
            {
                base.SetValue(GetItemFromAD.__ListIdProperty, value);
            }
        }

        public static DependencyProperty __ListItemProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__ListItem", typeof(int), typeof(GetItemFromAD));
        [Description("__ListItem")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int __ListItem
        {
            get
            {
                return ((int)(base.GetValue(GetItemFromAD.__ListItemProperty)));
            }
            set
            {
                base.SetValue(GetItemFromAD.__ListItemProperty, value);
            }
        }


        #endregion

        private void GetItemFromADCode_ExecuteCode(object sender, EventArgs e)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate() //запуск с повышенными привилегиями (system account)
                                                                //для того, чтобы это работало, необходимо чтобы списку (элементу)
                                                                //для system account были назначены полные права (чтение и запись)
                {
                    using (SPSite Site = new SPSite(__Context.Site.ID))
                    {
                        using (SPWeb spVeb = Site.OpenWeb(GetServerRelUrlFromFullUrl(__Context.Web.Url)))
                        {
                            SPList List = spVeb.Lists[new Guid(this.__ListId)];
                            SPListItem listItem = List.GetItemById(this.__ListItem);
                            this.MyOutValue = GetUserInfo(this.MyValue, this.MyActionType);
                        }
                    }
                });
                return;
            }
            catch
            {
                return;
            }
        }

        public static string GetUserInfo(String MyUser, String SrcParam)
        /////////////////////////////////////////////////////////////////////////////////
        //
        // Получение запрошенной информации из AD
        //
        /////////////////////////////////////////////////////////////////////////////////
        {
            String User = "";
            String RezStr = "";

            User = GetPureUser(MyUser);
            try
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://eskru");
                DirectorySearcher Searcher = new DirectorySearcher(entry);
                Searcher.Filter = "(samaccountname=" + User + ")";
                Searcher.PropertiesToLoad.Add("Name");
                Searcher.PropertiesToLoad.Add("mail");
                Searcher.PropertiesToLoad.Add("telephoneNumber");
                Searcher.PropertiesToLoad.Add("mobile");
                SearchResultCollection Results;
                Results = Searcher.FindAll();
                if (Results.Count > 0)
                {
                    if (string.Compare(SrcParam, "Name") == 0)
                    {
                        RezStr = Results[0].Properties["Name"][0].ToString();
                        return RezStr;
                    }
                    if (string.Compare(SrcParam, "mail") == 0)
                    {
                        RezStr = Results[0].Properties["mail"][0].ToString();
                        return RezStr;
                    }
                    if (string.Compare(SrcParam, "telephoneNumber") == 0)
                    {
                        RezStr = Results[0].Properties["telephoneNumber"][0].ToString();
                        return RezStr;
                    }
                    if (string.Compare(SrcParam, "mobile") == 0)
                    {
                        RezStr = Results[0].Properties["mobile"][0].ToString();
                        return RezStr;
                    }
                    RezStr = "";
                    return RezStr;
                }
                else
                {
                    RezStr = "";
                    return RezStr;
                }
            }
            catch
            {
                RezStr = "";
                return RezStr;
            }
        }

        public static string GetPureUser(String SrcUser)
        /////////////////////////////////////////////////////////////////////////////////
        //
        // Получение логина из строки пользователя
        //
        /////////////////////////////////////////////////////////////////////////////////
        {
            int MyPos = 0;
            String PureUser = "";
            String MySS = "\\";

            MyPos = SrcUser.IndexOf(MySS);
            if (MyPos == -1)
            {
                PureUser = SrcUser;
            }
            else
            {
                PureUser = SrcUser.Substring(MyPos + 1);
            }
            return PureUser;
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
