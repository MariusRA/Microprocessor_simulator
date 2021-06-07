
namespace Simulator
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
            this.loadfileBTN = new System.Windows.Forms.Button();
            this.assembleBTN = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // loadfileBTN
            // 
            this.loadfileBTN.Location = new System.Drawing.Point(653, 96);
            this.loadfileBTN.Name = "loadfileBTN";
            this.loadfileBTN.Size = new System.Drawing.Size(174, 44);
            this.loadfileBTN.TabIndex = 0;
            this.loadfileBTN.Text = "Load File";
            this.loadfileBTN.UseVisualStyleBackColor = true;
            this.loadfileBTN.Click += new System.EventHandler(this.loadfileBTN_Click);
            // 
            // assembleBTN
            // 
            this.assembleBTN.Enabled = false;
            this.assembleBTN.Location = new System.Drawing.Point(653, 184);
            this.assembleBTN.Name = "assembleBTN";
            this.assembleBTN.Size = new System.Drawing.Size(177, 40);
            this.assembleBTN.TabIndex = 1;
            this.assembleBTN.Text = "Assemble";
            this.assembleBTN.UseVisualStyleBackColor = true;
            this.assembleBTN.Click += new System.EventHandler(this.assembleBTN_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 96);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(450, 500);
            this.textBox1.TabIndex = 2;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1123, 608);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.assembleBTN);
            this.Controls.Add(this.loadfileBTN);
            this.Name = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loadfileBTN;
        private System.Windows.Forms.Button assembleBTN;
        private System.Windows.Forms.TextBox textBox1;
    }
}

