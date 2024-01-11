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

namespace ItemLevelSecurity
{
	public partial class ItemLevelSecurityActivity
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
            this.ItemLevelSecurityCodeActivity = new System.Workflow.Activities.CodeActivity();
            // 
            // ItemLevelSecurityCodeActivity
            // 
            this.ItemLevelSecurityCodeActivity.Name = "ItemLevelSecurityCodeActivity";
            this.ItemLevelSecurityCodeActivity.ExecuteCode += new System.EventHandler(this.ItemLevelSecurityCodeActivity_ExecuteCode);
            // 
            // ItemLevelSecurityActivity
            // 
            this.Activities.Add(this.ItemLevelSecurityCodeActivity);
            this.Name = "ItemLevelSecurityActivity";
            this.CanModifyActivities = false;

		}

		#endregion

        private CodeActivity ItemLevelSecurityCodeActivity;

    }
}
