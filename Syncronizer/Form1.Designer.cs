using System;

namespace Syncronizer
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AddNetworkNode = new System.Windows.Forms.Button();
            this.AddNode = new System.Windows.Forms.Button();
            this.AddPath = new System.Windows.Forms.Button();
            this.Copy = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.Count = new System.Windows.Forms.Label();
            this.ConnectIp = new System.Windows.Forms.TextBox();
            this.Log = new System.Windows.Forms.ListBox();
            this.clearLog = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.AddNetworkNode);
            this.groupBox1.Controls.Add(this.AddNode);
            this.groupBox1.Controls.Add(this.AddPath);
            this.groupBox1.Controls.Add(this.Copy);
            this.groupBox1.Location = new System.Drawing.Point(245, 37);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 105);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // AddNetworkNode
            // 
            this.AddNetworkNode.Location = new System.Drawing.Point(6, 48);
            this.AddNetworkNode.Name = "AddNetworkNode";
            this.AddNetworkNode.Size = new System.Drawing.Size(75, 38);
            this.AddNetworkNode.TabIndex = 3;
            this.AddNetworkNode.Text = "Add network node";
            this.AddNetworkNode.UseVisualStyleBackColor = true;
            this.AddNetworkNode.Click += new System.EventHandler(this.AddNetworkNode_Click);
            // 
            // AddNode
            // 
            this.AddNode.Location = new System.Drawing.Point(6, 19);
            this.AddNode.Name = "AddNode";
            this.AddNode.Size = new System.Drawing.Size(75, 23);
            this.AddNode.TabIndex = 0;
            this.AddNode.Text = "Add node";
            this.AddNode.UseVisualStyleBackColor = true;
            this.AddNode.Click += new System.EventHandler(this.AddNode_Click);
            // 
            // AddPath
            // 
            this.AddPath.Location = new System.Drawing.Point(107, 19);
            this.AddPath.Name = "AddPath";
            this.AddPath.Size = new System.Drawing.Size(75, 23);
            this.AddPath.TabIndex = 1;
            this.AddPath.Text = "Add path";
            this.AddPath.UseVisualStyleBackColor = true;
            this.AddPath.Click += new System.EventHandler(this.AddPath_Click);
            // 
            // Copy
            // 
            this.Copy.Location = new System.Drawing.Point(107, 56);
            this.Copy.Name = "Copy";
            this.Copy.Size = new System.Drawing.Size(75, 23);
            this.Copy.TabIndex = 2;
            this.Copy.Text = "Copy";
            this.Copy.UseVisualStyleBackColor = true;
            this.Copy.Click += new System.EventHandler(this.Copy_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(464, 56);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(15, 401);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(679, 32);
            this.progressBar1.TabIndex = 9;
            // 
            // Count
            // 
            this.Count.AutoSize = true;
            this.Count.Location = new System.Drawing.Point(725, 411);
            this.Count.Name = "Count";
            this.Count.Size = new System.Drawing.Size(24, 13);
            this.Count.TabIndex = 10;
            this.Count.Text = "0/0";
            // 
            // ConnectIp
            // 
            this.ConnectIp.Location = new System.Drawing.Point(658, 37);
            this.ConnectIp.Name = "ConnectIp";
            this.ConnectIp.Size = new System.Drawing.Size(100, 20);
            this.ConnectIp.TabIndex = 11;
            // 
            // Log
            // 
            this.Log.Cursor = System.Windows.Forms.Cursors.Default;
            this.Log.FormattingEnabled = true;
            this.Log.HorizontalScrollbar = true;
            this.Log.Location = new System.Drawing.Point(15, 231);
            this.Log.Name = "Log";
            this.Log.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Log.Size = new System.Drawing.Size(716, 82);
            this.Log.TabIndex = 4;
            // 
            // clearLog
            // 
            this.clearLog.Location = new System.Drawing.Point(15, 319);
            this.clearLog.Name = "clearLog";
            this.clearLog.Size = new System.Drawing.Size(75, 23);
            this.clearLog.TabIndex = 5;
            this.clearLog.Text = "Clear";
            this.clearLog.UseVisualStyleBackColor = true;
            this.clearLog.Click += new System.EventHandler(this.ClearLog_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(770, 449);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.clearLog);
            this.Controls.Add(this.ConnectIp);
            this.Controls.Add(this.Log);
            this.Controls.Add(this.Count);
            this.Controls.Add(this.progressBar1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button AddNode;
        private System.Windows.Forms.Button AddPath;
        private System.Windows.Forms.Button Copy;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label Count;
        private System.Windows.Forms.Button AddNetworkNode;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox ConnectIp;
        private System.Windows.Forms.ListBox Log;
        private System.Windows.Forms.Button clearLog;
    }
}

