using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Launcher.Annotations;
using NUnrar.Archive;
using SkServ;
using SkServ.Model.RequestParams;

namespace Launcher
{
    public class MainVm : INotifyPropertyChanged
    {
        private string _status;
        private HttpClient _httpClient = new HttpClient();

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public async void StartUpdate()
        {
            if (!Process.GetProcessesByName("SkladMe").Any())
            {
                Status = "Проверка доступности сервера";
                await CheckServerConnection().ConfigureAwait(false);
                Status = "Получение информации от сервера";
                CheckUpdate();
            }

            Status = "Запуск программы...";
            StartProgramm();
        }

        private async Task CheckServerConnection()
        {
            bool isServerAvailable = false;
            while (!isServerAvailable)
            {
                try
                {
                    await _httpClient.GetAsync(ServerApiClient.Host).ConfigureAwait(false);
                    isServerAvailable = true;
                }
                catch
                {
                    
                }
            }
        }

        private void CheckUpdate()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var api = new ServerApiClient(new CommonRequestParams());

            var updateInfo = api.App.CheckUpdate(version);
            if (updateInfo == null) return;
            string link = updateInfo.Link;
            DownloadAndUnrarUpdate(link);
        }

        private void DownloadAndUnrarUpdate(string link)
        {
            string programmLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string rarName = "update.rar";
            TryDeleteFile(programmLocation + "\\" + rarName);

            RemoveFiles();

            Status = "Загрузка обновления";
            var client = new WebClient();
            client.DownloadFile(new Uri(link), rarName);

            Status = "Копирование файлов";

            RenameUndeletedFiles(programmLocation);
            RarArchive.WriteToDirectory(rarName, programmLocation,
            NUnrar.Common.ExtractOptions.ExtractFullPath | NUnrar.Common.ExtractOptions.Overwrite);
            TryDeleteFile(programmLocation + "\\" + rarName);
        }
            
        private void RenameUndeletedFiles(string programmLocation)
        {
            File.Move(programmLocation + "\\Launcher.exe", programmLocation + "\\Launcher.exe.bak");
            File.Move(programmLocation + "\\SkDAL.dll", programmLocation + "\\SkDAL.dll.bak");
            File.Move(programmLocation + "\\SkServ.dll", programmLocation + "\\SkServ.dll.bak");
            File.Move(programmLocation + "\\SkladMe.Common.dll", programmLocation + "\\SkladMe.Common.dll.bak");
            File.Move(programmLocation + "\\SkladMe.API.dll", programmLocation + "\\SkladMe.API.dll.bak");

            File.Move(programmLocation + "\\Newtonsoft.Json.dll", programmLocation + "\\Newtonsoft.Json.dll.bak");
            File.Move(programmLocation + "\\NLog.dll", programmLocation + "\\NLog.dll.bak");
            File.Move(programmLocation + "\\NUnrar.dll", programmLocation + "\\NUnrar.dll.bak");
            File.Move(programmLocation + "\\d3dcompiler_47.dll", programmLocation + "\\d3dcompiler_47.dll.bak");
        }

        private void RemoveFiles()
        {
            string programmLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            TryDeleteFile(programmLocation + "\\Launcher.exe.bak");
            TryDeleteFile(programmLocation + "\\SkDAL.dll.bak");
            TryDeleteFile(programmLocation + "\\SkServ.dll.bak");
            TryDeleteFile(programmLocation + "\\SkladMe.Common.dll.bak");
            TryDeleteFile(programmLocation + "\\SkladMe.API.dll.bak");

            TryDeleteFile(programmLocation + "\\Newtonsoft.Json.dll.bak");
            TryDeleteFile(programmLocation + "\\NLog.dll.bak");
            TryDeleteFile(programmLocation + "\\NUnrar.dll.bak");
            TryDeleteFile(programmLocation + "\\d3dcompiler_47.dll.bak");
        }

        private void TryDeleteFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            while (true)
            {
                try
                {
                    using (File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        break;
                    }
                }
                catch (IOException)
                {
                }
            }
            File.Delete(filePath);
        }

        private void StartProgramm()
        {
            var processName = "SkladMe.exe";
            var startInfo = new ProcessStartInfo
            {
                FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + processName,
                Arguments = DateTime.Now.ToShortDateString()
            };
            Process.Start(startInfo);
            Application.Current.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
        }
        
        #region propertyChanged event

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}