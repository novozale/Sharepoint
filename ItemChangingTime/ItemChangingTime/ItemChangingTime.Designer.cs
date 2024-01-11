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

namespace ItemTime
{
	public partial class ItemChangingTime
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
            this.ItemChangingTimeCode = new System.Workflow.Activities.CodeActivity();
            // 
            // ItemChangingTimeCode
            // 
            this.ItemChangingTimeCode.Name = "ItemChangingTimeCode";
            this.ItemChangingTimeCode.ExecuteCode += new System.EventHandler(this.ItemChangingTimeCode_ExecuteCode);
            // 
            // ItemChangingTime
            // 
            this.Activities.Add(this.ItemChangingTimeCode);
            this.Name = "ItemChangingTime";
            this.CanModifyActivities = false;

		}

		#endregion

        private CodeActivity ItemChangingTimeCode;

    }
}
