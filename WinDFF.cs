using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WinDFF
{
    public partial class WinDFF : Form
    {
        public WinDFF()
        {
            InitializeComponent();
        }

        private void toolStripFileOpen_Click(object sender, EventArgs e)
        {
            var header = string.Empty;
            var InputLine = string.Empty;
            var filePath = string.Empty;

            string FileDate = string.Empty;
            string FileSize = string.Empty;
            string FileHash = string.Empty;
            string FileName = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    labelFileList.Text = "FileList: " + Path.GetFileName(filePath);

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    DataTable SelectedFiles = new DataTable();
                    SelectedFiles.Columns.Add("File Size", typeof(string));
                    SelectedFiles.Columns.Add("File Date", typeof(string));
                    SelectedFiles.Columns.Add("File Name", typeof(string));
                    SelectedFiles.Columns.Add("File Hash", typeof(string));

                    string PriorHash = string.Empty;

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        header = reader.ReadLine();
                        
                        while (!reader.EndOfStream)
                        {
                            InputLine = reader.ReadLine();
                            int LeftBracketPosition = InputLine.IndexOf('[');
                            int RightBracketPosition = InputLine.IndexOf(']');
                            int MPosition = InputLine.IndexOf("M");

                            int DateLength = MPosition + 1;
                            int SizeLength = LeftBracketPosition - MPosition - 3;
                            int HashLength = RightBracketPosition - LeftBracketPosition - 1;
                            int NameLength = InputLine.Length - RightBracketPosition - 1;

                            FileDate = InputLine.Substring(0, DateLength).Trim();
                            FileSize = InputLine.Substring(MPosition + 1, SizeLength).Trim();
                            FileHash = InputLine.Substring(LeftBracketPosition + 1, HashLength).Trim();
                            FileName = InputLine.Substring(RightBracketPosition + 1, NameLength).Trim();

                            if (PriorHash != string.Empty && PriorHash != FileHash)
                            {
                                DataRow BlankRow = SelectedFiles.NewRow();
                                BlankRow["File Size"] = " ";
                                BlankRow["File Date"] = " ";
                                BlankRow["File Name"] = " ";
                                BlankRow["File Hash"] = " ";
                                SelectedFiles.Rows.Add(BlankRow);
                            }

                            DataRow NewRow = SelectedFiles.NewRow();
                            NewRow["File Size"] = FileSize;
                            NewRow["File Date"] = FileDate;
                            NewRow["File Name"] = FileName;
                            NewRow["File Hash"] = FileHash;
                            SelectedFiles.Rows.Add(NewRow);

                            PriorHash = FileHash;
                        }

                        BindingSource SBind = new BindingSource();
                        SBind.DataSource = SelectedFiles;

                        ADGView1.AutoGenerateColumns = true;  //must be "true" here
                        ADGView1.Columns.Clear();
                        ADGView1.DataSource = SBind;

                        for (int i = 0; i < ADGView1.Columns.Count; i++)
                        {
                            ADGView1.Columns[i].DataPropertyName = SelectedFiles.Columns[i].ColumnName;
                            ADGView1.Columns[i].HeaderText = SelectedFiles.Columns[i].Caption;
                        }

                        ADGView1.Columns["File Size"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        ADGView1.DefaultCellStyle.Font = new Font("Courier New", 10);

                        ADGView1.Enabled = true;
                        ADGView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                        ADGView1.Refresh();
                    }
                }
                else
                {
                    return;
                }
            }

        }

        private void WinDFF_Load(object sender, EventArgs e)
        {
           WindowState = FormWindowState.Maximized;
        }

        private void toolStripFileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripEditScanParameters_Click(object sender, EventArgs e)
        {
            string Message = string.Empty;

            using (StreamReader reader = new StreamReader("WinDFF.ini"))
            {
                while (!reader.EndOfStream)
                {
                    Message += reader.ReadLine();
                    Message += "\r";
                }
            }

            MessageBox.Show(Message, "Parameters", MessageBoxButtons.OK);
            return;
        }

        private void toolStripFileScanShell_Click(object sender, EventArgs e)
        {
            Process proc = null;
            try
            {
                proc = new Process();
                proc.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                proc.StartInfo.FileName = "dffScan.exe";
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                proc.StartInfo.Verb = "runas";
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Scan Shell Error", MessageBoxButtons.OK);
            }
        }
        private void ADGView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private void ADGView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //handle the row selection on right click
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    ADGView1.CurrentCell = ADGView1.Rows[e.RowIndex].Cells[0];
                    ADGView1.Rows[e.RowIndex].Selected = true;
                    ADGView1.Focus();

                    var relativeMousePosition = ADGView1.PointToClient(Cursor.Position);
                    contextMenuStrip1.Show(ADGView1, relativeMousePosition);
                    
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message, "ADG View 1 Cell Mouse Down Error", MessageBoxButtons.OK);
                }
            }
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> AcceptableExtensions = new List<string>
            {
                ".pdf", ".txt", ".xls", ".xlsx", ".doc", ".docx", ".ppt", ".pptx"
            };

            string FileName = Convert.ToString(ADGView1.SelectedRows[0].Cells["File Name"].Value);

            if (FileName.Trim().Length == 0)
            {
                MessageBox.Show("Select a line with a file name on it", "No Such File", MessageBoxButtons.OK);
                return;
            }

            if (!File.Exists(FileName))
            {
                MessageBox.Show(FileName + "\rNo longer exists on this system.", "No Such File", MessageBoxButtons.OK);
                return;
            }

            string FileNameExtension = Path.GetExtension(FileName).ToLower();

            bool MatchFound = false;

            for (int i = 0; i < AcceptableExtensions.Count(); i++)
            {
                if (FileNameExtension == AcceptableExtensions[i])
                {
                    MatchFound = true;
                    break;
                }
            }

            if (MatchFound)
            {
                Process proc = null;
                try
                {
                    proc = new Process();
                    proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(FileName);
                    proc.StartInfo.FileName = Path.GetFileName(FileName);
                    proc.StartInfo.CreateNoWindow = false;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "View File Error", MessageBoxButtons.OK);
                }
            }
            else
            {
                ViewFile Form = new ViewFile(FileName);
                Form.ShowDialog();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string FileName = Convert.ToString(ADGView1.SelectedRows[0].Cells["File Name"].Value);

            if (FileName.Trim().Length == 0)
            {
                MessageBox.Show("Select a line with a file name on it", "No Such File", MessageBoxButtons.OK);
                return;
            }

            if (!File.Exists(FileName))
            {
                MessageBox.Show(FileName + "\rNo longer exists on this system.", "No Such File", MessageBoxButtons.OK);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete:\r" + FileName, 
                                                        "Confirm Delete", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    File.Delete(FileName);
                    MessageBox.Show(FileName + " Was Deleted", "File Delete Success", MessageBoxButtons.OK);
                }
                catch 
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter("WinDFFdelete.bat"))
                        {
                            sw.WriteLine("DEL {0}", FileName);
                        }

                        Process proc = null;
                        proc = new Process();
                        proc.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                        proc.StartInfo.FileName = "WinDFFdelete.bat";
                        proc.StartInfo.CreateNoWindow = false;
                        proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        proc.Start();
                        proc.WaitForExit();

                        if (File.Exists(FileName))
                        {
                            MessageBox.Show(FileName + " Was *** NOT *** Deleted", 
                                            "File Delete Error (Shell Failed)", 
                                            MessageBoxButtons.OK);
                        }
                        else
                        {
                            MessageBox.Show(FileName + " Was Deleted", "File Delete Success", MessageBoxButtons.OK);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "File Delete Error", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void editScanParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditParameters Form = new EditParameters();
            Form.ShowDialog();
        }

        private void toolStripViewDriveInfo_Click(object sender, EventArgs e)
        {
            ViewDriveInfo Form = new ViewDriveInfo();
            Form.ShowDialog();
        }

        private void generalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpGeneral Form = new HelpGeneral();
            Form.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpAbout Form = new HelpAbout();
            Form.ShowDialog();
        }
    }
}
