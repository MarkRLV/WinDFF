using System;
using System.IO;
using System.Windows.Forms;

namespace WinDFF
{
    public partial class WinDFFParameters : Form
    {
        public WinDFFParameters()
        {
            InitializeComponent();
        }

        public string ConfirmSingleFileDelete = string.Empty;
        public string ConfirmMultipleFileDelete = string.Empty;

        private void WinDFFParameters_Load(object sender, EventArgs e)
        {
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

            switch (ConfirmSingleFileDelete)
            {
                case "Y":
                    cbSingle.SelectedIndex = 0;
                    break;

                case "N":
                    cbSingle.SelectedIndex = 1;
                    break;

                default:
                    cbSingle.SelectedIndex = 0;
                    break;
            }

            switch (ConfirmMultipleFileDelete)
            {
                case "Y":
                    cbMultiple.SelectedIndex = 0;
                    break;

                case "N":
                    cbMultiple.SelectedIndex = 1;
                    break;

                default:
                    cbMultiple.SelectedIndex = 0;
                    break;
            }
        }

        private void BtnSaveParameters_Click(object sender, EventArgs e)
        {
            int Single = cbSingle.SelectedIndex;
            int Multiple = cbMultiple.SelectedIndex;

            using (StreamWriter sw = new StreamWriter("WinDFF.ini"))
            {
                if (Single == 1)
                {
                    sw.WriteLine("[ConfirmSingleDelete]=NO");
                }
                else
                {
                    sw.WriteLine("[ConfirmSingleDelete]=YES");
                }

                if (Multiple == 1)
                {
                    sw.WriteLine("[ConfirmMultipleDelete]=NO");
                }
                else
                {
                    sw.WriteLine("[ConfirmMultipleDelete]=YES");
                }
            }

            MessageBox.Show("Parameters Saved", "WinDFF Parameters", MessageBoxButtons.OK);
        }
    }
}
