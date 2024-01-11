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

namespace ItemChangingFromDataBase
{
	public partial class ItemChangingFromDB: SequenceActivity
	{
		public ItemChangingFromDB()
		{
			InitializeComponent();
        }

        #region Properties
        public static DependencyProperty MyDataProperty = DependencyProperty.Register("MyData", typeof(string), typeof(ItemChangingFromDB));
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

        public static DependencyProperty MyParamFieldNameProperty = DependencyProperty.Register("MyParamFieldName", typeof(string), typeof(ItemChangingFromDB));
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

        public static DependencyProperty MyFieldNameProperty = DependencyProperty.Register("MyFieldName", typeof(string), typeof(ItemChangingFromDB));
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

        public static DependencyProperty __ContextProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__Context", typeof(WorkflowContext), typeof(ItemChangingFromDB));

        [Description("Context")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public WorkflowContext __Context
        {
            get
            {
                return ((WorkflowContext)(base.GetValue(ItemChangingFromDB.__ContextProperty)));
            }
            set
            {
                base.SetValue(ItemChangingFromDB.__ContextProperty, value);
            }
        }

        public static DependencyProperty __ListIdProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__ListId", typeof(string), typeof(ItemChangingFromDB));
        [Description("__ListId")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string __ListId
        {
            get
            {
                return ((string)(base.GetValue(ItemChangingFromDB.__ListIdProperty)));
            }
            set
            {
                base.SetValue(ItemChangingFromDB.__ListIdProperty, value);
            }
        }

        public static DependencyProperty __ListItemProperty = System.Workflow.ComponentModel.DependencyProperty.Register("__ListItem", typeof(int), typeof(ItemChangingFromDB));
        [Description("__ListItem")]
        [ValidationOption(ValidationOption.Required)]
        [BrowsableAttribute(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int __ListItem
        {
            get
            {
                return ((int)(base.GetValue(ItemChangingFromDB.__ListItemProperty)));
            }
            set
            {
                base.SetValue(ItemChangingFromDB.__ListItemProperty, value);
            }
        }
        #endregion

        private void ItemChangingFromDBCode_ExecuteCode(object sender, EventArgs e)
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
                            if (string.Compare(this.MyData.ToString(), "Supplier Type") == 0)
                            //для поля устанавливаеми тип поставщика
                            {
                                listItem[this.MyFieldName.ToString()] = GetSupplierType(listItem[this.MyParamFieldName.ToString()].ToString());
                                listItem.Update();
                            }
                            if (string.Compare(this.MyData.ToString(), "Supplier Summ") == 0)
                            //для поля устанавливаеми весь объем закупок у поставщика
                            {
                                listItem[this.MyFieldName.ToString()] = GetSupplierSumm(listItem[this.MyParamFieldName.ToString()].ToString());
                                listItem.Update();
                            }
                            if (string.Compare(this.MyData.ToString(), "Supplier Summ Dig") == 0)
                            //для поля устанавливаеми весь объем закупок у поставщика
                            {
                                listItem[this.MyFieldName.ToString()] = GetSupplierSummDig(listItem[this.MyParamFieldName.ToString()].ToString());
                                listItem.Update();
                            }
                            if (string.Compare(this.MyData.ToString(), "Is Supplier Blocked") == 0)
                            //для поля устанавливаеми весь объем закупок у поставщика
                            {
                                listItem[this.MyFieldName.ToString()] = GetIsSupplierBlocked(listItem[this.MyParamFieldName.ToString()].ToString());
                                listItem.Update();
                            }
                            if (string.Compare(this.MyData.ToString(), "Does have a price") == 0)
                            //Для поля устанавливаем 'Нет' - нет прайс листа 'Да' - есть прайс лист
                            {
                                listItem[this.MyFieldName.ToString()] = GetDoesHaveAPrice(listItem[this.MyParamFieldName.ToString()].ToString());
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

        //Получение из БД информации о типе поставщика
        private static string GetSupplierType(string MySuppID)
        {
            string MySQLStr;
            string MyInfo;

            MyInfo = "В Scala не найдено";

            string connStr = "Data Source=SQLCLS;" +
                            "Initial Catalog=ScaDataDB;" +
                            "User id=sa;" +
                            "Password=sqladmin;";
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch
            {
            }
            MySQLStr = "SELECT CASE WHEN OnceOnly = 0 THEN '' ELSE 'Разовый поставщик' END AS OnceOnly ";
            MySQLStr = MySQLStr + "FROM tbl_SupplierCard0300 ";
            MySQLStr = MySQLStr + "WHERE (Ltrim(Rtrim(PL01001)) = N'" + MySuppID + "') ";

            try
            {
                SqlCommand cmd = new SqlCommand(MySQLStr, conn);

                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        MyInfo = dr.GetValue(0).ToString().Trim();
                    }
                }
            }
            catch
            {
            }
            conn.Close();
            conn.Dispose();
            return MyInfo;
        }

        //Получение из БД информации о сумме закупок у поставщика вывод в строку
        private static string GetSupplierSumm(string MySuppID)
        {
            string MySQLStr;
            string MyInfo;

            MyInfo = "В Scala не найдено";

            string connStr = "Data Source=SQLCLS;" +
                            "Initial Catalog=ScaDataDB;" +
                            "User id=sa;" +
                            "Password=sqladmin;";
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch
            {
            }
            MySQLStr = "SELECT ROUND(ISNULL(SUM(PC030300.PC03008 / PC030300.PC03019 * CASE WHEN PC030300.PC03010 = 0 THEN PC030300.PC03012 ELSE PC030300.PC03010 ";
            MySQLStr = MySQLStr + "END * CASE WHEN PC010300.PC01031 = 0 THEN 1 ELSE PC010300.PC01031 END), 0), 2) AS CC ";
            MySQLStr = MySQLStr + "FROM PC010300 WITH (NOLOCK) INNER JOIN ";
            MySQLStr = MySQLStr + "PC030300 ON PC010300.PC01001 = PC030300.PC03001 ";
            MySQLStr = MySQLStr + "WHERE (Ltrim(Rtrim(PC010300.PC01003)) = N'" + MySuppID + "') AND (PC030300.PC03002 <> N'0') ";
            MySQLStr = MySQLStr + "AND (dbo.PC010300.PC01015 > CONVERT(DATETIME, '" + "01/01/" + DateTime.Now.Year.ToString() + "', 103)) ";
            try
            {
                SqlCommand cmd = new SqlCommand(MySQLStr, conn);

                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        MyInfo = String.Format("{0:N2}", dr.GetValue(0));
                    }
                }
            }
            catch
            {
            }
            conn.Close();
            conn.Dispose();
            return MyInfo;
        }

        //Получение из БД информации о сумме закупок у поставщика вывод в число
        private static Double GetSupplierSummDig(string MySuppID)
        {
            string MySQLStr;
            Double MyInfo;

            MyInfo = 0;

            string connStr = "Data Source=SQLCLS;" +
                            "Initial Catalog=ScaDataDB;" +
                            "User id=sa;" +
                            "Password=sqladmin;";
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch
            {
            }
            MySQLStr = "SELECT ROUND(ISNULL(SUM(PC030300.PC03008 / PC030300.PC03019 * CASE WHEN PC030300.PC03010 = 0 THEN PC030300.PC03012 ELSE PC030300.PC03010 ";
            MySQLStr = MySQLStr + "END * CASE WHEN PC010300.PC01031 = 0 THEN 1 ELSE PC010300.PC01031 END), 0), 2) AS CC ";
            MySQLStr = MySQLStr + "FROM PC010300 WITH (NOLOCK) INNER JOIN ";
            MySQLStr = MySQLStr + "PC030300 ON PC010300.PC01001 = PC030300.PC03001 ";
            MySQLStr = MySQLStr + "WHERE (Ltrim(Rtrim(PC010300.PC01003)) = N'" + MySuppID + "') AND (PC030300.PC03002 <> N'0') ";
            MySQLStr = MySQLStr + "AND (dbo.PC010300.PC01015 > CONVERT(DATETIME, '" + "01/01/" + DateTime.Now.Year.ToString() + "', 103)) ";
            try
            {
                SqlCommand cmd = new SqlCommand(MySQLStr, conn);

                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        MyInfo = (Double)dr.GetDecimal(0);
                    }
                }
            }
            catch (Exception e)
            {
                String myerr = e.Message;
            }
            conn.Close();
            conn.Dispose();
            return MyInfo;
        }

