using System;
using System.Windows;
using NLog;

namespace Launcher
{
    public partial class App : Application
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        public static void Main()
        {
            var app = new App();
            app.DispatcherUnhandledException += (sender, args) =>
            {
                _logger.Error(args.Exception);
                MessageBox.Show(args.Exception.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            };
            app.InitializeComponent();
            app.Run();
        }

        public MainVm MainViewModel { get; } = new MainVm();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var mainWindow = new MainWindow { DataContext = MainViewModel };
            mainWindow.Show();
            MainViewModel.StartUpdate();
        }
    }
}
