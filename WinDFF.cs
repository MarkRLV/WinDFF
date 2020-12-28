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

        public string ConfirmSingleFileDelete = string.Empty;
        public string ConfirmMultipleFileDelete = string.Empty;
        public DataTable dtFiles;
        public BindingSource SBind;
        public string CurrentActiveFile = string.Empty;


        private void WinDFF_Load(object sender, EventArgs e)
        {
            dtFiles = new DataTable();

            WindowState = FormWindowState.Maximized;
            if (File.Exists("WinDFF.ini"))
            {
                using (StreamReader sr = new StreamReader("WinDFF.ini"))
                {
                    while (!sr.EndOfStream)
                    {
                        string iniLine = sr.ReadLine().ToUpper();

                        if (iniLine.StartsWith("[CONFIRMSINGLEDELETE]="))
                        {
                            int eqSignPosition = iniLine.IndexOf('=');
                            if (iniLine.Length > eqSignPosition)
                            {
                                ConfirmSingleFileDelete = iniLine.Substring(eqSignPosition + 1, 1);
                            }
                            if (ConfirmSingleFileDelete != "Y" && ConfirmSingleFileDelete != "N")
                            {
                                ConfirmSingleFileDelete = "Y";
                            }
                        }

                        if (iniLine.StartsWith("[CONFIRMMULTIPLEDELETE]="))
                        {
                            int eqSignPosition = iniLine.IndexOf('=');
                            if (iniLine.Length > eqSignPosition)
                            {
                                ConfirmMultipleFileDelete = iniLine.Substring(eqSignPosition + 1, 1);
                            }
                            if (ConfirmMultipleFileDelete != "Y" && ConfirmMultipleFileDelete != "N")
                            {
                                ConfirmMultipleFileDelete = "Y";
                            }
                        }
                    }
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter("WinDFF.ini"))
                {
                    sw.WriteLine("[ConfirmSingleDelete]=YES");
                    sw.WriteLine("[ConfirmMultipleDelete]=YES");
                }
                ConfirmSingleFileDelete = "Y";
                ConfirmMultipleFileDelete = "Y";
            }
        }

        private void toolStripFileOpen_Click(object sender, EventArgs e)
        {
            string header = string.Empty;
            string InputLine = string.Empty;
            string filePath = string.Empty;

            string FileDate = string.Empty;
            string FileSize = string.Empty;
            string FileHash = string.Empty;
            string FileName = string.Empty;

            try
            {
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
                        CurrentActiveFile = filePath;

                        labelFileList.Text = "FileList: " + Path.GetFileName(filePath);

                        //Read the contents of the file into a stream
                        var fileStream = openFileDialog.OpenFile();

                        int FileRows = 0;
                        try
                        {
                            FileRows = dtFiles.Rows.Count;
                        }
                        catch (Exception ex)
                        {
                            string dummy = ex.Message;
                            FileRows = 0;
                        }

                        if (FileRows == 0)
                        {
                            dtFiles.Columns.Add("File Size", typeof(string));
                            dtFiles.Columns.Add("File Date", typeof(string));
                            dtFiles.Columns.Add("File Name", typeof(string));
                            dtFiles.Columns.Add("File Hash", typeof(string));
                        }
                        else
                        {
                            dtFiles.Clear();
                        }

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
                                    DataRow BlankRow = dtFiles.NewRow();
                                    BlankRow["File Size"] = " ";
                                    BlankRow["File Date"] = " ";
                                    BlankRow["File Name"] = " ";
                                    BlankRow["File Hash"] = " ";
                                    dtFiles.Rows.Add(BlankRow);
                                }

                                DataRow NewRow = dtFiles.NewRow();
                                NewRow["File Size"] = FileSize;
                                NewRow["File Date"] = FileDate;
                                NewRow["File Name"] = FileName;
                                NewRow["File Hash"] = FileHash;
                                dtFiles.Rows.Add(NewRow);

                                PriorHash = FileHash;
                            }

                            SBind = new BindingSource();
                            SBind.DataSource = dtFiles;

                            ADGView1.AutoGenerateColumns = true;  //must be "true" here
                            ADGView1.Columns.Clear();
                            ADGView1.DataSource = SBind;

                            for (int i = 0; i < ADGView1.Columns.Count; i++)
                            {
                                ADGView1.Columns[i].DataPropertyName = dtFiles.Columns[i].ColumnName;
                                ADGView1.Columns[i].HeaderText = dtFiles.Columns[i].Caption;
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace.ToString(), "Error in File Open", MessageBoxButtons.OK);
            }

            

        }

        private void toolStripFileSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentActiveFile == string.Empty)
                {
                    MessageBox.Show("No File List is currently open", "No File", MessageBoxButtons.OK);
                    return;
                }

                if (OverwriteFile(CurrentActiveFile))
                {
                    string TheMessage = "File Updated: " + CurrentActiveFile;

                    MessageBox.Show(TheMessage, "File Save Successful", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace.ToString(), "Error in File Save", MessageBoxButtons.OK);
            }

        }

        private void toolStripFileSaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                    saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string TheFile = saveFileDialog.FileName;
                        if (OverwriteFile(TheFile))
                        {
                            MessageBox.Show("File Saved", "File Save Successful", MessageBoxButtons.OK);
                            CurrentActiveFile = TheFile;
                            labelFileList.Text = "FileList: " + Path.GetFileName(CurrentActiveFile);
                            labelFileList.Refresh();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace.ToString(), "Error in File Save As", MessageBoxButtons.OK);
            }

            return;
        }
        private bool OverwriteFile(string TheFile)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(TheFile))
                {
                    sw.WriteLine("---- File Date ----  ----- File Size -----  ------------------------------- File Hash -------------------------------  ---- File Name ----");

                    for (int r = 0; r < dtFiles.Rows.Count; r++)
                    {
                        string FileSize = Convert.ToString(dtFiles.Rows[r]["File Size"]).Trim();
                        string FileDate = Convert.ToString(dtFiles.Rows[r]["File Date"]).Trim();
                        string FileName = Convert.ToString(dtFiles.Rows[r]["File Name"]).Trim();
                        string FileHash = Convert.ToString(dtFiles.Rows[r]["File Hash"]).Trim();

                        if (FileHash.Length > 0)
                        {
                            sw.WriteLine("{0}  {1,21}  [{2}]  {3}", FileDate, FileSize, FileHash, FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace.ToString(), "Error in Overwrite File", MessageBoxButtons.OK);
                return false;
            }

            return true;
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
                //proc.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace.ToString(), "Scan Shell Error", MessageBoxButtons.OK);
            }
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

        private void winDFFParametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                WinDFFParameters Form = new WinDFFParameters();
                Form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace.ToString(), "Error in WinDFF Parameters", MessageBoxButtons.OK);
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ADG View 1 Cell Mouse Down Error", MessageBoxButtons.OK);
                }
            }
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> AcceptableExtensions = new List<string>
            {
                ".pdf", ".txt", ".xls", ".xlsx", ".doc", ".docx", ".ppt", ".pptx"
            };

            DataGridViewRow[] selectedRows = ADGView1.SelectedRows.OfType<DataGridViewRow>().Where(row => !row.IsNewRow).ToArray();

            switch (selectedRows.Count())
            {
                case 0:
                    MessageBox.Show("You did not select any rows!", "No Rows Selected", MessageBoxButtons.OK);
                    return;

                case 1:
                    break;

                default:
                    MessageBox.Show("You cannot view more than one file at a time", "Too Many Rows Selected", MessageBoxButtons.OK);
                    return;

            }

            // Getting to here means only one row is selected

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
            int NumberOfSelectedRows = ADGView1.SelectedRows.Count;
            if (NumberOfSelectedRows == 0)
            {
                MessageBox.Show("You did not select any rows!", "No Rows Selected", MessageBoxButtons.OK);
                return;
            }

            ConfirmSingleFileDelete = "Y";
            ConfirmMultipleFileDelete = "Y";

            if (File.Exists("WinDFF.ini"))
            {
                using (StreamReader sr = new StreamReader("WinDFF.ini"))
                {
                    while (!sr.EndOfStream)
                    {
                        string iniLine = sr.ReadLine().ToUpper();

                        if (iniLine.StartsWith("[CONFIRMSINGLEDELETE]="))
                        {
                            int eqSignPosition = iniLine.IndexOf('=');
                            if (iniLine.Length > eqSignPosition)
                            {
                                ConfirmSingleFileDelete = iniLine.Substring(eqSignPosition + 1, 1);
                            }
                            if (ConfirmSingleFileDelete != "Y" && ConfirmSingleFileDelete != "N")
                            {
                                ConfirmSingleFileDelete = "Y";
                            }
                        }

                        if (iniLine.StartsWith("[CONFIRMMULTIPLEDELETE]="))
                        {
                            int eqSignPosition = iniLine.IndexOf('=');
                            if (iniLine.Length > eqSignPosition)
                            {
                                ConfirmMultipleFileDelete = iniLine.Substring(eqSignPosition + 1, 1);
                            }
                            if (ConfirmMultipleFileDelete != "Y" && ConfirmMultipleFileDelete != "N")
                            {
                                ConfirmMultipleFileDelete = "Y";
                            }
                        }
                    }
                }
            }

            bool Verbose = true;
            DialogResult dialogResult;

            if (NumberOfSelectedRows == 1 && ConfirmSingleFileDelete == "N")
            {
                Verbose = false;
            }

            string ListOfDeletedFilesFileName = string.Empty;
            if (NumberOfSelectedRows > 1)
            {
                DateTime StartTime = DateTime.Now;

                ListOfDeletedFilesFileName = "ListOfFilesDeletedOn-" + StartTime.ToString("yyyy-MM-dd-hh-mm-ss") + ".log";
                using (StreamWriter sw = File.CreateText(ListOfDeletedFilesFileName))
                {
                    sw.WriteLine("Files Deleted On {0}", StartTime.ToString("MM/dd/yyyy hh:mm:ss"));
                }

                if (ConfirmMultipleFileDelete == "N")
                {
                    Verbose = false;
                }

                if (Verbose)
                {
                    dialogResult = MessageBox.Show("Are you sure you want to delete ALL of the selected rows?",
                                                   "Multiple Delete Confirm",
                                                   MessageBoxButtons.YesNo);
                    switch (dialogResult)
                    {
                        case DialogResult.Yes:
                            break;

                        case DialogResult.No:
                            MessageBox.Show("Multiple File Delete Cancelled", "Multiple Delete Cancelled", MessageBoxButtons.OK);
                            return;

                        default:
                            MessageBox.Show("Try Again.  This time pick YES or NO.", "Multiple Delete Confirm", MessageBoxButtons.OK);
                            return;
                    }
                }
            }
            
            // Let the deleting begin!

            int NumberOfFilesDeleted = 0;

            for (int srow = 0; srow < NumberOfSelectedRows; srow++)
            {
                string FileName = Convert.ToString(ADGView1.SelectedRows[srow].Cells["File Name"].Value);

                if (FileName.Trim().Length > 0)
                {
                    if (File.Exists(FileName))
                    {
                        if (Verbose)
                        {
                            dialogResult = MessageBox.Show("Are you sure you want to delete:\r" + FileName,
                                                           "Confirm Delete",
                                                           MessageBoxButtons.YesNo);
                        }
                        else
                        {
                            dialogResult = DialogResult.Yes;
                        }

                        switch (dialogResult)
                        {
                            case DialogResult.Yes:
                                try
                                {
                                    File.Delete(FileName);
                                    ADGView1.SelectedRows[srow].Cells["File Name"].Value = "** DELETED **";
                                    ADGView1.SelectedRows[srow].Cells["File Hash"].Value = "** DELETED **";
                                    if (Verbose)
                                    {
                                        MessageBox.Show(FileName + " Was Deleted", "File Delete Success", MessageBoxButtons.OK);
                                    }

                                    if (ListOfDeletedFilesFileName != string.Empty)
                                    {
                                        using (StreamWriter sw = File.AppendText(ListOfDeletedFilesFileName))
                                        {
                                            sw.WriteLine(FileName);
                                        }
                                    }
                                    NumberOfFilesDeleted++;
                                }
                                catch (Exception ex)
                                {
                                    string dummy = ex.Message;
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
                                            ADGView1.SelectedRows[srow].Cells["File Name"].Value = "** DELETED **";
                                            ADGView1.SelectedRows[srow].Cells["File Hash"].Value = "** DELETED **";

                                            if (Verbose)
                                            {
                                                MessageBox.Show(FileName + " Was Deleted", "File Delete Success", MessageBoxButtons.OK);
                                            }
                                           
                                            NumberOfFilesDeleted++;

                                            if (ListOfDeletedFilesFileName != string.Empty)
                                            {
                                                using (StreamWriter sw = File.AppendText(ListOfDeletedFilesFileName))
                                                {
                                                    sw.WriteLine(FileName);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex2)
                                    {
                                        MessageBox.Show(ex2.Message + ex2.StackTrace.ToString(), "File Delete Error", MessageBoxButtons.OK);
                                    }
                                }
                                break;

                            case DialogResult.No:
                                break;

                            default:
                                MessageBox.Show("Try Again.  This time pick YES or NO.", "Multiple Delete Confirm", MessageBoxButtons.OK);
                                return;
                        }
                    }
                }
            }

            if (NumberOfFilesDeleted > 0)
            {
                dtFiles.AcceptChanges();
                ADGView1.Refresh();
                BtnRebuildList.Visible = true;

                DateTime EndTime = DateTime.Now;

                if (ListOfDeletedFilesFileName != string.Empty)
                {
                    using (StreamWriter sw = File.AppendText(ListOfDeletedFilesFileName))
                    {
                        sw.WriteLine("Deleting Files was completed at: {0}", EndTime.ToString("MM/dd/yyyy hh:mm:ss"));
                    }

                    dialogResult = MessageBox.Show("Log File:\r" + ListOfDeletedFilesFileName + " was created.\rWould you like to view it now?",
                                                   "View log of deleted file names", MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.Yes)
                    {
                        using (StreamWriter sw = new StreamWriter("ViewLog.bat"))
                        {
                            sw.WriteLine("notepad {0}", ListOfDeletedFilesFileName);
                        }
                        Process proc = null;
                        try
                        {
                            proc = new Process();
                            proc.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                            proc.StartInfo.FileName = "ViewLog.bat";
                            proc.StartInfo.CreateNoWindow = false;
                            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            proc.Start();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message + ex.StackTrace.ToString(), "View Log File Error", MessageBoxButtons.OK);
                        }
                    }
                }
            }
            else
            {
                BtnRebuildList.Visible = false;
                if (ListOfDeletedFilesFileName != string.Empty && File.Exists(ListOfDeletedFilesFileName))
                {
                    try
                    {
                        File.Delete(ListOfDeletedFilesFileName);
                    }
                    catch (Exception ex)
                    {
                        string dummy = ex.Message;
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

        
        private void BtnRebuildList_Click(object sender, EventArgs e)
        {
            DeleteRowsWithHash("** DELETED **");

            string CurrentHash = string.Empty;
            int CurrentHashCount = 0;

            List<string> UniqueHashes = new List<string>();

            for (int r = 0; r < dtFiles.Rows.Count; r++)
            {
                string ThisHash = Convert.ToString(dtFiles.Rows[r]["File Hash"]).Trim();

                if (ThisHash.Length > 0) // Ignore all the blank Lines
                {
                    if (CurrentHash == string.Empty) // If just getting started
                    {
                        CurrentHash = ThisHash;
                        CurrentHashCount = 1;
                    }
                    else
                    {
                        if (ThisHash == CurrentHash)
                        {
                            CurrentHashCount++;
                        }
                        else
                        {
                            if (CurrentHashCount == 1)
                            {
                                UniqueHashes.Add(CurrentHash);
                            }
                            CurrentHash = ThisHash;
                            CurrentHashCount = 1;
                        }
                    }
                }
            }

            if (CurrentHashCount == 1)
            {
                UniqueHashes.Add(CurrentHash);
            }

            foreach (string Hash in UniqueHashes)
            {
                DeleteRowsWithHash(Hash);
            }

            bool Rescan = true;
            while (Rescan)
            {
                Rescan = false;

                for (int r = 0; r < dtFiles.Rows.Count - 1; r++)
                {
                    int ThisHashLength = Convert.ToString(dtFiles.Rows[r]["File Hash"]).Trim().Length;
                    int NextHashLength = Convert.ToString(dtFiles.Rows[r + 1]["File Hash"]).Trim().Length;

                    if (ThisHashLength == 0 && NextHashLength == 0)
                    {
                        dtFiles.Rows[r]["File Hash"] = "** DELETE ME **";
                        Rescan = true;
                    }
                }
                dtFiles.AcceptChanges();

                if (Rescan)
                {
                    DeleteRowsWithHash("** DELETE ME **");
                }
            }

            ADGView1.Refresh();
            BtnRebuildList.Visible = false;
        }

        private void DeleteRowsWithHash(string v)
        {
            for (int i = dtFiles.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = dtFiles.Rows[i];
                if (dr["File Hash"].ToString() == v)
                {
                    dtFiles.Rows.Remove(dr);
                }
            }
            dtFiles.AcceptChanges();
        }
    }
}
