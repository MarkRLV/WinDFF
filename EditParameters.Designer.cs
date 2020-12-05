namespace WinDFF
{
    partial class EditParameters
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
            this.txtDrivesToScan = new System.Windows.Forms.TextBox();
            this.btnSaveParameters = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMinimumSize = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(185, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Drives To Scan:";
            // 
            // txtDrivesToScan
            // 
            this.txtDrivesToScan.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDrivesToScan.Location = new System.Drawing.Point(333, 64);
            this.txtDrivesToScan.Name = "txtDrivesToScan";
            this.txtDrivesToScan.Size = new System.Drawing.Size(192, 26);
            this.txtDrivesToScan.TabIndex = 2;
            // 
            // btnSaveParameters
            // 
            this.btnSaveParameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveParameters.Location = new System.Drawing.Point(225, 178);
            this.btnSaveParameters.Name = "btnSaveParameters";
            this.btnSaveParameters.Size = new System.Drawing.Size(256, 34);
            this.btnSaveParameters.TabIndex = 4;
            this.btnSaveParameters.Text = "Save Parameters";
            this.btnSaveParameters.UseVisualStyleBackColor = true;
            this.btnSaveParameters.Click += new System.EventHandler(this.btnSaveParameters_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(194, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Minimum Size:";
            // 
            // txtMinimumSize
            // 
            this.txtMinimumSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMinimumSize.Location = new System.Drawing.Point(333, 113);
            this.txtMinimumSize.Name = "txtMinimumSize";
            this.txtMinimumSize.Size = new System.Drawing.Size(192, 26);
            this.txtMinimumSize.TabIndex = 3;
            // 
            // EditParameters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtMinimumSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSaveParameters);
            this.Controls.Add(this.txtDrivesToScan);
            this.Controls.Add(this.label1);
            this.Name = "EditParameters";
            this.Text = "Edit Parameters";
            this.Load += new System.EventHandler(this.EditParameters_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDrivesToScan;
        private System.Windows.Forms.Button btnSaveParameters;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMinimumSize;
    }
}