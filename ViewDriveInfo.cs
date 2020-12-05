using System;
using System.Data;
using System.Drawing;
using System.Management;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WinDFF
{
    public partial class ViewDriveInfo : Form
    {
        public ViewDriveInfo()
        {
            InitializeComponent();
        }

        private void ViewDriveInfo_Load(object sender, EventArgs e)
        {
            DataTable AllDriveInfo = new DataTable();
            AllDriveInfo.Columns.Add("DriveLetter", typeof(string));
            AllDriveInfo.Columns.Add("DriveNumber", typeof(string));
            AllDriveInfo.Columns.Add("VolumeLabel", typeof(string));
            AllDriveInfo.Columns.Add("VolumeSerialNumber", typeof(string));
            AllDriveInfo.Columns.Add("PhysicalSerialNumber", typeof(string));
            AllDriveInfo.Columns.Add("PhysicalModel", typeof(string));

            var drives = new ManagementObjectSearcher("Select * from Win32_LogicalDiskToPartition").Get().Cast<ManagementObject>();
            var disks = new ManagementObjectSearcher("Select * from Win32_LogicalDisk").Get().Cast<ManagementObject>();
            foreach (var drive in drives)
            {
                DataRow NewRow = AllDriveInfo.NewRow();

                var driveLetter = Regex.Match((string)drive["Dependent"], @"DeviceID=""(.*)""").Groups[1].Value;
                var driveNumber = Regex.Match((string)drive["Antecedent"], @"Disk #(\d*),").Groups[1].Value;

                NewRow["DriveLetter"] = Convert.ToString(driveLetter).Trim();
                NewRow["DriveNumber"] = Convert.ToString(driveNumber).Trim();

                var foundDisk = disks.Where((d) => d["Name"].ToString() == driveLetter).FirstOrDefault();
                if (foundDisk == null)
                {
                    foundDisk = disks.Where((d) => d.Path.ToString() == drive["Dependent"].ToString()).FirstOrDefault();
                }
                if (foundDisk == null)
                {
                    NewRow["VolumeLabel"] = "no label";
                    NewRow["VolumeSerialNumber"] = "no serial number";
                }
                else
                {
                    string volumeLabel = Convert.ToString(foundDisk["VolumeName"]).Trim();
                    if (volumeLabel.Length == 0) volumeLabel = "no label";
                    NewRow["VolumeLabel"] = volumeLabel;

                    string volumeSerialNumber = Convert.ToString(foundDisk["VolumeSerialNumber"]).Trim();

                    switch (volumeSerialNumber.Length)
                    {
                        case 0:
                            volumeSerialNumber = "no serial number";
                            break;

                        case 8:
                            volumeSerialNumber = volumeSerialNumber.Substring(0, 4) + "-" + volumeSerialNumber.Substring(4, 4);
                            break;
                    }
                    NewRow["VolumeSerialNumber"] = volumeSerialNumber;
                }
                AllDriveInfo.Rows.Add(NewRow);
            }


            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");


            foreach (ManagementObject wmi_HD in objSearcher.Get())
            {
                string FullDriveName;
                string FullDriveNumber;
                string PSN;

                FullDriveName = GetWMI(wmi_HD, "Tag");
                int StartingPos = FullDriveName.LastIndexOf('\\') + 1;
                int Length = FullDriveName.Length - StartingPos;
                FullDriveName = FullDriveName.Substring(StartingPos, Length);

                FullDriveNumber = FullDriveName.Substring(FullDriveName.Length - 1, 1);

                PSN = GetWMI(wmi_HD, "SerialNumber").Trim();

                if (FullDriveName.Contains("PHYSICALDRIVE"))
                {
                    for (int i = 0; i < AllDriveInfo.Rows.Count; i++)
                    {
                        if (Convert.ToString(AllDriveInfo.Rows[i]["DriveNumber"]) == FullDriveNumber)
                        {
                            AllDriveInfo.Rows[i]["PhysicalSerialNumber"] = PSN;
                        }
                    }
                }
            }

            ManagementObjectSearcher mosDisks = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            // Loop through each object (disk) retrieved by WMI
            foreach (ManagementObject moDisk in mosDisks.Get())
            {
                string PSN = moDisk["SerialNumber"].ToString().Trim();
                string PModel = moDisk["Model"].ToString();

                for (int i = 0; i < AllDriveInfo.Rows.Count; i++)
                {
                    if (Convert.ToString(AllDriveInfo.Rows[i]["PhysicalSerialNumber"]) == PSN)
                    {
                        AllDriveInfo.Rows[i]["PhysicalModel"] = PModel;
                    }
                }
            }

            int[] ColumnSize = new int[AllDriveInfo.Columns.Count];

            for (int i = 0; i < AllDriveInfo.Rows.Count; i++)
            {
                for (int j = 0; j < AllDriveInfo.Columns.Count; j++)
                {
                    string field = Convert.ToString(AllDriveInfo.Rows[i][j]);
                    int Length = field.Length;
                    if (Length > ColumnSize[j]) ColumnSize[j] = Length;
                }
            }

            BindingSource SBind = new BindingSource
            {
                DataSource = AllDriveInfo
            };

            dataGridView1.AutoGenerateColumns = true;  //must be "true" here
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = SBind;

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].DataPropertyName = AllDriveInfo.Columns[i].ColumnName;
                dataGridView1.Columns[i].HeaderText = AllDriveInfo.Columns[i].Caption;
            }

            dataGridView1.DefaultCellStyle.Font = new Font("Ariel", 10);

            dataGridView1.Enabled = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView1.Refresh();
        }

        private static string GetWMI(ManagementObject wmi_HD, string PName)
        {
            string Answer = string.Empty;
            if (wmi_HD[PName] == null)
                Answer = "None";
            else
                Answer = wmi_HD[PName].ToString().Trim();

            return Answer;
        }
    }
}
