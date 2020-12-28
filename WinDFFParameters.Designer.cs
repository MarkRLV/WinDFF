namespace WinDFF
{
    partial class WinDFFParameters
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbSingle = new System.Windows.Forms.ComboBox();
            this.cbMultiple = new System.Windows.Forms.ComboBox();
            this.BtnSaveParameters = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(237, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(238, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Confirm Before Delete: (Single File): ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(237, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(247, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Confirm Before Delete: (Multiple File): ";
            // 
            // cbSingle
            // 
            this.cbSingle.FormattingEnabled = true;
            this.cbSingle.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cbSingle.Location = new System.Drawing.Point(522, 64);
            this.cbSingle.Name = "cbSingle";
            this.cbSingle.Size = new System.Drawing.Size(121, 24);
            this.cbSingle.TabIndex = 2;
            // 
            // cbMultiple
            // 
            this.cbMultiple.FormattingEnabled = true;
            this.cbMultiple.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.cbMultiple.Location = new System.Drawing.Point(522, 107);
            this.cbMultiple.Name = "cbMultiple";
            this.cbMultiple.Size = new System.Drawing.Size(121, 24);
            this.cbMultiple.TabIndex = 3;
            // 
            // BtnSaveParameters
            // 
            this.BtnSaveParameters.Location = new System.Drawing.Point(378, 187);
            this.BtnSaveParameters.Name = "BtnSaveParameters";
            this.BtnSaveParameters.Size = new System.Drawing.Size(211, 36);
            this.BtnSaveParameters.TabIndex = 4;
            this.BtnSaveParameters.Text = "Save Parameters";
            this.BtnSaveParameters.UseVisualStyleBackColor = true;
            this.BtnSaveParameters.Click += new System.EventHandler(this.BtnSaveParameters_Click);
            // 
            // WinDFFParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.BtnSaveParameters);
            this.Controls.Add(this.cbMultiple);
            this.Controls.Add(this.cbSingle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "WinDFFParameters";
            this.Text = "WinDFF Parameters";
            this.Load += new System.EventHandler(this.WinDFFParameters_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbSingle;
        private System.Windows.Forms.ComboBox cbMultiple;
        private System.Windows.Forms.Button BtnSaveParameters;
    }
}