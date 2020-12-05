using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WinDFF
{
    public partial class ViewFile : Form
    {
        public ViewFile(string FileName)
        {
            InitializeComponent();
            richTextBoxFileName.Text = "File: " + FileName;
            
        }

        private void ViewFile_Load(object sender, EventArgs e)
        {
            int ssPosition = richTextBoxFileName.Text.IndexOf(" ");
            string FileName = richTextBoxFileName.Text.Substring(ssPosition + 1, richTextBoxFileName.Text.Length - ssPosition - 1).Trim();

            string FileContents = string.Empty;

            const int bufferSize = 32;
            const long bytesToRead = bufferSize * 256;
            byte[] buffer = new byte[bufferSize];
            long read = 0;
            long r = -1;

            richTextBox1.Text = string.Empty;
            
            if (File.Exists(FileName))
            {
                try
                {
                    using (var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                    {
                        while (read <= bytesToRead && r != 0)
                        {
                            read += (r = stream.Read(buffer, 0, bufferSize));
                            StringBuilder hex = new StringBuilder(bufferSize * 3);
                            string DisplayLine = string.Empty;
                            foreach (byte b in buffer)
                            {
                                hex.AppendFormat("{0:x2}-", b);
                                if (b < ' ' || b > 'z')
                                {
                                    DisplayLine += ".";
                                }
                                else
                                {
                                    DisplayLine += Convert.ToString(Convert.ToChar(b));
                                }
                            }
                            string hexString = hex.ToString();
                            hexString = hexString.Substring(0, hexString.Length - 1);

                            if (richTextBox1.Text != string.Empty)
                            {
                                richTextBox1.Text += "\r";
                            }

                            richTextBox1.Text += hexString + "    " + DisplayLine;
                        }
                    }
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message, "View File Error", MessageBoxButtons.OK);
                    Close();
                    return;
                }
            }
            else
            {
                MessageBox.Show("File Does Not Exist", "View File Error", MessageBoxButtons.OK);
                Close();
                return;
            }

            richTextBox1.Text += "\r";
            richTextBox1.Text += "Only first " + bytesToRead.ToString("N0") + " bytes are shown";
            richTextBox1.Refresh();
        }
    }
}
