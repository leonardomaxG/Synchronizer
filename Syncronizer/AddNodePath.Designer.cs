namespace Syncronizer
{
    partial class AddNodePath
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
            this.NodeToAdd = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Confirm = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.browseFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.Browse = new System.Windows.Forms.Button();
            this.PathToAdd = new System.Windows.Forms.TextBox();
            this.canReceive = new System.Windows.Forms.CheckBox();
            this.canSend = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // NodeToAdd
            // 
            this.NodeToAdd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NodeToAdd.FormattingEnabled = true;
            this.NodeToAdd.Location = new System.Drawing.Point(100, 12);
            this.NodeToAdd.Name = "NodeToAdd";
            this.NodeToAdd.Size = new System.Drawing.Size(357, 21);
            this.NodeToAdd.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select node";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Add path";
            // 
            // Confirm
            // 
            this.Confirm.Location = new System.Drawing.Point(292, 74);
            this.Confirm.Name = "Confirm";
            this.Confirm.Size = new System.Drawing.Size(75, 23);
            this.Confirm.TabIndex = 4;
            this.Confirm.Text = "Confirm";
            this.Confirm.UseVisualStyleBackColor = true;
            this.Confirm.Click += new System.EventHandler(this.Confirm_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(382, 74);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 5;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Browse
            // 
            this.Browse.Location = new System.Drawing.Point(382, 43);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(75, 20);
            this.Browse.TabIndex = 6;
            this.Browse.Text = "Browse...";
            this.Browse.UseVisualStyleBackColor = true;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // PathToAdd
            // 
            this.PathToAdd.Location = new System.Drawing.Point(100, 43);
            this.PathToAdd.Name = "PathToAdd";
            this.PathToAdd.Size = new System.Drawing.Size(276, 20);
            this.PathToAdd.TabIndex = 7;
            // 
            // canReceive
            // 
            this.canReceive.AutoSize = true;
            this.canReceive.Checked = true;
            this.canReceive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.canReceive.Location = new System.Drawing.Point(100, 77);
            this.canReceive.Name = "canReceive";
            this.canReceive.Size = new System.Drawing.Size(83, 17);
            this.canReceive.TabIndex = 8;
            this.canReceive.Text = "Can receive";
            this.canReceive.UseVisualStyleBackColor = true;
            // 
            // canSend
            // 
            this.canSend.AutoSize = true;
            this.canSend.Checked = true;
            this.canSend.CheckState = System.Windows.Forms.CheckState.Checked;
            this.canSend.Location = new System.Drawing.Point(189, 77);
            this.canSend.Name = "canSend";
            this.canSend.Size = new System.Drawing.Size(71, 17);
            this.canSend.TabIndex = 9;
            this.canSend.Text = "Can send";
            this.canSend.UseVisualStyleBackColor = true;
            // 
            // AddNodePath
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 106);
            this.Controls.Add(this.canSend);
            this.Controls.Add(this.canReceive);
            this.Controls.Add(this.PathToAdd);
            this.Controls.Add(this.Browse);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Confirm);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NodeToAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AddNodePath";
            this.Text = "AddNodePath";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox NodeToAdd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Confirm;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.FolderBrowserDialog browseFolder;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.TextBox PathToAdd;
        private System.Windows.Forms.CheckBox canReceive;
        private System.Windows.Forms.CheckBox canSend;
    }
}