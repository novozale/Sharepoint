﻿using System;
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

namespace Elektroskandia.PlainTextEmail
{
	public partial class SendPlainTextEmail
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
            this.SendPlainTextEmailCode = new System.Workflow.Activities.CodeActivity();
            // 
            // SendPlainTextEmailCode
            // 
            this.SendPlainTextEmailCode.Name = "SendPlainTextEmailCode";
            this.SendPlainTextEmailCode.ExecuteCode += new System.EventHandler(this.SendPlainTextEmailCode_ExecuteCode);
            // 
            // SendPlainTextEmail
            // 
            this.Activities.Add(this.SendPlainTextEmailCode);
            this.Name = "SendPlainTextEmail";
            this.CanModifyActivities = false;

		}

		#endregion

        private CodeActivity SendPlainTextEmailCode;

    }
}
