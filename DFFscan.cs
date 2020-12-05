using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Management;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace dffScan
{
    class DFFscan
    {
        static void Main(string[] args)
        {
            //Sandbox();
            //Console.WriteLine("End of Sandbox, Press RETURN to continue");
            //Console.ReadLine();

            DateTime StartTime = DateTime.Now;
            Console.WriteLine("dffScan");
                        
            DataTable AllDriveInfo = new DataTable("AllDriveInfo");
            DataTable IniFile = new DataTable("IniFile");
            DataTable FilesTable = new DataTable("FilesTable");

            BuildTables(AllDriveInfo, IniFile, FilesTable);
            FilesTable = SortTable(FilesTable, "FileSize desc");
            AnalyzeForDuplicates(FilesTable, StartTime);
            ListCleanUp(StartTime);
            DisplayJobEndMessage(StartTime);

            return;
        }

        private static void Sandbox()
        {
            return;
        }

        private static void BuildTables(DataTable AllDriveInfo, DataTable IniFile, DataTable FilesTable)
        {
            GetDriveInfo(AllDriveInfo);

            IniFile.Columns.Add("Tag", typeof(string));
            IniFile.Columns.Add("Value", typeof(string));
            IniFile.Columns.Add("Qualifiers", typeof(string));
            ReadINIFile(IniFile);

            string DrivesToScan = GetParameter(IniFile, "[Drives To Scan]");
            string MinSizeString = GetParameter(IniFile, "[MinSize]");
            UInt64.TryParse(MinSizeString, out UInt64 MinSize);

            Console.WriteLine("This Run will scan these drives: {0}", DrivesToScan);
            Console.WriteLine("Only files that are at least {0} bytes will be analyzed", MinSize.ToString("N0"));
            
            FilesTable.Columns.Add("FileDate", typeof(DateTime));
            FilesTable.Columns.Add("FileSize", typeof(UInt64));
            FilesTable.Columns.Add("FileName", typeof(string));

            StreamWriter swlog = new StreamWriter("dffScan.log");

            string[] DrivesList = DrivesToScan.Split(',');

            foreach (string DriveLetter in DrivesList)
            {
                ScanDrive(DriveLetter, FilesTable, MinSize, swlog);
            }

            swlog.Close();

            Console.WriteLine("Scan Completed");
            return;

        }

        private static DataTable SortTable(DataTable FilesTable, string SortOrder)
        {
            Console.Write("Sorting Results...");
            try
            {
                FilesTable.DefaultView.Sort = SortOrder;
            }
            catch (Exception E)
            {
                Console.WriteLine("Error Sorting Files Table");
                Console.WriteLine(E.Message);
                Console.ReadLine();
            }

            DataTable SortedTable = FilesTable.DefaultView.ToTable();
            Console.WriteLine("Done.");

            return SortedTable;
        }

        private static void AnalyzeForDuplicates(DataTable FilesTable, DateTime StartTime)
        {
            Console.WriteLine("Hashing Files where two or more have the same file size");
            bool[] HasBeenWritten = new bool[FilesTable.Rows.Count];
            for (int i = 0; i < FilesTable.Rows.Count; i++)
            {
                HasBeenWritten[i] = false;
            }

            UInt64 ThisFileSize = 0;
            UInt64 NextFileSize = 0;
            DateTime ThisFileDate = new DateTime(1970, 1, 1);
            string ThisFileName = string.Empty;
            string ThisFileHash = string.Empty;

            string FileListName = "dffFileList" + StartTime.ToString("yyyyMMddHHmm") + ".temp";
            StreamWriter sw = new StreamWriter(FileListName);
            sw.WriteLine("---- File Date ----  ----- File Size -----  ------------------------------- File Hash -------------------------------  ---- File Name ----");

            for (int i = 0; i < FilesTable.Rows.Count - 1; i++)
            {
                ThisFileSize = Convert.ToUInt64(FilesTable.Rows[i]["FileSize"]);
                NextFileSize = Convert.ToUInt64(FilesTable.Rows[i + 1]["FileSize"]);

                if (ThisFileSize == NextFileSize)
                {
                    if (!HasBeenWritten[i])
                    {
                        ThisFileDate = Convert.ToDateTime(FilesTable.Rows[i]["FileDate"]);
                        ThisFileName = Convert.ToString(FilesTable.Rows[i]["FileName"]);
                        ThisFileHash = GetFileSHA256(ThisFileName);
                        sw.WriteLine("{0}  {1,19}  [{2}]  {3}",
                            ThisFileDate.ToString("MM/dd/yyyy hh:mm tt"),
                            ThisFileSize.ToString("N0"),
                            ThisFileHash, ThisFileName);
                        HasBeenWritten[i] = true;
                    }

                    ThisFileSize = NextFileSize;
                    ThisFileDate = Convert.ToDateTime(FilesTable.Rows[i + 1]["FileDate"]);
                    ThisFileName = Convert.ToString(FilesTable.Rows[i + 1]["FileName"]);
                    ThisFileHash = GetFileSHA256(ThisFileName);
                    sw.WriteLine("{0}  {1,19}  [{2}]  {3}", 
                        ThisFileDate.ToString("MM/dd/yyyy hh:mm tt"), 
                        ThisFileSize.ToString("N0"), 
                        ThisFileHash, ThisFileName);
                    HasBeenWritten[i + 1] = true;
                }
            }
            sw.Close();
            Console.WriteLine("{0} has been created.", FileListName);
            return;
        }

        private static void ListCleanUp(DateTime StartTime)
        {
            DataTable FilesTable = new DataTable();
            FilesTable.Columns.Add("FileSize", typeof(string));
            FilesTable.Columns.Add("FileHash", typeof(string));
            FilesTable.Columns.Add("FileDate", typeof(string));
            FilesTable.Columns.Add("FileName", typeof(string));

            string InputFile = "dffFileList" + StartTime.ToString("yyyyMMddHHmm") + ".temp";

            StreamReader sr = new StreamReader(InputFile);
            string Header = sr.ReadLine();

            while (!sr.EndOfStream)
            {
                string InputLine = sr.ReadLine();
                if (InputLine.Contains("[") && InputLine.Contains("]") && InputLine.Contains("M"))
                {
                    int LeftBracketPosition = InputLine.IndexOf('[');
                    int RightBracketPosition = InputLine.IndexOf(']');
                    int MPosition = InputLine.IndexOf("M");

                    int DateLength = MPosition + 1;
                    int SizeLength = LeftBracketPosition - MPosition - 3;
                    int HashLength = RightBracketPosition - LeftBracketPosition - 1;
                    int NameLength = InputLine.Length - RightBracketPosition - 1;

                    string FileHash = InputLine.Substring(LeftBracketPosition + 1, HashLength).Trim();

                    if (!FileHash.Contains("Error"))
                    {
                        DataRow NewFileRow = FilesTable.NewRow();
                        NewFileRow["FileDate"] = InputLine.Substring(0, DateLength).Trim();
                        NewFileRow["FileSize"] = InputLine.Substring(MPosition + 1, SizeLength);
                        NewFileRow["FileHash"] = InputLine.Substring(LeftBracketPosition + 1, HashLength).Trim();
                        NewFileRow["FileName"] = InputLine.Substring(RightBracketPosition + 1, NameLength).Trim();
                        FilesTable.Rows.Add(NewFileRow);
                    }
                }
            }
            sr.Close();

            FilesTable.DefaultView.Sort = "FileSize desc, FileHash";
            FilesTable = FilesTable.DefaultView.ToTable();

            List<string> HashList = new List<string>();
            List<int> HashCount = new List<int>();

            int HashListSub = 0;
            int HashListCount = 0;

            for (int i = 0; i < FilesTable.Rows.Count; i++)
            {
                string FileHash = Convert.ToString(FilesTable.Rows[i]["FileHash"]);

                bool HashFound = false;

                if (HashListCount > 0)
                {
                    if (HashListSub < HashListCount && FileHash == HashList[HashListSub])
                    {
                        HashFound = true;
                    }
                    else
                    {
                        for (int j = 0; j < HashListCount; j++)
                        {
                            if (FileHash == HashList[j])
                            {
                                HashFound = true;
                                HashListSub = j;
                            }
                        }
                    }
                }

                if (HashFound)
                {
                    HashCount[HashListSub]++;
                }
                else
                {
                    HashList.Add(FileHash);
                    HashCount.Add(1);
                    HashListCount = HashList.Count();
                    HashListSub = HashListCount - 1;
                }
            }

            HashListSub = 0;
            HashListCount = HashList.Count();

            string OutputFile = "dffFileList" + StartTime.ToString("yyyyMMddHHmm") + ".txt";
            StreamWriter sw = new StreamWriter(OutputFile);
            sw.WriteLine(Header);

            for (int i = 0; i < FilesTable.Rows.Count; i++)
            {
                string FileHash = Convert.ToString(FilesTable.Rows[i]["FileHash"]);
                string FileSize = Convert.ToString(FilesTable.Rows[i]["FileSize"]);
                string FileDate = Convert.ToString(FilesTable.Rows[i]["FileDate"]);
                string FileName = Convert.ToString(FilesTable.Rows[i]["FileName"]);

                bool HashFound = false;

                if (HashListSub < HashListCount - 1 && FileHash == HashList[HashListSub + 1])
                {
                    HashListSub++;
                }

                if (HashListSub < HashListCount && FileHash == HashList[HashListSub])
                {
                    HashFound = true;
                }
                else
                {
                    for (int j = 0; j < HashListCount; j++)
                    {
                        if (FileHash == HashList[j])
                        {
                            HashFound = true;
                            HashListSub = j;
                        }
                    }
                }

                if (HashFound && HashCount[HashListSub] > 1)
                {
                    sw.WriteLine("{0}  {1,19}  [{2}]  {3}", FileDate, FileSize, FileHash, FileName);
                }
            }

            sw.Close();
            if (File.Exists(InputFile))
            {
                File.Delete(InputFile);
            }

            return;
        }

        private static void DisplayAnyTable(DataTable AnyTable)
        {
            int[] MaxColumnWidth = new int[AnyTable.Columns.Count];

            for (int c = 0; c < AnyTable.Columns.Count; c++)
            {
                string DataField = Convert.ToString(AnyTable.Columns[c].ColumnName);
                int FieldLength = DataField.Length;
                if (FieldLength > MaxColumnWidth[c]) MaxColumnWidth[c] = FieldLength;
            }

            for (int w = 0; w < AnyTable.Rows.Count; w++)
            {
                for (int c = 0; c < AnyTable.Columns.Count; c++)
                {
                    string DataField = Convert.ToString(AnyTable.Rows[w][c]);
                    int FieldLength = DataField.Length;
                    if (FieldLength > MaxColumnWidth[c]) MaxColumnWidth[c] = FieldLength;
                }
            }

            for (int c = 0; c < AnyTable.Columns.Count; c++)
            {
                string DataField = Convert.ToString(AnyTable.Columns[c].ColumnName);
                string OutputFormat = "{0," + MaxColumnWidth[c].ToString() + "} ";
                Console.Write(OutputFormat, DataField);
            }
            Console.WriteLine();

            for (int c = 0; c < AnyTable.Columns.Count; c++)
            {
                string OutputFormat = "{0," + MaxColumnWidth[c].ToString() + "} ";
                Console.Write(OutputFormat, string.Concat(Enumerable.Repeat("-", MaxColumnWidth[c])));
            }
            Console.WriteLine();

            for (int c = 0; c < AnyTable.Columns.Count; c++)
            {
                string DataField = Convert.ToString(AnyTable.Columns[c].ColumnName);
                int FieldLength = DataField.Length;
                if (FieldLength > MaxColumnWidth[c]) MaxColumnWidth[c] = FieldLength;
            }

            int LineCount = 0;

            for (int w = 0; w < AnyTable.Rows.Count; w++)
            {
                for (int c = 0; c < AnyTable.Columns.Count; c++)
                {
                    string DataField = Convert.ToString(AnyTable.Rows[w][c]);
                    string OutputFormat = "{0," + MaxColumnWidth[c].ToString() + "} ";
                    Console.Write(OutputFormat, DataField);
                }
                Console.WriteLine();
                LineCount++;
                if (LineCount > 30)
                {
                    Console.Write("Press RETURN to continue:  ");
                    Console.ReadLine();
                    LineCount = 0;
                }
            }

            Console.WriteLine("End of Table");
            Console.Write("Press RETURN to continue:  ");
            Console.ReadLine();
            return;
        }


        static void GetDriveInfo(DataTable AllDriveInfo)
        {
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
                    NewRow["VolumeLabel"] = "no_label";
                    NewRow["VolumeSerialNumber"] = "no_sn";
                }
                else
                {
                    string volumeLabel = Convert.ToString(foundDisk["VolumeName"]).Trim();
                    if (volumeLabel.Length == 0) volumeLabel = "no_label";
                    NewRow["VolumeLabel"] = volumeLabel;

                    string volumeSerialNumber = Convert.ToString(foundDisk["VolumeSerialNumber"]).Trim();
                    if (volumeSerialNumber.Length == 0) volumeLabel = "no_sn";
                    NewRow["VolumeSerialNumber"] = volumeSerialNumber;
                }
                AllDriveInfo.Rows.Add(NewRow);
            }


            ManagementObjectSearcher objSearcher = new
               ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");


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

            List<string> Header1 = new List<string>
            {
                "Drive", "SATA", " ", "Volume", " ", " "
            };

            List<string> Header2 = new List<string>
            {
                "Letter", "Number", "Label", "Serial #", "Physical Serial #", "Device Name"
            };

            for (int j = 0; j < Header1.Count(); j++)
            {
                int Length1 = Header1[j].Length;
                int Length2 = Header2[j].Length;
                if (Length1 > ColumnSize[j]) ColumnSize[j] = Length1;
                if (Length2 > ColumnSize[j]) ColumnSize[j] = Length2;
            }

            StreamWriter sw = new StreamWriter("dffScan.DriveInfo");

            Console.WriteLine("Your Physical Drives:");
            sw.WriteLine("Your Physical Drives:");

            int tablewidth = 0;
            for (int j = 0; j < AllDriveInfo.Columns.Count; j++)
            {
                tablewidth += (ColumnSize[j] + 3);
            }
            tablewidth += 2;

            string breakline = string.Concat(Enumerable.Repeat("-", tablewidth));
            Console.WriteLine(breakline);
            sw.WriteLine(breakline);

            for (int j = 0; j < Header1.Count(); j++)
            {
                string FieldFormat = "| {0,-" + Convert.ToString(ColumnSize[j]) + "} ";
                Console.Write(FieldFormat, Header1[j]);
                sw.Write(FieldFormat, Header1[j]);
            }
            Console.WriteLine(" |");
            sw.WriteLine(" |");

            for (int j = 0; j < Header2.Count(); j++)
            {
                string FieldFormat = "| {0,-" + Convert.ToString(ColumnSize[j]) + "} ";
                Console.Write(FieldFormat, Header2[j]);
                sw.Write(FieldFormat, Header2[j]);
            }
            Console.WriteLine(" |");
            sw.WriteLine(" |");

            Console.WriteLine(breakline);
            sw.WriteLine(breakline);

            for (int i = 0; i < AllDriveInfo.Rows.Count; i++)
            {
                for (int j = 0; j < AllDriveInfo.Columns.Count; j++)
                {
                    string FieldFormat = "| {0,-" + Convert.ToString(ColumnSize[j]) + "} ";
                    Console.Write(FieldFormat, Convert.ToString(AllDriveInfo.Rows[i][j]));
                    sw.Write(FieldFormat, Convert.ToString(AllDriveInfo.Rows[i][j]));
                }
                Console.WriteLine(" |");
                sw.WriteLine(" |");
            }

            Console.WriteLine(breakline);
            sw.WriteLine(breakline);

            sw.Close();

            return;
        }

        static string GetWMI(ManagementObject wmi_HD, string PName)
        {
            string Answer = string.Empty;
            if (wmi_HD[PName] == null)
                Answer = "None";
            else
                Answer = wmi_HD[PName].ToString().Trim();

            return Answer;
        }

        static void ReadINIFile(DataTable IniFile)
        {
            char[] delims = { '=', '/' };

            StreamReader sr = new StreamReader("dffScan.ini");
            while (!sr.EndOfStream)
            {
                string IniText = sr.ReadLine();
                
                if (IniText.Length > 0)
                {
                    string[] chunks = IniText.Split(delims);
                    int i = 0;
                    DataRow NewRow = IniFile.NewRow();
                    foreach (string chunk in chunks)
                    {
                        switch (i)
                        {
                            case 0:
                                NewRow["Tag"] = chunk;
                                break;

                            case 1:
                                NewRow["Value"] = chunk;
                                break;

                            default:
                                NewRow["Qualifiers"] += "/" + chunk;
                                break;
                        }

                        i++;
                    }
                    IniFile.Rows.Add(NewRow);
                }

            }
            return;
        }

        private static string GetParameter(DataTable IniFile, string TagToFind)
        {
            string TagToFindUpper = TagToFind.ToUpper();

            for (int i = 0; i < IniFile.Rows.Count; i++)
            {
                string tag = Convert.ToString(IniFile.Rows[i]["Tag"]);
                if (tag.ToUpper() == TagToFindUpper)
                {
                    return Convert.ToString(IniFile.Rows[i]["Value"]);
                }
            }

            return string.Empty;
        }

        static void ScanDrive(string DriveLetter, DataTable FilesTable, UInt64 MinSize, StreamWriter swlog)
        {
            string DriveName = DriveLetter + ":";

            string VolumeName = string.Empty;
            string VolumeSerialNumber = string.Empty;
            string PhysicalSerialNumber = string.Empty;

            string SearchString = DriveLetter + ":\\";
            Console.WriteLine("Scanning Drive {0}", SearchString);
            ProcessDirectory(SearchString, FilesTable, MinSize, swlog);

            return;
        }

        private static void ProcessDirectory(string folder, DataTable FilesTable, UInt64 MinSize, StreamWriter swlog)
        {
            int SlashCount = 0;

            for (int i = 0; i < folder.Length; i++)
            {
                if (folder.Substring(i, 1) == "\\") SlashCount++;
            }

            if (SlashCount < 3)
            {
                Console.WriteLine("Processing: [{0}\\] and its subdirectories", folder);
            }

            int FilesReviewed = 0;
            UInt64 TotalSpace = 0;

            try
            {
                string[] files = Directory.GetFiles(folder, "*.*", SearchOption.TopDirectoryOnly);
                foreach (string FileName in files)
                {
                    FilesReviewed++;
                    if (FilesReviewed % 500 == 0)
                    {
                        Console.WriteLine("In Directory [{0}\\] Files Reviewed={1}", folder, FilesReviewed);
                    }

                    try
                    {
                        FileInfo fi = new FileInfo(FileName);
                        UInt64 ThisFileSize = Convert.ToUInt64(fi.Length);
                        TotalSpace += ThisFileSize;

                        if (ThisFileSize >= MinSize)
                        {
                            DataRow NewFileRow = FilesTable.NewRow();
                            NewFileRow["FileDate"] = RoundTime(fi.LastWriteTime);
                            NewFileRow["FileSize"] = ThisFileSize;
                            NewFileRow["FileName"] = FileName;
                            FilesTable.Rows.Add(NewFileRow);
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("For File: {0}", FileName);
                        Console.WriteLine(E.Message);
                        swlog.WriteLine("For File: {0}", FileName);
                        swlog.WriteLine(E.Message);
                        Console.WriteLine("Scanning Continues");
                    }
                }

                if (FilesReviewed > 500)
                {
                    Console.WriteLine("In Directory [{0}\\] Files Reviewed={1}.  Directory Completed.", folder, FilesReviewed);
                }

                swlog.WriteLine("Directory [{0}\\] has {1} Files and uses {2} bytes of space.", 
                    folder, FilesReviewed, TotalSpace.ToString("N0"));

                try
                {
                    string[] Subdirectories = Directory.GetDirectories(folder);

                    try
                    {
                        foreach (string Subdirectory in Subdirectories)
                        {
                            if (IsIgnorable(Subdirectory))
                            {
                                swlog.WriteLine("Directory [{0}\\] was ignored.", Subdirectory);
                            }
                            else
                            {
                                try
                                {
                                    ProcessDirectory(Subdirectory, FilesTable, MinSize, swlog);
                                }
                                catch (Exception E)
                                {
                                    Console.WriteLine("For Subdirectory: {0} in Folder {1}", Subdirectory, folder);
                                    Console.WriteLine(E.Message);
                                    swlog.WriteLine("For Subdirectory: {0} in Folder {1}", Subdirectory, folder);
                                    swlog.WriteLine(E.Message);
                                    Console.WriteLine("Scanning Continues");
                                }
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("For (1) Folder {0}", folder);
                        Console.WriteLine(E.Message);
                        swlog.WriteLine("For (1) Folder {0}", folder);
                        swlog.WriteLine(E.Message);
                        Console.WriteLine("Scanning Continues");
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine("For (2) Folder {0}", folder);
                    Console.WriteLine(E.Message);
                    swlog.WriteLine("For (2) Folder {0}", folder);
                    swlog.WriteLine(E.Message);
                    Console.WriteLine("Scanning Continues");
                }
            }
            catch (Exception E)
            {
                Console.WriteLine("For (3) Folder {0}", folder);
                Console.WriteLine(E.Message);
                swlog.WriteLine("For (3) Folder {0}", folder);
                swlog.WriteLine(E.Message);
                Console.WriteLine("Scanning Continues");
            }

            return;
        }

        static bool IsIgnorable(string dir)
        {
            string dirUpCase = dir.ToUpper();
            if (dirUpCase.EndsWith("SYSTEM VOLUME INFORMATION")) return true;
            if (dirUpCase.Contains("$RECYCLE.BIN")) return true;

            bool isAjunction = (new DirectoryInfo(dir).Attributes & FileAttributes.ReparsePoint) != 0;

            if (isAjunction)
            {
                Console.WriteLine("{0} is a junction and will not be scanned.", dir);
                return true;
            }

            return false;
        }

        static string GetFileSHA256(string file)
        {
            try
            {
                using (FileStream stream = File.OpenRead(file))
                {
                    Console.Write("Hashing {0}..", file);
                    SHA256Managed SHA256 = new SHA256Managed();
                    byte[] checksum = SHA256.ComputeHash(stream);
                    Console.WriteLine("Done");
                    string UnformattedHash = BitConverter.ToString(checksum).Replace("-", string.Empty);
                    string FormattedHash = string.Empty;
                    int i = 0;
                    while (i < UnformattedHash.Length)
                    {
                        if (i > 0) FormattedHash += "-";
                        FormattedHash += UnformattedHash.Substring(i, 8);

                        i += 8;
                    }
                    return FormattedHash;
                }
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }

            return "Hashing Error";
        }

        static DateTime RoundTime(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
        }

        private static void DisplayJobEndMessage(DateTime StartTime)
        {
            DateTime EndTime = DateTime.Now;

            Console.WriteLine();
            Console.WriteLine("Start Time : {0}", StartTime);
            Console.WriteLine("  End Time : {0}", EndTime);
            Console.WriteLine("This run took {0} minutes.", ((EndTime - StartTime).TotalMinutes).ToString("F2"));

            Console.WriteLine("Run Completed.  Press RETURN to continue.");
            Console.ReadLine();
        }
    }
}

