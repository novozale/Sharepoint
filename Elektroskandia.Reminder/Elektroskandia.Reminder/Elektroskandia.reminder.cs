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

using System.Net.Mail;

namespace Elektroskandia.Reminder
{
	public partial class ReminderCLS: SequenceActivity
	{
		public ReminderCLS()
		{
			InitializeComponent();
		}

        #region Properties

        public static DependencyProperty __ContextProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(ReminderCLS));

        [Description("Context")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowContext __Context
        {
            get
            {
                return ((WorkflowContext)(base.GetValue(ReminderCLS.__ContextProperty)));
            }
            set
            {
                base.SetValue(ReminderCLS.__ContextProperty, value);
            }
        }

        public static DependencyProperty BodyProperty = System.Workflow.ComponentModel.DependencyProperty.Register("Body", typeof(string), typeof(ReminderCLS));
        [Description("Body")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Body
        {
            get
            {
                return ((string)(base.GetValue(ReminderCLS.BodyProperty)));
            }
            set
            {
                base.SetValue(ReminderCLS.BodyProperty, value);
            }
        }

        public static DependencyProperty SubjectProperty = System.Workflow.ComponentModel.DependencyProperty.Register("Subject", typeof(string), typeof(ReminderCLS));
        [Description("Subject")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Subject
        {
            get
            {
                return ((string)(base.GetValue(ReminderCLS.SubjectProperty)));
            }
            set
            {
                base.SetValue(ReminderCLS.SubjectProperty, value);
            }
        }

        public static DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(System.Collections.ArrayList), typeof(ReminderCLS));
        [ValidationOption(ValidationOption.Required)]
        public System.Collections.ArrayList To
        {
            get
            {
                return (System.Collections.ArrayList)base.GetValue(ToProperty);
            }
            set
            {
                base.SetValue(ToProperty, value);
            }
        }

        public static DependencyProperty CCProperty = DependencyProperty.Register("CC", typeof(System.Collections.ArrayList), typeof(ReminderCLS));
        [ValidationOption(ValidationOption.Required)]
        public System.Collections.ArrayList CC
        {
            get
            {
                return (System.Collections.ArrayList)base.GetValue(CCProperty);
            }
            set
            {
                base.SetValue(CCProperty, value);
            }
        }

        public static DependencyProperty ListIdProperty = System.Workflow.ComponentModel.DependencyProperty.Register("ListId", typeof(string), typeof(ReminderCLS));
        [Description("ListId")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ListId
        {
            get
            {
                return ((string)(base.GetValue(ReminderCLS.ListIdProperty)));
            }
            set
            {
                base.SetValue(ReminderCLS.ListIdProperty, value);
            }
        }

        public static DependencyProperty MyValueProperty = System.Workflow.ComponentModel.DependencyProperty.Register("MyValue", typeof(System.Int32), typeof(ReminderCLS));
        [Description("MyValue")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public System.Int32 MyValue
        {
            get
            {
                return ((System.Int32)(base.GetValue(ReminderCLS.MyValueProperty)));
            }
            set
            {
                base.SetValue(ReminderCLS.MyValueProperty, value);
            }
        }

        public static DependencyProperty MyFieldNameProperty = System.Workflow.ComponentModel.DependencyProperty.Register("MyFieldName", typeof(string), typeof(ReminderCLS));
        [Description("MyFieldName")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string MyFieldName
        {
            get
            {
                return ((string)(base.GetValue(ReminderCLS.MyFieldNameProperty)));
            }
            set
            {
                base.SetValue(ReminderCLS.MyFieldNameProperty, value);
            }
        }

        public static DependencyProperty MyValue1Property = DependencyProperty.Register("MyValue1", typeof(System.Collections.ArrayList), typeof(ReminderCLS));
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

        private void ReminderCode_ExecuteCode(object sender, EventArgs e)
        {
            bool MyFlag;
            string MyWrkString;
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
                                    if (DateTime.Compare(DateTime.Today, Convert.ToDateTime(listItem["Изменен"]).AddDays((double)this.MyValue)) > 0)
                                    {
                                        MyWrkString = "";
                                        //тут проверим что значение поля не совпадает ни с одним из выбранных значений
                                        MyFlag = false;
                                        for (int j = 0; j < MyValue1.Count; j++)
                                        {
                                            if (string.Compare(listItem[MyFieldName.ToString()].ToString(), MyValue1[j].ToString()) == 0)
                                            {
                                                MyFlag = true;
                                            }
                                        }
                                        if (MyFlag == false)
                                        {
                                            MyWrkString = "С заявкой на создание '" + listItem["Название"].ToString() + "' Не производится никаких работ в течение " + MyValue.ToString() + " дней. Просьба принять меры к выполнению заявки. ";
                                            MyWrkString = MyWrkString + "\n" + Body.ToString();
                                            SendMyReminder(To, CC, Subject, MyWrkString);
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

        //Отправка письма с напоминанием о необходимости завести поставщика
        private void SendMyReminder(System.Collections.ArrayList To, System.Collections.ArrayList CC, string Subject, string MyWrkString)
        {
            // Set reference to Smtp Server.
            SmtpClient smtp = new SmtpClient("spbvrt6");

            MailMessage msg = new MailMessage();
            for (int k = 0; k < To.Count; k++)
            {
                msg.To.Add(To[k].ToString());
            }
            
            msg.From = new MailAddress("reportserver@elektroskandia.ru");

            if (CC != null)
            {
                for (int l = 0; l < CC.Count; l++)
                {
                    msg.CC.Add(CC[l].ToString());
                }
            }

            if (!String.IsNullOrEmpty(Subject))
            {
                msg.Subject = Subject;
            }

            if (!String.IsNullOrEmpty(MyWrkString))
            {
                msg.Body = MyWrkString;
            }

            smtp.Send(msg);
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
