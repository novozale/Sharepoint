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

namespace Elektroskandia.ItemSetInDB
{
    public partial class ItemSetInDB : SequenceActivity
	{
		#region Activity Designer generated code

        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            this.ItemSetInDBCode = new System.Workflow.Activities.CodeActivity();
            // 
            // ItemSetInDBCode
            // 
            this.ItemSetInDBCode.Name = "ItemSetInDBCode";
            this.ItemSetInDBCode.ExecuteCode += new System.EventHandler(this.ItemSetInDB_ExecuteCode);
            // 
            // ItemSetInDB
            // 
            this.Activities.Add(this.ItemSetInDBCode);
            this.Name = "ItemSetInDB";
            this.CanModifyActivities = false;

        }

        #endregion

        private CodeActivity ItemSetInDBCode;
    }
}
