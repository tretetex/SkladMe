using System;
using System.Windows;
using CefSharp;
using NLog;
using SkladMe.API.Utils;
using SkladMe.Infrastructure;
using SkladMe.ViewModel;

namespace SkladMe
{
    public partial class App : Application
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static MainVM MainVM { get; private set; }

        [STAThread]
        static void Main()
        {
#if DEBUG
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
#endif
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                _logger.Error(args.ExceptionObject);
                View.MessageBox.Show("Во время выполнения программы возникла ошибка.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            };

            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        public App()
        {
            var settings = new CefSettings { UserAgent = Web.UserAgent };
            Cef.Initialize(settings);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var splashscreen = new SplashScreen("/Resources/splashscreen.jpg");
            splashscreen.Show(true);

            MainVM = new MainVM();

            base.OnStartup(e);
            var mainWindow = new MainWindow {DataContext = MainVM};
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
        }
    }
}