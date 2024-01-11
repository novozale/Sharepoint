using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace Elektroskandia.Reminder
{
	public partial class ReminderCLS
	{
		#region Activity Designer generated code
		
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
		private void InitializeComponent()
		{
            this.CanModifyActivities = true;
            this.ReminderCode = new System.Workflow.Activities.CodeActivity();
            // 
            // ReminderCode
            // 
            this.ReminderCode.Name = "ReminderCode";
            this.ReminderCode.ExecuteCode += new System.EventHandler(this.ReminderCode_ExecuteCode);
            // 
            // ReminderCLS
            // 
            this.Activities.Add(this.ReminderCode);
            this.Name = "ReminderCLS";
            this.CanModifyActivities = false;

		}

		#endregion

        private CodeActivity ReminderCode;
	}
}
