using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using SkladMe.API.Methods;
using SkladMe.Infrastructure;
using SkServ;
using SkServ.Model;
using SkServ.Model.RequestParams;

namespace SkladMe.ViewModel
{
    public class AuthorizationVM : BaseVM
    {
        private static string _programmLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                                                  "\\.license";

        private string _status;
        private ICommand _loginCommand;
        private bool _isSuccess;
        private bool _inProgress;
        private System.Timers.Timer _timer;
        private DateTime _activationDate;
        private DateTime _expireDate;
        private int _daysLeft;

        public AuthorizationVM()
        {

            _timer = new System.Timers.Timer(300000);
            _timer.Elapsed += (s, e) =>
            {
                Login();
            };

            ReadKeyFromFile();
            Login();
        }

        public static string Key { get; set; }
        public static string HardwareId => GetMd5Hash(HashAlgorithm.Create(), GetHardwareId());

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public bool InProgress
        {
            get => _inProgress;
            set
            {
                _inProgress = value;
                OnPropertyChanged();
            }
        }

        public bool IsSuccess
        {
            get => _isSuccess;
            private set
            {
                _isSuccess = value;
                OnPropertyChanged();
            }
        }

        public DateTime ActivationDate
        {
            get => _activationDate;
            set
            {
                _activationDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime ExpireDate
        {
            get => _expireDate;
            set
            {
                _expireDate = value;
                OnPropertyChanged();
            }
        }

        public int DaysLeft
        {
            get => _daysLeft;
            set
            {
                _daysLeft = value;
                OnPropertyChanged();
            }
        }

        public string CurrentVersion => GetCurrentVersion();

        public string GetCurrentVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version.Major + "." + version.Minor + "." + version.Build;
        }

        public ObservableCollection<UpdateInfo> Updates { get; } = new ObservableCollection<UpdateInfo>();

        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new RelayCommand(Login));

        private void Login()
        {
            if (string.IsNullOrWhiteSpace(Key)) return;
            InProgress = true;
            var requestParams = new CommonRequestParams
            {
                Hw = HardwareId,
                Key = Key
            };

            var api = new ServerApiClient(requestParams);

            var license = api.Auth.SetLicense();
                
            if (license.Code == 1)
            {
                SetLicenseInfo(api);
                SetUpdatesLog(api);

                IsSuccess = true;
                SetXPaths(api);
                SaveKeyToFile();
                _timer.Start();
            }
            else
            {
                IsSuccess = false;
                Status = license.Message + "\n" + license.BannedInfo;
                _timer.Stop();
            }
            InProgress = false;
        }

        private void SetLicenseInfo(ServerApiClient api)
        {
            var licenseInfo = api.Auth.CheckLicense();
            if (licenseInfo.Expires.HasValue)
            {
                ExpireDate = licenseInfo.Expires.Value;
            }
            if (licenseInfo.ActiveFrom.HasValue)
            {
                ActivationDate = licenseInfo.ActiveFrom.Value;
            }
            DaysLeft = (ExpireDate - DateTime.Today).Days;
        }

        private void SetUpdatesLog(ServerApiClient api)
        {
            if (Updates.Count > 0) return;
            var updates = api.App.GetAllUpdates().OrderByDescending(info => info.Version).ToList();
            foreach (var updateInfo in updates)
            {
                Updates.Add(updateInfo);
            }
        }

        private static void SetXPaths(ServerApiClient api)
        {
            if (Products.XPaths == null || Products.XPaths.Count == 0)
            {
                var data = api.Data.Get();
                Products.XPaths = data.Products;
                Chapters.XPaths = data.Chapters;
            }
        }

        private static string GetOs()
        {
            string str = "";
            var searcher = new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM CIM_OperatingSystem");

            foreach (var queryObj in searcher.Get())
            {
                try
                {
                    str = queryObj["SerialNumber"].ToString();
                }
                catch
                {
                    // ignored
                }
            }
            return str;
        }

        private static string GetMotherBoardId()
        {
            string str = "";
            var searcher = new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM CIM_Card");
            foreach (var queryObj in searcher.Get())
            {
                try
                {
                    str = queryObj["SerialNumber"].ToString();
                }
                catch
                {
                }
            }


            return str;
        }

        private static string GetProcessId()
        {
            string str = "";
            var searcher =
                new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_Processor");
            foreach (var queryObj in searcher.Get())
            {
                try
                {
                    str = queryObj["ProcessorId"].ToString();
                }
                catch
                {
                    //ignore
                }
            }

            return str;
        }

        private static string GetHardwareId()
        {
            var os = GetOs();
            var motherboard = GetMotherBoardId();
            var processId = GetProcessId();

            return os + motherboard + processId;
        }

        static string GetMd5Hash(HashAlgorithm md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            foreach (byte t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }


            return sBuilder.ToString();
        }

        private void SaveKeyToFile()
        {
            File.WriteAllText(_programmLocation, Key);
        }

        private void ReadKeyFromFile()
        {
            if (File.Exists(_programmLocation))
            {
                Key = File.ReadAllText(_programmLocation).Trim();
            }
        }
    }
}