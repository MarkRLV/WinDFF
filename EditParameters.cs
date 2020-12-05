using System;
using System.IO;
using System.Windows.Forms;

namespace WinDFF
{
    public partial class EditParameters : Form
    {
        public EditParameters()
        {
            InitializeComponent();
        }

        private void EditParameters_Load(object sender, EventArgs e)
        {
            if (File.Exists("dffScan.ini"))
            {
                using (StreamReader sr = new StreamReader("dffScan.ini"))
                {
                    while (!sr.EndOfStream)
                    {
                        string InputLine = sr.ReadLine().Trim();
                        string InputLineLower = InputLine.ToLower();

                        if (InputLineLower.StartsWith("[drives to scan]="))
                        {
                            int ssPosition = InputLine.IndexOf('=') + 1;
                            int ssLength = InputLine.Length - ssPosition;
                            txtDrivesToScan.Text = InputLine.Substring(ssPosition, ssLength).ToUpper();
                        }

                        if (InputLineLower.StartsWith("[minsize]="))
                        {
                            int ssPosition = InputLine.IndexOf('=') + 1;
                            int ssLength = InputLine.Length - ssPosition;
                            string MinSizeString = InputLine.Substring(ssPosition, ssLength);
                            long.TryParse(MinSizeString, out long MinSize);
                            txtMinimumSize.Text = MinSize.ToString("N0");
                        }
                    }
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter("dffScan.ini"))
                {
                    sw.WriteLine("[Drives to Scan]=C");
                    sw.WriteLine("[MinSize]=3000000");
                }

                txtDrivesToScan.Text = "C";
                txtMinimumSize.Text = "3,000,000";
            }
        }

        private void btnSaveParameters_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter("dffScan.ini"))
            {
                sw.WriteLine("[Drives to Scan]={0}", txtDrivesToScan.Text.ToUpper().Trim());
                string MinSizeString = txtMinimumSize.Text.Trim().Replace(",", string.Empty);
                long.TryParse(MinSizeString, out long MinSize);
                sw.WriteLine("[MinSize]={0}", MinSize.ToString());
                txtMinimumSize.Text = txtMinimumSize.Text = MinSize.ToString("N0");
            }

            MessageBox.Show("Parameters Saved.", "Edit Parameters", MessageBoxButtons.OK);
            
        }
    }
}
