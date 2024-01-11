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

namespace Elektroskandia.DeleteNotUsed
{
	public partial class DeleteNotUsedCLS
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
            this.DeleteNotUsedCode = new System.Workflow.Activities.CodeActivity();
            // 
            // DeleteNotUsedCode
            // 
            this.DeleteNotUsedCode.Name = "DeleteNotUsedCode";
            this.DeleteNotUsedCode.ExecuteCode += new System.EventHandler(this.DeleteNotUsedCode_ExecuteCode);
            // 
            // DeleteNotUsedCLS
            // 
            this.Activities.Add(this.DeleteNotUsedCode);
            this.Name = "DeleteNotUsedCLS";
            this.CanModifyActivities = false;

		}

		#endregion

        private CodeActivity DeleteNotUsedCode;





    }
}
