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

namespace ItemTime
{
	public partial class ItemChangingTime: SequenceActivity
	{
		public ItemChangingTime()
		{
			InitializeComponent();
		}

        #region Properties

        public static DependencyProperty MyActionTypeProperty = DependencyProperty.Register("MyActionType", typeof(string), typeof(ItemChangingTime));
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

        public static DependencyProperty MyValueProperty = DependencyProperty.Register("MyValue", typeof(string), typeof(ItemChangingTime));
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

        public static DependencyProperty MyFieldNameProperty = DependencyProperty.Register("MyFieldName", typeof(string), typeof(ItemChangingTime));
        [ValidationOption(ValidationOption.Required)]
        public string MyFieldName
        {
            get
            {
                return (string)base.GetValue(MyFieldNameProperty);
            }
            set
            {
                base.SetValue(MyFieldNameProperty, value);
            }
        }

        public static DependencyProperty __ContextProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(ItemChangingTime));

        [Description("Context")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowContext __Context
        {
            get
            {
                return ((WorkflowContext)(base.GetValue(ItemChangingTime.__ContextProperty)));
            }
            set
            {
                base.SetValue(ItemChangingTime.__ContextProperty, value);
            }
        }

        public static DependencyProperty __ListIdProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__ListId", typeof(string), typeof(ItemChangingTime));
        [Description("__ListId")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string __ListId
        {
            get
            {
                return ((string)(base.GetValue(ItemChangingTime.__ListIdProperty)));
            }
            set
            {
                base.SetValue(ItemChangingTime.__ListIdProperty, value);
            }
        }

        public static DependencyProperty __ListItemProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__ListItem", typeof(int), typeof(ItemChangingTime));
        [Description("__ListItem")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int __ListItem
        {
            get
            {
                return ((int)(base.GetValue(ItemChangingTime.__ListItemProperty)));
            }
            set
            {
                base.SetValue(ItemChangingTime.__ListItemProperty, value);
            }
        }


        #endregion

        private void ItemChangingTimeCode_ExecuteCode(object sender, EventArgs e)
        {
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
                            if (string.Compare(this.MyActionType.ToString(), "Current Time") == 0)
                            //для поля устанавливаеми текущее время
                            {
                                //listItem["Изменен"] = "01/01/1900";
                                listItem[this.MyFieldName.ToString()] = System.DateTime.Now;
                                listItem.Update();
                            }
                            else
                            //для поля устанавливаем значение
                            {
                                listItem[this.MyFieldName.ToString()] = this.MyValue.ToString();
                                listItem.Update();
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
