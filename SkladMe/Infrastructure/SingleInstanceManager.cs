using System.Windows;
using Microsoft.VisualBasic.ApplicationServices;

namespace SkladMe.Infrastructure
{
    public class SingleInstanceManager : WindowsFormsApplicationBase
    {
        Application _app;

        public SingleInstanceManager(Application app)
        {
            _app = app; 
            IsSingleInstance = true;
        }

        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e)
        {
            //_app = new App();
            _app.Run();
            return false;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            base.OnStartupNextInstance(eventArgs);
            _app.MainWindow.WindowState = WindowState.Normal;
            _app.MainWindow.Activate();
        }
    }
}
