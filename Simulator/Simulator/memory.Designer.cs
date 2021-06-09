
namespace Simulator
{
    partial class memory
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
            this.memDGV = new System.Windows.Forms.DataGridView();
            this.memAddr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.memDGV)).BeginInit();
            this.SuspendLayout();
            // 
            // memDGV
            // 
            this.memDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.memDGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.memAddr,
            this.value});
            this.memDGV.Location = new System.Drawing.Point(4, 8);
            this.memDGV.Name = "memDGV";
            this.memDGV.Size = new System.Drawing.Size(212, 641);
            this.memDGV.TabIndex = 0;
            // 
            // memAddr
            // 
            this.memAddr.HeaderText = "MemoryAddress";
            this.memAddr.Name = "memAddr";
            // 
            // value
            // 
            this.value.HeaderText = "Value";
            this.value.Name = "value";
            // 
            // memory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(223, 661);
            this.Controls.Add(this.memDGV);
            this.Name = "memory";
            this.Text = "memory";
            ((System.ComponentModel.ISupportInitialize)(this.memDGV)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView memDGV;
        private System.Windows.Forms.DataGridViewTextBoxColumn memAddr;
        private System.Windows.Forms.DataGridViewTextBoxColumn value;
    }
}