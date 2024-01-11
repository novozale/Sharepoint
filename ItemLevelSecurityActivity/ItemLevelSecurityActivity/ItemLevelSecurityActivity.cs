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

namespace ItemLevelSecurity
{
	public partial class ItemLevelSecurityActivity: SequenceActivity
	{
		public ItemLevelSecurityActivity()
		{
			InitializeComponent();
		}

        #region Properties

        public static DependencyProperty __ContextProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(ItemLevelSecurityActivity));

        [Description("Context")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowContext __Context
        {
            get
            {
                return ((WorkflowContext)(base.GetValue(ItemLevelSecurityActivity.__ContextProperty)));
            }
            set
            {
                base.SetValue(ItemLevelSecurityActivity.__ContextProperty, value);
            }
        }

        public static DependencyProperty NamesToRWProperty = DependencyProperty.Register("NamesToRW", typeof(System.Collections.ArrayList), typeof(ItemLevelSecurityActivity));
        [ValidationOption(ValidationOption.Required)]
        public System.Collections.ArrayList NamesToRW
        {
            get
            {
                return (System.Collections.ArrayList)base.GetValue(NamesToRWProperty);
            }
            set
            {
                base.SetValue(NamesToRWProperty, value);
            }
        }

        public static DependencyProperty NamesToRProperty = DependencyProperty.Register("NamesToR", typeof(System.Collections.ArrayList), typeof(ItemLevelSecurityActivity));
        //[ValidationOption(ValidationOption.Required)]
        public System.Collections.ArrayList NamesToR
        {
            get
            {
                return (System.Collections.ArrayList)base.GetValue(NamesToRProperty);
            }
            set
            {
                base.SetValue(NamesToRProperty, value);
            }
        }

        public static DependencyProperty RWPermissionLevelProperty = System.Workflow.ComponentModel.DependencyProperty.Register("RWPermissionLevel", typeof(string), typeof(ItemLevelSecurityActivity));
        [Description("RWPermissionLevel")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string RWPermissionLevel
        {
            get
            {
                return ((string)(base.GetValue(ItemLevelSecurityActivity.RWPermissionLevelProperty)));
            }
            set
            {
                base.SetValue(ItemLevelSecurityActivity.RWPermissionLevelProperty, value);
            }
        }

        public static DependencyProperty RPermissionLevelProperty = System.Workflow.ComponentModel.DependencyProperty.Register("RPermissionLevel", typeof(string), typeof(ItemLevelSecurityActivity));
        [Description("RPermissionLevel")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string RPermissionLevel
        {
            get
            {
                return ((string)(base.GetValue(ItemLevelSecurityActivity.RPermissionLevelProperty)));
            }
            set
            {
                base.SetValue(ItemLevelSecurityActivity.RPermissionLevelProperty, value);
            }
        }

        public static DependencyProperty __ListIdProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__ListId", typeof(string), typeof(ItemLevelSecurityActivity));
        [Description("ListId")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string __ListId
        {
            get
            {
                return ((string)(base.GetValue(ItemLevelSecurityActivity.__ListIdProperty)));
            }
            set
            {
                base.SetValue(ItemLevelSecurityActivity.__ListIdProperty, value);
            }
        }

        public static DependencyProperty __ListItemProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__ListItem", typeof(int), typeof(ItemLevelSecurityActivity));
        [Description("ListItem")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int __ListItem
        {
            get
            {
                return ((int)(base.GetValue(ItemLevelSecurityActivity.__ListItemProperty)));
            }
            set
            {
                base.SetValue(ItemLevelSecurityActivity.__ListItemProperty, value);
            }
        }

        #endregion

        private void ItemLevelSecurityCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            int FindInSharepointFlag;                           //Флаг - найден ли пользователь в Sharepoint
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate() //запуск с повышенными привилегиями (system account)
                                                                //для того, чтобы это работало, необходимо чтобы списку (элементу)
                                                                //для system account были назначены полные права (чтение и запись)
                {
                    using (SPSite Site = new SPSite (__Context.Site.ID))
                    {
                        using (SPWeb spVeb = Site.OpenWeb(GetServerRelUrlFromFullUrl(__Context.Web.Url)))
                        {
                            SPList List = spVeb.Lists[new Guid(this.__ListId)];
                            SPListItem listItem = List.GetItemById(this.__ListItem);

                            listItem.BreakRoleInheritance(true);              //снятие наследования

                            //Назначаем права на чтение
                            //группам
                            if (this.NamesToR != null)
                            {
                                for (int j = 0; j < NamesToR.Count; j++)          //перебор групп, кот. дано право на чтение
                                {
                                    for (int i = 0; i < spVeb.Groups.Count; i++)  //перебор групп которые есть на сайте и сравнение
                                    {
                                        if (string.Compare(NamesToR[j].ToString(), spVeb.Groups[i].Name.ToString()) == 0) //если такая есть на сайте
                                        {
                                            SPGroup group = spVeb.Groups[i];
                                            SPPrincipal[] principals = { group };

                                            for (int k = 0; k < listItem.Web.RoleDefinitions.Count; k++)
                                            {
                                                if (string.Compare(listItem.Web.RoleDefinitions[k].Name, this.RPermissionLevel.ToString()) == 0)
                                                {
                                                    SPRoleDefinition roleDefinition = listItem.Web.RoleDefinitions[k];
                                                    try
                                                    {
                                                        //права на чтение назначаем только группам, для кот. что - то назначено для этого элемента
                                                        SPRoleAssignment roleAssignment = listItem.RoleAssignments.GetAssignmentByPrincipal(principals[0]);
                                                        roleAssignment.RoleDefinitionBindings.RemoveAll();
                                                        roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                                                        roleAssignment.Update();
                                                    }
                                                    catch
                                                    {

                                                    }
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }

                                //пользователям
                                for (int j = 0; j < NamesToR.Count; j++) //перебор групп, кот. дано право на чтение
                                {
                                    for (int i = 0; i < spVeb.Users.Count; i++) //перебор пользователей которые есть на сайте и сравнение
                                    {
                                        if (string.Compare(NamesToR[j].ToString(), spVeb.Users[i].LoginName.ToString()) == 0) //если такой есть на сайте
                                        {
                                            SPUser user = spVeb.Users[i];
                                            SPPrincipal[] principals = { user };

                                            for (int k = 0; k < listItem.Web.RoleDefinitions.Count; k++)
                                            {
                                                if (string.Compare(listItem.Web.RoleDefinitions[k].Name, this.RPermissionLevel.ToString()) == 0)
                                                {
                                                    try
                                                    {
                                                        //права на чтение назначаем только пользователям, для кот. что - то назначено для этого элемента
                                                        SPRoleDefinition roleDefinition = listItem.Web.RoleDefinitions[k];
                                                        SPRoleAssignment roleAssignment = listItem.RoleAssignments.GetAssignmentByPrincipal(principals[0]);
                                                        roleAssignment.RoleDefinitionBindings.RemoveAll();
                                                        roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                                                        roleAssignment.Update();
                                                    }
                                                    catch
                                                    {

                                                    }
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }

                            //Назначаем права на запись
                            //группам
                            if (this.NamesToRW != null)
                            {
                                for (int j = 0; j < NamesToRW.Count; j++) //перебор групп, кот. дано право на запись
                                {
                                    for (int i = 0; i < spVeb.Groups.Count; i++) //перебор групп которые есть на сайте и сравнение
                                    {
                                        if (string.Compare(NamesToRW[j].ToString(), spVeb.Groups[i].Name.ToString()) == 0) //если такая есть на сайте
                                        {
                                            SPGroup group = spVeb.Groups[i];
                                            SPPrincipal[] principals = { group };

                                            for (int k = 0; k < listItem.Web.RoleDefinitions.Count; k++)
                                            {
                                                if (string.Compare(listItem.Web.RoleDefinitions[k].Name, this.RWPermissionLevel.ToString()) == 0)
                                                {
                                                    try
                                                    {
                                                        //права на запись назначаем только группам, для кот. что - то назначено для этого элемента
                                                        SPRoleDefinition roleDefinition = listItem.Web.RoleDefinitions[k];
                                                        SPRoleAssignment roleAssignment = listItem.RoleAssignments.GetAssignmentByPrincipal(principals[0]);
                                                        roleAssignment.RoleDefinitionBindings.RemoveAll();
                                                        roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                                                        roleAssignment.Update();
                                                    }
                                                    catch
                                                    {

                                                    }
                                                    break;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }

                                //пользователям
                                for (int j = 0; j < NamesToRW.Count; j++) //перебор пользователей, кот. дано право на запись
                                {
                                    FindInSharepointFlag = 0;
                                    for (int i = 0; i < spVeb.Users.Count; i++) //перебор пользователей которые есть на сайте и сравнение
                                    {
                                        if (string.Compare(NamesToRW[j].ToString(), spVeb.Users[i].LoginName.ToString()) == 0) //если такой есть на сайте
                                        {
                                            SPUser user = spVeb.Users[i];
                                            SPPrincipal[] principals = { user };

                                            for (int k = 0; k < listItem.Web.RoleDefinitions.Count; k++)
                                            {
                                                if (string.Compare(listItem.Web.RoleDefinitions[k].Name, this.RWPermissionLevel.ToString()) == 0)
                                                {
                                                    try
                                                    {
                                                        //права на чтение назначаем только пользователям, для кот. что - то назначено для этого элемента
                                                        SPRoleDefinition roleDefinition = listItem.Web.RoleDefinitions[k];
                                                        SPRoleAssignment roleAssignment = listItem.RoleAssignments.GetAssignmentByPrincipal(principals[0]);
                                                        roleAssignment.RoleDefinitionBindings.RemoveAll();
                                                        roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                                                        roleAssignment.Update();
                                                    }
                                                    catch
                                                    {
                                                        //если для этого пользователя ничего нет для этого элемента - то создаем
                                                        SPRoleAssignment roleAssignmentU = new SPRoleAssignment(spVeb.Users[i].LoginName, spVeb.Users[i].Email, spVeb.Users[i].Name, spVeb.Users[i].Notes);
                                                        //добавляем пользователя в объект роли
                                                        roleAssignmentU.RoleDefinitionBindings.Add(listItem.Web.RoleDefinitions[k]);
                                                        listItem.RoleAssignments.Add(roleAssignmentU);
                                                    }
                                                    break;
                                                }
                                            }
                                            FindInSharepointFlag = 1;
                                            break;
                                        }
                                    }
                                    if (FindInSharepointFlag == 0)   //Если пользователя нет в Sharepoint - его надо завести.
                                    {
                                        if (CheckAD(NamesToRW[j].ToString()) == true)
                                        {
                                            for (int l = 0; l < listItem.Web.RoleDefinitions.Count; l++)
                                            {
                                                if (string.Compare(listItem.Web.RoleDefinitions[l].Name, this.RWPermissionLevel.ToString()) == 0)
                                                {
                                                    SPRoleAssignment roleAssignmentN = new SPRoleAssignment(NamesToRW[j].ToString(), string.Empty, string.Empty, string.Empty);
                                                    //добавляем пользователя в объект роли
                                                    roleAssignmentN.RoleDefinitionBindings.Add(listItem.Web.RoleDefinitions[l]);
                                                    listItem.RoleAssignments.Add(roleAssignmentN);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
                //listItem.Update();
                return;
            }
            catch
            {
                return;
            }
        }

        //Function used to check if the username exists in the Active Directory
        private bool CheckAD(string AuthorName)
        {
            try
            {
                //Split the Domain name and the Username
                string[] Authors = AuthorName.Split('\\');

                //Set the path for Searching in the Active Directory
                string rootQuery = @"LDAP://" + Authors[0];

                //Set the filter condition for searching
                //string SearchFilter = @"(&(SAMAccountName=" + Authors[1] + ")(ObjectCategory=person)(objectClass=*))";
                string SearchFilter = @"(&(SAMAccountName=" + Authors[1] + ")(objectClass=*))";

                SearchResult Result = null;
                using (DirectoryEntry root = new DirectoryEntry(rootQuery))
                {
                    //Query the Active Directory
                    using (DirectorySearcher Searcher = new DirectorySearcher(root))
                    {
                        Searcher.Filter = SearchFilter;

                        //determine the result of searching the Active Directory
                        SearchResultCollection results = Searcher.FindAll();
                        Result = (results.Count != 0) ? results[0] : null;

                        //if the user exists then return true else return false
                        //if (Result.Properties["member"].Count > 0)
                        if (Result != null)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
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
