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

namespace Elektroskandia.MWorkflowStarter
{
	public partial class MWorkflowStarterCLS
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
            this.MWorkflowStarterCode = new System.Workflow.Activities.CodeActivity();
            // 
            // MWorkflowStarterCode
            // 
            this.MWorkflowStarterCode.Name = "MWorkflowStarterCode";
            this.MWorkflowStarterCode.ExecuteCode += new System.EventHandler(this.MWSCode);
            // 
            // MWorkflowStarterCLS
            // 
            this.Activities.Add(this.MWorkflowStarterCode);
            this.Name = "MWorkflowStarterCLS";
            this.CanModifyActivities = false;

		}

		#endregion

        private CodeActivity MWorkflowStarterCode;

    }
}
