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

using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Elektroskandia.ItemSetInDB
{
	public partial class ItemSetInDB: SequenceActivity
	{
		public ItemSetInDB()
		{
			InitializeComponent();
        }

        #region Properties
        public static DependencyProperty MyDataProperty = DependencyProperty.Register("MyData", typeof(string), typeof(ItemSetInDB));
        [ValidationOption(ValidationOption.Required)]
        public string MyData
        {
            get
            {
                return (string)base.GetValue(MyDataProperty);
            }
            set
            {
                base.SetValue(MyDataProperty, value);
            }
        }

        public static DependencyProperty MyParamFieldNameProperty = DependencyProperty.Register("MyParamFieldName", typeof(string), typeof(ItemSetInDB));
        [ValidationOption(ValidationOption.Required)]
        public string MyParamFieldName
        {
            get
            {
                return (string)base.GetValue(MyParamFieldNameProperty);
            }
            set
            {
                base.SetValue(MyParamFieldNameProperty, value);
            }
        }

        public static DependencyProperty __ContextProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(ItemSetInDB));

        [Description("Context")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowContext __Context
        {
            get
            {
                return ((WorkflowContext)(base.GetValue(ItemSetInDB.__ContextProperty)));
            }
            set
            {
                base.SetValue(ItemSetInDB.__ContextProperty, value);
            }
        }

        public static DependencyProperty __ListIdProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__ListId", typeof(string), typeof(ItemSetInDB));
        [Description("__ListId")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string __ListId
        {
            get
            {
                return ((string)(base.GetValue(ItemSetInDB.__ListIdProperty)));
            }
            set
            {
                base.SetValue(ItemSetInDB.__ListIdProperty, value);
            }
        }

        public static DependencyProperty __ListItemProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__ListItem", typeof(int), typeof(ItemSetInDB));
        [Description("__ListItem")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int __ListItem
        {
            get
            {
                return ((int)(base.GetValue(ItemSetInDB.__ListItemProperty)));
            }
            set
            {
                base.SetValue(ItemSetInDB.__ListItemProperty, value);
            }
        }

        #endregion

        private void ItemSetInDB_ExecuteCode(object sender, EventArgs e)
        ///////////////////////////////////////////////////////////////////////
        //
        // Выполнение кода
        //
        ///////////////////////////////////////////////////////////////////////
        {
            string MyErr = "";
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
                            MyErr = SetProjectState(this.MyData.ToString(), listItem[this.MyParamFieldName.ToString()].ToString());
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

        private static string SetProjectState(string MyNewState, string MyProjectID)
        ///////////////////////////////////////////////////////////////////////
        //
        // выставление нового состояния проекта
        //
        ///////////////////////////////////////////////////////////////////////
        {
            string MySQLStr = "";
            string MyErr = "";
            string connStr = "Data Source=SQLCLS;" +
                            "Initial Catalog=ScaDataDB;" +
                            "User id=sa;" +
                            "Password=sqladmin;";
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch(Exception e)
            {
                MyErr = e.Message;
                return MyErr;
            }
            MySQLStr = "UPDATE tbl_CRM_Projects ";
            MySQLStr = MySQLStr + "SET IsApproved = " + MyNewState.Trim() + " ";
            MySQLStr = MySQLStr + "WHERE (ProjectID = '" + MyProjectID.Trim() + "')";
            try
            {
                SqlCommand cmd = new SqlCommand(MySQLStr, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MyErr = e.Message;
                return MyErr;
            }
            conn.Close();
            conn.Dispose();
            return MyErr;
        }

        internal static string GetServerRelUrlFromFullUrl(string url)
        ///////////////////////////////////////////////////////////////////////
        //
        // Получение относительного URL из полного
        //
        ///////////////////////////////////////////////////////////////////////
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
