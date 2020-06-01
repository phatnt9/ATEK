using ATEK.AccessControl_2.Services;
using ATEK.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Excel = Microsoft.Office.Interop.Excel;

namespace ATEK.AccessControl_2.Profiles
{
    public class ImportProfilesViewModel : BindableBase
    {
        private readonly IAccessControlRepository repo;
        private bool addProfilesChecked;
        private string importProcess;
        private int importProcessValue;
        private string importStatus;
        private string importFilePath;
        private string importFileFolder;
        private BackgroundWorker importBackGroundWorker;
        private bool isBackGroundWorkerBusy;
        private List<Class> allClasses;
        private List<Group> allGroups;
        private Excel.Application xlApp;
        private Excel.Workbook xlWorkbook;
        private Excel._Worksheet xlWorksheet;
        private Excel.Range xlRange;

        public ImportProfilesViewModel(IAccessControlRepository repo)
        {
            this.repo = repo;
            AddProfilesChecked = true;
            importBackGroundWorker = new BackgroundWorker();
            SelectFileCommand = new RelayCommand(OnSelectFile);
            StartImportCommand = new RelayCommand(OnStartImport);
            StopImportCommand = new RelayCommand(OnStopImport);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private void OnStopImport()
        {
            if (importBackGroundWorker.WorkerSupportsCancellation)
            {
                importBackGroundWorker.CancelAsync();
            }
        }

        #region Methods

        private void OnStartImport()
        {
            ImportProcessValue = 0;
            if (string.IsNullOrEmpty(importFilePath))
            {
                System.Windows.Forms.MessageBox.Show(String.Format(GlobalConstant.messageValidate, "File", "File"), GlobalConstant.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(importFilePath))
            {
                System.Windows.Forms.MessageBox.Show("File not Exist!", GlobalConstant.messageTitileError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            importBackGroundWorker = new BackgroundWorker();
            importBackGroundWorker.WorkerSupportsCancellation = true;
            importBackGroundWorker.WorkerReportsProgress = true;
            importBackGroundWorker.DoWork += ImportBackGroundWorker_DoWork;
            importBackGroundWorker.RunWorkerCompleted += ImportBackGroundWorker_RunWorkerCompleted;
            importBackGroundWorker.ProgressChanged += ImportBackGroundWorker_ProgressChanged;
            importBackGroundWorker.Disposed += ImportBackGroundWorker_Disposed;
            importBackGroundWorker.RunWorkerAsync();
            IsBackGroundWorkerBusy = true;
        }

        private void ImportBackGroundWorker_Disposed(object sender, EventArgs e)
        {
            IsBackGroundWorkerBusy = false;
        }

        private void ImportBackGroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ImportProcessValue = e.ProgressPercentage;
        }

        private void ImportBackGroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // check error, check cancel, then use result
            if (e.Error != null)
            {
                // handle the error
                ImportStatus = "Error";
            }
            else if (e.Cancelled)
            {
                // handle cancellation
                ImportStatus = "Cancelled";
            }
            else
            {
            }
            // general cleanup code, runs when there was an error or not.
            ImportFilePath = "";
            ImportProcessValue = 0;
            importBackGroundWorker.Dispose();

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close(false);
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            xlWorksheet = null;
            xlWorkbook = null;
            xlApp = null;
            xlRange = null;
        }

        private void ImportBackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            xlApp = new Excel.Application();
            xlWorkbook = xlApp.Workbooks.Open(importFilePath);
            xlWorksheet = xlWorkbook.Sheets[1];
            xlRange = xlWorksheet.UsedRange;

            List<Profile> listProfiles = new List<Profile>();
            Profile profile = new Profile();

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            string className = "";
            string groupName = "";
            string errorColumn = "";
            int errorColumnIndex = 0;
            int errorRowIndex = 0;
            bool hasError = false;

            for (int i = 2; i <= rowCount; i++)
            {
                ImportStatus = "Reading";
                profile = new Profile();
                className = "";
                groupName = "";
                errorColumn = "";
                errorColumnIndex = 0;
                errorRowIndex = 0;
                hasError = false;

                //Name 2
                if (CheckStringInputFromExcel(xlRange.Cells[i, 2].Value2))
                {
                    profile.Name = xlRange.Cells[i, 2].Value2.ToString().ToUpper();
                }
                else
                {
                    errorColumn = "Name";
                    errorColumnIndex = 2;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Pinno 8
                if (CheckNumberInputFromExcel(xlRange.Cells[i, 8].Value2))
                {
                    profile.Pinno = xlRange.Cells[i, 8].Value2.ToString();
                }
                else
                {
                    errorColumn = "Pinno";
                    errorColumnIndex = 8;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Adno 3
                if (CheckStringInputFromExcel(xlRange.Cells[i, 3].Value2))
                {
                    profile.Adno = xlRange.Cells[i, 3].Value2.ToString().ToUpper();
                }
                else
                {
                    errorColumn = "Adno";
                    errorColumnIndex = 3;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Gender 4
                if (CheckStringInputFromExcel(xlRange.Cells[i, 4].Value2))
                {
                    profile.Gender = (xlRange.Cells[i, 4].Value2.ToString().ToUpper() == "MALE" ? "Male" : "Female");
                }
                else
                {
                    errorColumn = "Gender";
                    errorColumnIndex = 4;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Date of Birth 5
                if (CheckDateInputFromExcel(xlRange.Cells[i, 5].Value2))
                {
                    profile.DateOfBirth = ParseDateTimeFormCell(xlRange.Cells[i, 5].Value2.ToString());
                }
                else
                {
                    errorColumn = "Date of Birth";
                    errorColumnIndex = 5;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Date of Issue 6
                if (CheckDateInputFromExcel(xlRange.Cells[i, 6].Value2))
                {
                    profile.DateOfIssue = ParseDateTimeFormCell(xlRange.Cells[i, 6].Value2.ToString());
                }
                else
                {
                    errorColumn = "Date of Issue";
                    errorColumnIndex = 6;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Image 7
                if (CheckStringInputFromExcel(xlRange.Cells[i, 7].Value2))
                {
                    profile.Image = xlRange.Cells[i, 7].Value2.ToString();
                }
                else
                {
                    errorColumn = "Image";
                    errorColumnIndex = 7;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Class 9
                if (CheckStringInputFromExcel(xlRange.Cells[i, 9].Value2))
                {
                    className = xlRange.Cells[i, 9].Value2.ToString();
                    int classId = CheckClassNameValid(className.ToUpper());
                    if (classId == 0)
                    {
                        //Create New Class
                        Class newClass = new Class() { Name = className.ToUpper() };
                        repo.AddClass(newClass);
                        LoadData();
                        profile.ClassId = newClass.Id;
                        //ImportProfileImage(importFileFolder, profile.IMAGE);
                    }
                    else
                    {
                        profile.ClassId = classId;
                        //ImportProfileImage(importFileFolder, profile.IMAGE);
                    }
                }
                else
                {
                    errorColumn = "Class";
                    errorColumnIndex = 9;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Group 10
                if (CheckStringInputFromExcel(xlRange.Cells[i, 10].Value2))
                {
                    groupName = xlRange.Cells[i, 10].Value2.ToString();
                    int groupId = CheckGroupNameValid(groupName.ToUpper());
                    if (groupId == 0)
                    {
                        //Create New Group
                        Group newGroup = new Group() { Name = groupName.ToUpper() };
                        repo.AddGroup(newGroup);
                        LoadData();
                        profile.ProfileGroups.Add(new ProfileGroup() { GroupId = newGroup.Id });
                        //ImportProfileImage(importFileFolder, profile.IMAGE);
                    }
                    else
                    {
                        profile.ProfileGroups.Add(new ProfileGroup() { GroupId = groupId });
                        //ImportProfileImage(importFileFolder, profile.IMAGE);
                    }
                }
                else
                {
                    errorColumn = "Group";
                    errorColumnIndex = 10;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Email 11
                if (CheckStringInputFromExcel(xlRange.Cells[i, 11].Value2))
                {
                    profile.Email = xlRange.Cells[i, 11].Value2.ToString();
                }
                else
                {
                    errorColumn = "Email";
                    errorColumnIndex = 11;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Address 12
                if (CheckStringInputFromExcel(xlRange.Cells[i, 12].Value2))
                {
                    profile.Address = xlRange.Cells[i, 12].Value2.ToString();
                }
                else
                {
                    errorColumn = "Address";
                    errorColumnIndex = 12;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Phone 13
                if (CheckStringInputFromExcel(xlRange.Cells[i, 13].Value2))
                {
                    profile.Phone = xlRange.Cells[i, 13].Value2.ToString();
                }
                else
                {
                    errorColumn = "Phone";
                    errorColumnIndex = 13;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Status 14
                if (CheckStringInputFromExcel(xlRange.Cells[i, 14].Value2))
                {
                    profile.Status = xlRange.Cells[i, 14].Value2.ToString();
                }
                else
                {
                    errorColumn = "Status";
                    errorColumnIndex = 14;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Expire Date 15
                if (CheckDateInputFromExcel(xlRange.Cells[i, 15].Value2))
                {
                    var expireDate = ParseDateTimeFormCell(xlRange.Cells[i, 15].Value2.ToString());
                    if (expireDate != null)
                    {
                        profile.DateToLock = expireDate;
                    }
                    else
                    {
                        errorColumn = "Expire Date";
                        errorColumnIndex = 15;
                        errorRowIndex = i;
                        hasError = true;
                        break;
                    }
                }
                else
                {
                    errorColumn = "Expire Date";
                    errorColumnIndex = 15;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Automatic Suspension 16
                if (CheckStringInputFromExcel(xlRange.Cells[i, 16].Value2))
                {
                    bool checkDateToLock = false;
                    if (Boolean.TryParse(xlRange.Cells[i, 16].Value2.ToString(), out checkDateToLock))
                    {
                        profile.CheckDateToLock = checkDateToLock;
                    }
                    else
                    {
                        profile.CheckDateToLock = false;
                    }
                }
                else
                {
                    profile.CheckDateToLock = false;
                }
                //License Plate 17
                if (CheckStringInputFromExcel(xlRange.Cells[i, 17].Value2))
                {
                    profile.LicensePlate = xlRange.Cells[i, 17].Value2.ToString();
                }
                else
                {
                    errorColumn = "License Plate";
                    errorColumnIndex = 17;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Date Created 18
                if (CheckDateInputFromExcel(xlRange.Cells[i, 18].Value2))
                {
                    if (addProfilesChecked)
                    {
                        profile.DateCreated = DateTime.Now;
                    }
                    else
                    {
                        var dateCreated = ParseDateTimeFormCell(xlRange.Cells[i, 18].Value2.ToString());
                        if (dateCreated != null)
                        {
                            profile.DateCreated = dateCreated;
                        }
                        else
                        {
                            errorColumn = "Date Created";
                            errorColumnIndex = 18;
                            errorRowIndex = i;
                            hasError = true;
                            break;
                        }
                    }
                }
                else
                {
                    errorColumn = "Date Created";
                    errorColumnIndex = 18;
                    errorRowIndex = i;
                    hasError = true;
                    break;
                }
                //Date Modified 19
                profile.DateCreated = DateTime.Now;

                listProfiles.Add(profile);

                //Finish process
                if (importBackGroundWorker.CancellationPending)
                {
                    ImportStatus = "Stopped";
                    return;
                }
                (sender as BackgroundWorker).ReportProgress((i * 100) / rowCount);
            }

            if (hasError)
            {
                string errorMessage = $"Invalid data in Row:{errorRowIndex}, Column:{errorColumn}(index:{errorColumnIndex})";
                System.Windows.Forms.MessageBox.Show(errorMessage, "Invalid Data!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (listProfiles.Count > 0)
            {
                ImportStatus = "Importing";
                repo.AddProfiles(listProfiles);
                ImportStatus = "Finished";
            }
        }

        private DateTime? ParseDateTimeFormCell(string sDate)
        {
            DateTime? ngayThang = null;
            double dateD;
            try
            {
                ngayThang = DateTime.ParseExact(sDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {
                try
                {
                    dateD = double.Parse(sDate);
                    var dateTime = DateTime.FromOADate(dateD).ToString("MMMM dd, yyyy");
                    ngayThang = DateTime.Parse(dateTime);
                }
                catch { }
            }
            return ngayThang;
        }

        private void OnSelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Browse Excel Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "Excel",
                Filter = "All Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImportFilePath = openFileDialog.FileName;
                importFileFolder = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
            }
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private bool CheckStringInputFromExcel(object value)
        {
            //Console.WriteLine("====CheckString====");
            if (value != null)
            {
                string str = value.ToString();
                bool notNullOrWhiteSpace = !string.IsNullOrWhiteSpace(value.ToString());
                //Console.WriteLine("str: " + str);
                //Console.WriteLine("notNullOrWhiteSpace: " + notNullOrWhiteSpace);
                if (notNullOrWhiteSpace)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Value is null.");
                return false;
            }
        }

        private bool CheckDateInputFromExcel(object value)
        {
            //Console.WriteLine("====CheckDouble====");
            if (value != null)
            {
                string str = value.ToString();
                bool notNullOrWhiteSpace = !string.IsNullOrWhiteSpace(value.ToString());
                //Console.WriteLine("str: " + str);
                //Console.WriteLine("notNullOrWhiteSpace: " + notNullOrWhiteSpace);
                if (notNullOrWhiteSpace)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Value is null.");
                return false;
            }
        }

        private bool CheckNumberInputFromExcel(object value)
        {
            //Console.WriteLine("====CheckDouble====");
            if (value != null)
            {
                string str = value.ToString();
                bool isDigitsOnly = IsDigitsOnly(value.ToString());
                bool notNullOrWhiteSpace = !string.IsNullOrWhiteSpace(value.ToString());
                //Console.WriteLine("str: " + str);
                //Console.WriteLine("isDigitsOnly: " + isDigitsOnly);
                //Console.WriteLine("notNullOrWhiteSpace: " + notNullOrWhiteSpace);
                if (isDigitsOnly && notNullOrWhiteSpace)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Value is null.");
                return false;
            }
        }

        private int CheckClassNameValid(string className)
        {
            var @class = allClasses.FirstOrDefault(c => c.Name == className);
            if (@class != null)
            {
                return @class.Id;
            }
            else
            {
                return 0;
            }
        }

        private int CheckGroupNameValid(string groupName)
        {
            var group = allGroups.FirstOrDefault(g => g.Name == groupName);
            if (group != null)
            {
                return group.Id;
            }
            else
            {
                return 0;
            }
        }

        private void OnCancel()
        {
            Done();
        }

        public void LoadData()
        {
            Console.WriteLine("ImportProfilesView load data.");
            //if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            //{
            //    return;
            //}
            allClasses = repo.GetClasses().ToList();
            allGroups = repo.GetGroups().ToList();
        }

        #endregion Methods

        #region Commands

        public RelayCommand SelectFileCommand { get; private set; }
        public RelayCommand StartImportCommand { get; private set; }
        public RelayCommand StopImportCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        #endregion Commands

        #region Actions

        public event Action Done = delegate { };

        #endregion Actions

        #region Properties

        public bool IsBackGroundWorkerBusy
        {
            get { return isBackGroundWorkerBusy; }
            set
            {
                SetProperty(ref isBackGroundWorkerBusy, value);
            }
        }

        public bool AddProfilesChecked
        {
            get { return addProfilesChecked; }
            set { SetProperty(ref addProfilesChecked, value); }
        }

        public string ImportProcess
        {
            get { return importProcess; }
            set { SetProperty(ref importProcess, value); }
        }

        public int ImportProcessValue
        {
            get { return importProcessValue; }
            set
            {
                SetProperty(ref importProcessValue, value);
                ImportProcess = importProcessValue + "%";
            }
        }

        public string ImportStatus
        {
            get { return importStatus; }
            set { SetProperty(ref importStatus, value); }
        }

        public string ImportFilePath
        {
            get { return importFilePath; }
            set { SetProperty(ref importFilePath, value); }
        }

        #endregion Properties
    }
}