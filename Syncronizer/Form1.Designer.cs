﻿using System;

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
            this.AddNode = new System.Windows.Forms.Button();
            this.AddPath = new System.Windows.Forms.Button();
            this.Copy = new System.Windows.Forms.Button();
            this.Log = new System.Windows.Forms.ListBox();
            this.clearLog = new System.Windows.Forms.Button();
            this.filewrite = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // AddNode
            // 
            this.AddNode.Location = new System.Drawing.Point(181, 47);
            this.AddNode.Name = "AddNode";
            this.AddNode.Size = new System.Drawing.Size(75, 23);
            this.AddNode.TabIndex = 0;
            this.AddNode.Text = "Add node";
            this.AddNode.UseVisualStyleBackColor = true;
            this.AddNode.Click += new System.EventHandler(this.AddNode_Click);
            // 
            // AddPath
            // 
            this.AddPath.Location = new System.Drawing.Point(324, 47);
            this.AddPath.Name = "AddPath";
            this.AddPath.Size = new System.Drawing.Size(75, 23);
            this.AddPath.TabIndex = 1;
            this.AddPath.Text = "Add path";
            this.AddPath.UseVisualStyleBackColor = true;
            this.AddPath.Click += new System.EventHandler(this.AddPath_Click);
            // 
            // Copy
            // 
            this.Copy.Location = new System.Drawing.Point(483, 47);
            this.Copy.Name = "Copy";
            this.Copy.Size = new System.Drawing.Size(75, 23);
            this.Copy.TabIndex = 2;
            this.Copy.Text = "Copy";
            this.Copy.UseVisualStyleBackColor = true;
            this.Copy.Click += new System.EventHandler(this.Copy_Click);
            // 
            // Log
            // 
            this.Log.Cursor = System.Windows.Forms.Cursors.Default;
            this.Log.FormattingEnabled = true;
            this.Log.Location = new System.Drawing.Point(63, 185);
            this.Log.Name = "Log";
            this.Log.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Log.Size = new System.Drawing.Size(648, 95);
            this.Log.TabIndex = 4;
            // 
            // clearLog
            // 
            this.clearLog.Location = new System.Drawing.Point(63, 287);
            this.clearLog.Name = "clearLog";
            this.clearLog.Size = new System.Drawing.Size(75, 23);
            this.clearLog.TabIndex = 5;
            this.clearLog.Text = "Clear";
            this.clearLog.UseVisualStyleBackColor = true;
            this.clearLog.Click += new System.EventHandler(this.ClearLog_Click);
            // 
            // filewrite
            // 
            this.filewrite.Location = new System.Drawing.Point(350, 370);
            this.filewrite.Name = "filewrite";
            this.filewrite.Size = new System.Drawing.Size(75, 23);
            this.filewrite.TabIndex = 6;
            this.filewrite.Text = "filewrite";
            this.filewrite.UseVisualStyleBackColor = true;
            this.filewrite.Click += new System.EventHandler(this.Filewrite_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.filewrite);
            this.Controls.Add(this.clearLog);
            this.Controls.Add(this.Log);
            this.Controls.Add(this.Copy);
            this.Controls.Add(this.AddPath);
            this.Controls.Add(this.AddNode);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.Button AddNode;
        private System.Windows.Forms.Button AddPath;
        private System.Windows.Forms.Button Copy;
        private System.Windows.Forms.ListBox Log;
        private System.Windows.Forms.Button clearLog;
        private System.Windows.Forms.Button filewrite;
    }
}

