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

namespace ItemDelete
{
	public partial class ItemFullDelete
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
            this.ItemFullDeleteCode = new System.Workflow.Activities.CodeActivity();
            // 
            // ItemFullDeleteCode
            // 
            this.ItemFullDeleteCode.Name = "ItemFullDeleteCode";
            this.ItemFullDeleteCode.ExecuteCode += new System.EventHandler(this.ItemFullDeleteCode_ExecuteCode);
            // 
            // ItemFullDelete
            // 
            this.Activities.Add(this.ItemFullDeleteCode);
            this.Name = "ItemFullDelete";
            this.CanModifyActivities = false;

		}

		#endregion

        private CodeActivity ItemFullDeleteCode;

    }
}
