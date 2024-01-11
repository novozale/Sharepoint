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

namespace ItemCopy
{
	public partial class ItemFullCopy: SequenceActivity
	{
		public ItemFullCopy()
		{
			InitializeComponent();
		}

        #region Properties

        public static DependencyProperty __ContextProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(ItemFullCopy));

        [Description("Context")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowContext __Context
        {
            get
            {
                return ((WorkflowContext)(base.GetValue(ItemFullCopy.__ContextProperty)));
            }
            set
            {
                base.SetValue(ItemFullCopy.__ContextProperty, value);
            }
        }

        public static DependencyProperty ListIdProperty = System.Workflow.ComponentModel.DependencyProperty.Register("ListId", typeof(string), typeof(ItemFullCopy));
        [Description("ListId")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ListId
        {
            get
            {
                return ((string)(base.GetValue(ItemFullCopy.ListIdProperty)));
            }
            set
            {
                base.SetValue(ItemFullCopy.ListIdProperty, value);
            }
        }

        public static DependencyProperty ListItemProperty = System.Workflow.ComponentModel.DependencyProperty.Register("ListItem", typeof(int), typeof(ItemFullCopy));
        [Description("ListItem")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ListItem
        {
            get
            {
                return ((int)(base.GetValue(ItemFullCopy.ListItemProperty)));
            }
            set
            {
                base.SetValue(ItemFullCopy.ListItemProperty, value);
            }
        }

        public static DependencyProperty ToListIdProperty = System.Workflow.ComponentModel.DependencyProperty.Register("ToListId", typeof(string), typeof(ItemFullCopy));
        [Description("ToListId")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ToListId
        {
            get
            {
                return ((string)(base.GetValue(ItemFullCopy.ToListIdProperty)));
            }
            set
            {
                base.SetValue(ItemFullCopy.ToListIdProperty, value);
            }
        }

        #endregion

        private void ItemFullCopyCode_ExecuteCode(object sender, EventArgs e)
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
                            SPList ListFrom = spVeb.Lists[new Guid(this.ListId)];         //список откуда копируем
                            SPListItem listItemFrom = ListFrom.GetItemById(this.ListItem);//элемент который копируем
                            SPList ListTo = spVeb.Lists[new Guid(this.ToListId)];         //список куда копируем

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
