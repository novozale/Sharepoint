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

namespace Elektroskandia.MovingItemsByDate
{
	public partial class MovingItemsByDateCLS
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
            this.MovingItemsByDateCode = new System.Workflow.Activities.CodeActivity();
            // 
            // MovingItemsByDateCode
            // 
            this.MovingItemsByDateCode.Name = "MovingItemsByDateCode";
            this.MovingItemsByDateCode.ExecuteCode += new System.EventHandler(this.MovingItemsByDateCode_ExecuteCode);
            // 
            // MovingItemsByDateCLS
            // 
            this.Activities.Add(this.MovingItemsByDateCode);
            this.Name = "MovingItemsByDateCLS";
            this.CanModifyActivities = false;

		}

		#endregion

        private CodeActivity MovingItemsByDateCode;

    }
}