        //получение из БД тнформации - заблокирован поставщик или нет
        private static string GetIsSupplierBlocked(string MySuppID)
        {
            string MySQLStr;
            string MyInfo;

            MyInfo = "Да";

            string connStr = "Data Source=SQLCLS;" +
                            "Initial Catalog=ScaDataDB;" +
                            "User id=sa;" +
                            "Password=sqladmin;";
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch
            {
            }
            MySQLStr = "SELECT CASE WHEN IsBlocked = 0 THEN 'Нет' ELSE 'Да' END AS IsBlocked ";
            MySQLStr = MySQLStr + "FROM tbl_SupplierCard0300 ";
            MySQLStr = MySQLStr + "WHERE (PL01001 = N'" + MySuppID + "')";

            try
            {
                SqlCommand cmd = new SqlCommand(MySQLStr, conn);

                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        MyInfo = dr.GetValue(0).ToString().Trim();
                    }
                }
            }
            catch
            {
                MyInfo = "Да";
            }
            conn.Close();
            conn.Dispose();
            return MyInfo;

        }

        //Получение из БД информации - есть у поставщика прайс лист или нет
        private static string GetDoesHaveAPrice(string MySuppID)
        {
            string MySQLStr;
            string MyInfo;

            MyInfo = "Да";

            string connStr = "Data Source=SQLCLS;" +
                            "Initial Catalog=ScaDataDB;" +
                            "User id=sa;" +
                            "Password=sqladmin;";
            SqlConnection conn = new SqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch
            {
            }
            MySQLStr = "SELECT CASE WHEN COUNT(ID) > 0 THEN 'Да' ELSE 'Нет' END AS CC ";
            MySQLStr = MySQLStr + "FROM  tbl_PurchasePriceHistory ";
            MySQLStr = MySQLStr + "WHERE (Ltrim(Rtrim(PL01001)) = N'" + MySuppID + "') AND (DateTo = CONVERT(DATETIME, '9999-12-31 00:00:00', 102)) ";

            try
            {
                SqlCommand cmd = new SqlCommand(MySQLStr, conn);

                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dr.Read())
                    {
                        MyInfo = dr.GetValue(0).ToString().Trim();
                    }
                }
            }
            catch
            {
                MyInfo = "Да";
            }
            conn.Close();
            conn.Dispose();
            return MyInfo;
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
