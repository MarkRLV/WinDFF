using System;

namespace WinDFF
{
    partial class WinDFF
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WinDFF));
            this.labelFileList = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButtonFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileScanShell = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButtonEdit = new System.Windows.Forms.ToolStripDropDownButton();
            this.editScanParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButtonView = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripDropDownButtonHelp = new System.Windows.Forms.ToolStripDropDownButton();
            this.generalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.ADGView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripViewDriveInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ADGView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelFileList
            // 
            this.labelFileList.AutoSize = true;
            this.labelFileList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFileList.Location = new System.Drawing.Point(66, 45);
            this.labelFileList.Name = "labelFileList";
            this.labelFileList.Size = new System.Drawing.Size(170, 20);
            this.labelFileList.TabIndex = 0;
            this.labelFileList.Text = "Current File List (none)";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButtonFile,
            this.toolStripDropDownButtonEdit,
            this.toolStripDropDownButtonView,
            this.toolStripDropDownButtonHelp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(994, 28);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButtonFile
            // 
            this.toolStripDropDownButtonFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripFileOpen,
            this.toolStripFileScanShell,
            this.toolStripFileExit});
            this.toolStripDropDownButtonFile.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonFile.Image")));
            this.toolStripDropDownButtonFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonFile.Name = "toolStripDropDownButtonFile";
            this.toolStripDropDownButtonFile.Size = new System.Drawing.Size(47, 25);
            this.toolStripDropDownButtonFile.Text = "File";
            // 
            // toolStripFileOpen
            // 
            this.toolStripFileOpen.Name = "toolStripFileOpen";
            this.toolStripFileOpen.Size = new System.Drawing.Size(180, 26);
            this.toolStripFileOpen.Text = "Open";
            this.toolStripFileOpen.Click += new System.EventHandler(this.toolStripFileOpen_Click);
            // 
            // toolStripFileScanShell
            // 
            this.toolStripFileScanShell.Name = "toolStripFileScanShell";
            this.toolStripFileScanShell.Size = new System.Drawing.Size(180, 26);
            this.toolStripFileScanShell.Text = "Scan";
            this.toolStripFileScanShell.Click += new System.EventHandler(this.toolStripFileScanShell_Click);
            // 
            // toolStripFileExit
            // 
            this.toolStripFileExit.Name = "toolStripFileExit";
            this.toolStripFileExit.Size = new System.Drawing.Size(180, 26);
            this.toolStripFileExit.Text = "Exit";
            this.toolStripFileExit.Click += new System.EventHandler(this.toolStripFileExit_Click);
            // 
            // toolStripDropDownButtonEdit
            // 
            this.toolStripDropDownButtonEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editScanParametersToolStripMenuItem});
            this.toolStripDropDownButtonEdit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonEdit.Image")));
            this.toolStripDropDownButtonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonEdit.Name = "toolStripDropDownButtonEdit";
            this.toolStripDropDownButtonEdit.Size = new System.Drawing.Size(49, 25);
            this.toolStripDropDownButtonEdit.Text = "Edit";
            // 
            // editScanParametersToolStripMenuItem
            // 
            this.editScanParametersToolStripMenuItem.Name = "editScanParametersToolStripMenuItem";
            this.editScanParametersToolStripMenuItem.Size = new System.Drawing.Size(195, 26);
            this.editScanParametersToolStripMenuItem.Text = "Scan Parameters";
            this.editScanParametersToolStripMenuItem.Click += new System.EventHandler(this.editScanParametersToolStripMenuItem_Click);
            // 
            // toolStripDropDownButtonView
            // 
            this.toolStripDropDownButtonView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripViewDriveInfo});
            this.toolStripDropDownButtonView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonView.Image")));
            this.toolStripDropDownButtonView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonView.Name = "toolStripDropDownButtonView";
            this.toolStripDropDownButtonView.Size = new System.Drawing.Size(57, 25);
            this.toolStripDropDownButtonView.Text = "View";
            // 
            // toolStripDropDownButtonHelp
            // 
            this.toolStripDropDownButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generalToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.toolStripDropDownButtonHelp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButtonHelp.Image")));
            this.toolStripDropDownButtonHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonHelp.Name = "toolStripDropDownButtonHelp";
            this.toolStripDropDownButtonHelp.Size = new System.Drawing.Size(55, 25);
            this.toolStripDropDownButtonHelp.Text = "Help";
            // 
            // generalToolStripMenuItem
            // 
            this.generalToolStripMenuItem.Name = "generalToolStripMenuItem";
            this.generalToolStripMenuItem.Size = new System.Drawing.Size(180, 26);
            this.generalToolStripMenuItem.Text = "General";
            this.generalToolStripMenuItem.Click += new System.EventHandler(this.generalToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(180, 26);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // ADGView1
            // 
            this.ADGView1.AllowUserToAddRows = false;
            this.ADGView1.AllowUserToDeleteRows = false;
            this.ADGView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ADGView1.Location = new System.Drawing.Point(21, 72);
            this.ADGView1.MaximumSize = new System.Drawing.Size(1200, 600);
            this.ADGView1.MinimumSize = new System.Drawing.Size(1200, 600);
            this.ADGView1.Name = "ADGView1";
            this.ADGView1.ReadOnly = true;
            this.ADGView1.Size = new System.Drawing.Size(1200, 600);
            this.ADGView1.TabIndex = 6;
            this.ADGView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ADGView1_CellMouseDown);
            this.ADGView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ADGView1_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(108, 48);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.viewToolStripMenuItem.Text = "View";
            this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripViewDriveInfo
            // 
            this.toolStripViewDriveInfo.Name = "toolStripViewDriveInfo";
            this.toolStripViewDriveInfo.Size = new System.Drawing.Size(180, 26);
            this.toolStripViewDriveInfo.Text = "Drive Info";
            this.toolStripViewDriveInfo.Click += new System.EventHandler(this.toolStripViewDriveInfo_Click);
            // 
            // WinDFF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(994, 493);
            this.Controls.Add(this.ADGView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.labelFileList);
            this.Name = "WinDFF";
            this.Text = "WinDFF";
            this.Load += new System.EventHandler(this.WinDFF_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ADGView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ADGView1_CellMouseDown(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.Label labelFileList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonFile;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileOpen;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileExit;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonView;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.DataGridView ADGView1;
        private System.Windows.Forms.ToolStripMenuItem toolStripFileScanShell;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonEdit;
        private System.Windows.Forms.ToolStripMenuItem editScanParametersToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonHelp;
        private System.Windows.Forms.ToolStripMenuItem generalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripViewDriveInfo;
    }
}

