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

namespace Elektroskandia.MovingItemsByDate
{
	public partial class MovingItemsByDateCLS: SequenceActivity
	{
		public MovingItemsByDateCLS()
		{
			InitializeComponent();
		}

        #region Properties

        public static DependencyProperty __ContextProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(MovingItemsByDateCLS));

        [Description("Context")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowContext __Context
        {
            get
            {
                return ((WorkflowContext)(base.GetValue(MovingItemsByDateCLS.__ContextProperty)));
            }
            set
            {
                base.SetValue(MovingItemsByDateCLS.__ContextProperty, value);
            }
        }

        public static DependencyProperty FromListIdProperty = System.Workflow.ComponentModel.DependencyProperty.Register("FromListId", typeof(string), typeof(MovingItemsByDateCLS));
        [Description("FromListId")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string FromListId
        {
            get
            {
                return ((string)(base.GetValue(MovingItemsByDateCLS.FromListIdProperty)));
            }
            set
            {
                base.SetValue(MovingItemsByDateCLS.FromListIdProperty, value);
            }
        }

        public static DependencyProperty ToListIdProperty = System.Workflow.ComponentModel.DependencyProperty.Register("ToListId", typeof(string), typeof(MovingItemsByDateCLS));
        [Description("ToListId")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ToListId
        {
            get
            {
                return ((string)(base.GetValue(MovingItemsByDateCLS.ToListIdProperty)));
            }
            set
            {
                base.SetValue(MovingItemsByDateCLS.ToListIdProperty, value);
            }
        }

        public static DependencyProperty MyValueProperty = System.Workflow.ComponentModel.DependencyProperty.Register("MyValue", typeof(System.Int32), typeof(MovingItemsByDateCLS));
        [Description("MyValue")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public System.Int32 MyValue
        {
            get
            {
                return ((System.Int32)(base.GetValue(MovingItemsByDateCLS.MyValueProperty)));
            }
            set
            {
                base.SetValue(MovingItemsByDateCLS.MyValueProperty, value);
            }
        }

        public static DependencyProperty MyValue1Property = DependencyProperty.Register("MyValue1", typeof(string), typeof(MovingItemsByDateCLS));
        [ValidationOption(ValidationOption.Required)]
        public string MyValue1
        {
            get
            {
                return (string)base.GetValue(MyValue1Property);
            }
            set
            {
                base.SetValue(MyValue1Property, value);
            }
        }

        public static DependencyProperty MyFieldNameProperty = DependencyProperty.Register("MyFieldName", typeof(string), typeof(MovingItemsByDateCLS));
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

        #endregion

        private void MovingItemsByDateCode_ExecuteCode(object sender, EventArgs e)
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
                            SPList ListFrom = spVeb.Lists[new Guid(this.FromListId)];     //список откуда копируем
                            SPList ListTo = spVeb.Lists[new Guid(this.ToListId)];         //список куда копируем
                            SPListItem listItemTo;

                            //проверяем все элементы
                            for (int i = ListFrom.Items.Count - 1; i >= 0; i--)
                            {
                                listItemTo = ListFrom.Items[i];
                                try
                                {
                                    if (DateTime.Compare(DateTime.Today, Convert.ToDateTime(listItemTo["Изменен"]).AddDays((double)this.MyValue)) > 0)
                                    {
                                        if (string.Compare(listItemTo[this.MyFieldName.ToString()].ToString(), this.MyValue1.ToString()) == 0)
                                        {
                                            MoveItems(ListFrom, ListFrom.Items[i], ListTo);
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

        //Перемещаем подходящий по условию элемент
        private void MoveItems(SPList ListFrom, SPListItem listItemFrom, SPList ListTo)
        {
            SPListItem listItemTo = ListTo.Items.Add();

            //Переносим версии     
            for (int i = listItemFrom.Versions.Count - 1; i >= 0; i--)
            {
                foreach (SPField sourceField in listItemFrom.Fields)
                {
                    SPListItemVersion version = listItemFrom.Versions[i];
                    if ((!sourceField.ReadOnlyField) && (sourceField.Type != SPFieldType.Attachments))
                    {
                        if (version[sourceField.Title] != null)
                        {
                            if (version[sourceField.Title].GetType().Name == "DateTime")
                            {
                                listItemTo[sourceField.Title] = Convert.ToDateTime(version[sourceField.Title]).ToLocalTime();
                            }
                            else
                            {
                                listItemTo[sourceField.Title] = version[sourceField.Title];
                            }
                        }
                        else
                        {
                            listItemTo[sourceField.Title] = version[sourceField.Title];
                        }
                    }
                    else if (sourceField.Title == "Кем создано" ||    //sourceField.Title == "Created By" ||     
                        sourceField.Title == "Автор изменений")       //sourceField.Title == "Modified By")
                    {
                        listItemTo[sourceField.Title] = version[sourceField.Title];
                    }
                    else if (sourceField.Title == "Создан" ||         //(sourceField.Title == "Created" ||         
                        sourceField.Title == "Изменен")               //sourceField.Title == "Modified" ||   
                    {
                        listItemTo[sourceField.Title] = Convert.ToDateTime(version[sourceField.Title]).ToLocalTime();
                    }
                }
                listItemTo.Update();
            }


            //и аттачменты
            foreach (string attachmentName in listItemFrom.Attachments)
            {
                SPFile file = listItemFrom.ParentList.ParentWeb.GetFile(listItemFrom.Attachments.UrlPrefix + attachmentName);
                listItemTo.Attachments.Add(attachmentName, file.OpenBinary());
            }
            listItemTo.Update();

            //и удаляем старый элемент.    
            listItemFrom.Delete();
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
