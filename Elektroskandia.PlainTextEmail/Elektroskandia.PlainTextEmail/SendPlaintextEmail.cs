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
using System.Net.Mail;

namespace Elektroskandia.PlainTextEmail
{
	public partial class SendPlainTextEmail: SequenceActivity
	{
		public SendPlainTextEmail()
		{
			InitializeComponent();
        }

        #region Properties
        public static DependencyProperty __ContextProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(SendPlainTextEmail));

        [Description("Context")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowContext __Context
        {
            get
            {
                return ((WorkflowContext)(base.GetValue(SendPlainTextEmail.__ContextProperty)));
            }
            set
            {
                base.SetValue(SendPlainTextEmail.__ContextProperty, value);
            }
        }

        public static DependencyProperty NamesToProperty = DependencyProperty.Register("NamesTo", typeof(System.Collections.ArrayList), typeof(SendPlainTextEmail));
        [ValidationOption(ValidationOption.Required)]
        public System.Collections.ArrayList NamesTo
        {
            get
            {
                return (System.Collections.ArrayList)base.GetValue(NamesToProperty);
            }
            set
            {
                base.SetValue(NamesToProperty, value);
            }
        }

        public static DependencyProperty HeaderTextProperty = System.Workflow.ComponentModel.DependencyProperty.Register("HeaderText", typeof(string), typeof(SendPlainTextEmail));
        [Description("HeaderText")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string HeaderText
        {
            get
            {
                return ((string)(base.GetValue(SendPlainTextEmail.HeaderTextProperty)));
            }
            set
            {
                base.SetValue(SendPlainTextEmail.HeaderTextProperty, value);
            }
        }

        public static DependencyProperty BodyTextProperty = System.Workflow.ComponentModel.DependencyProperty.Register("BodyText", typeof(string), typeof(SendPlainTextEmail));
        [Description("BodyText")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string BodyText
        {
            get
            {
                return ((string)(base.GetValue(SendPlainTextEmail.BodyTextProperty)));
            }
            set
            {
                base.SetValue(SendPlainTextEmail.BodyTextProperty, value);
            }
        }

        #endregion

        private void SendPlainTextEmailCode_ExecuteCode(object sender, EventArgs e)
        ///////////////////////////////////////////////////////////////////////
        //
        // Основная процедура
        //
        ///////////////////////////////////////////////////////////////////////
        {
            SPSecurity.RunWithElevatedPrivileges(delegate() //запуск с повышенными привилегиями (system account)
                //для того, чтобы это работало, необходимо чтобы списку (элементу)
                //для system account были назначены полные права (чтение и запись)
            {
                SendMyReminder(NamesTo, HeaderText, BodyText);
            });
        }

        private void SendMyReminder(System.Collections.ArrayList NamesTo, string HeaderText, string BodyText)
        ///////////////////////////////////////////////////////////////////////
        //
        // Отправка письма с заданными параметрами
        //
        ///////////////////////////////////////////////////////////////////////
        {
            string MyAddr;
            string MyLogin;
            SmtpClient smtp = new SmtpClient("spbvrt6");
            MailMessage msg = new MailMessage();

            for (int k = 0; k < NamesTo.Count; k++)
            {
                MyLogin = NamesTo[k].ToString();
                if (ItIsEmail(MyLogin) == true)
                {
                    msg.To.Add(MyLogin);
                } else {
                    MyLogin = MyLogin.Substring(MyLogin.LastIndexOf("\\") + 1);
                    MyAddr = GetEmailByLogin(MyLogin);
                    if (MyAddr != "")
                    {
                        msg.To.Add(MyAddr);
                    }
                }
            }

            if (msg.To.Count > 0)
            {
                msg.From = new MailAddress("reportserver@skandikagroup.ru");

                if (!String.IsNullOrEmpty(HeaderText))
                {
                    msg.Subject = HeaderText;
                }

                if (!String.IsNullOrEmpty(BodyText))
                {
                    msg.Body = BodyText;
                }

                
                smtp.Send(msg);
            }
        }

        private string GetEmailByLogin(string MyLogin)
        ///////////////////////////////////////////////////////////////////////
        //
        // Получение Email по логину в АД (если это логин)
        //
        ///////////////////////////////////////////////////////////////////////
        {
            try
            {
                DirectoryEntry entry = new DirectoryEntry();
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(&(objectClass=user)(anr=" + MyLogin + "))";
                search.PropertiesToLoad.Add("mail");
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    try
                    {
                        return result.Properties["mail"][0].ToString();
                    }
                    catch
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        private Boolean ItIsEmail(string MyLogin)
        ///////////////////////////////////////////////////////////////////////
        //
        // Проверка - это Email или нет
        //
        ///////////////////////////////////////////////////////////////////////
        {
            try
            {
                System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(MyLogin);
                return true;
            }
            catch
            {
                return false;
            }
        }
	}
}
