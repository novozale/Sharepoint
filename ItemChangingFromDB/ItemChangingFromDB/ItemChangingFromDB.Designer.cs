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

namespace ItemChangingFromDataBase
{
	public partial class ItemChangingFromDB
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
            this.ItemChangingFromDBCodeActivity = new System.Workflow.Activities.CodeActivity();
            // 
            // ItemChangingFromDBCodeActivity
            // 
            this.ItemChangingFromDBCodeActivity.Name = "ItemChangingFromDBCodeActivity";
            this.ItemChangingFromDBCodeActivity.ExecuteCode += new System.EventHandler(this.ItemChangingFromDBCode_ExecuteCode);
            // 
            // ItemChangingFromDB
            // 
            this.Activities.Add(this.ItemChangingFromDBCodeActivity);
            this.Name = "ItemChangingFromDB";
            this.CanModifyActivities = false;

		}

		#endregion

        private CodeActivity ItemChangingFromDBCodeActivity;

    }
}
