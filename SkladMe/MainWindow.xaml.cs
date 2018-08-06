using System;
using System.ComponentModel;
using System.Windows;
using CefSharp;
using MahApps.Metro.Controls;
using SkladMe.Infrastructure;
using SkladMe.Properties;

namespace SkladMe
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if(string.IsNullOrWhiteSpace(Settings.Default.SelectedCategoryId))
                    TabControlAbove.SelectedIndex = 0;
            };

            // Set window location
            Left = Settings.Default.WindowLocation.X;
            Top = Settings.Default.WindowLocation.Y;

            // Set window size
            Height = Settings.Default.WindowSize.X;
            Width = Settings.Default.WindowSize.Y;

            if (Settings.Default.WindowMaximized)
            {
                WindowState = WindowState.Maximized;
            }

            CategoriesColumn.Width = new GridLength(Settings.Default.CategoryColumnWidth, GridUnitType.Pixel);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (CriticalTasks.Count > 0)
            {
                var result = MessageBox.Show("Не все задачи завершены. Вы действительно хотите закрыть приложение?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
            }

            Settings.Default.WindowLocation = new System.Drawing.Point(Convert.ToInt32(Left), Convert.ToInt32(Top));
            Settings.Default.WindowSize = new System.Drawing.Point(Convert.ToInt32(Height), Convert.ToInt32(Width));
            Settings.Default.WindowMaximized = WindowState == WindowState.Maximized;

            Settings.Default.CategoryColumnWidth = CategoriesColumn.Width.Value;
            Settings.Default.Save();

            Cef.Shutdown();
            base.OnClosing(e);
        }


    }
}