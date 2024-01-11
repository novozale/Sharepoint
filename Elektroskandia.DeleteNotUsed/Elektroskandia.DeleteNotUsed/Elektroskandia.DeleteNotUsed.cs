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

namespace Elektroskandia.DeleteNotUsed
{
	public partial class DeleteNotUsedCLS: SequenceActivity
	{
		public DeleteNotUsedCLS()
		{
			InitializeComponent();
		}

        #region Properties

        public static DependencyProperty __ContextProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(DeleteNotUsedCLS));

        [Description("Context")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowContext __Context
        {
            get
            {
                return ((WorkflowContext)(base.GetValue(DeleteNotUsedCLS.__ContextProperty)));
            }
            set
            {
                base.SetValue(DeleteNotUsedCLS.__ContextProperty, value);
            }
        }

        public static DependencyProperty ListIdProperty = System.Workflow.ComponentModel.DependencyProperty.Register("ListId", typeof(string), typeof(DeleteNotUsedCLS));
        [Description("ListId")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ListId
        {
            get
            {
                return ((string)(base.GetValue(DeleteNotUsedCLS.ListIdProperty)));
            }
            set
            {
                base.SetValue(DeleteNotUsedCLS.ListIdProperty, value);
            }
        }

        public static DependencyProperty MyValueProperty = System.Workflow.ComponentModel.DependencyProperty.Register("MyValue", typeof(System.Int32), typeof(DeleteNotUsedCLS));
        [Description("MyValue")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public System.Int32 MyValue
        {
            get
            {
                return ((System.Int32)(base.GetValue(DeleteNotUsedCLS.MyValueProperty)));
            }
            set
            {
                base.SetValue(DeleteNotUsedCLS.MyValueProperty, value);
            }
        }

        public static DependencyProperty MyFieldNameProperty = System.Workflow.ComponentModel.DependencyProperty.Register("MyFieldName", typeof(string), typeof(DeleteNotUsedCLS));
        [Description("MyFieldName")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string MyFieldName
        {
            get
            {
                return ((string)(base.GetValue(DeleteNotUsedCLS.MyFieldNameProperty)));
            }
            set
            {
                base.SetValue(DeleteNotUsedCLS.MyFieldNameProperty, value);
            }
        }

        public static DependencyProperty MyValue1Property = DependencyProperty.Register("MyValue1", typeof(System.Collections.ArrayList), typeof(DeleteNotUsedCLS));
        [Description("MyValue1")]
        [ValidationOption(ValidationOption.Required)]
        public System.Collections.ArrayList MyValue1
        {
            get
            {
                return (System.Collections.ArrayList)base.GetValue(MyValue1Property);
            }
            set
            {
                base.SetValue(MyValue1Property, value);
            }
        }

        #endregion


        private void DeleteNotUsedCode_ExecuteCode(object sender, EventArgs e)
        {
            bool MyFlag;
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
                            SPList ListChk = spVeb.Lists[new Guid(this.ListId)];     //проверяемый список
                            SPListItem listItem;

                            //проверяем все элементы
                            for (int i = ListChk.Items.Count - 1; i >= 0; i--)
                            {
                                listItem = ListChk.Items[i];
                                try
                                {
                                    //последний раз менялся более чем... дней назад
                                    if (DateTime.Compare(DateTime.Today, Convert.ToDateTime(listItem["Изменен"]).AddDays((double)this.MyValue)) > 0)
                                    {
                                        MyFlag = false;
                                        //при этом заданное поле равно одному из значений
                                        for (int j = 0; j < MyValue1.Count; j++)
                                        {
                                            if (string.Compare(listItem[MyFieldName.ToString()].ToString(), MyValue1[j].ToString()) == 0)
                                            {
                                                MyFlag = true;
                                            }
                                        }
                                        if (MyFlag == true)
                                        {
                                            listItem.Delete();
                                        }
                                    }
                                }
                                catch
                                {

                                }
                            }
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
