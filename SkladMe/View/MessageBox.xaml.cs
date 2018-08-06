using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using SkladMe.Infrastructure;

namespace SkladMe.View
{
    public partial class MessageBox
    {
        public MessageBox()
        {
            InitializeComponent();
            if (Application.Current.MainWindow is MainWindow)
            {
                Owner = Application.Current.MainWindow;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        void AddButtons(MessageBoxButton buttons)
        {
            switch (buttons)
            {
                case MessageBoxButton.OK:
                    AddButton("OK", MessageBoxResult.OK);
                    break;
                case MessageBoxButton.OKCancel:
                    AddButton("OK", MessageBoxResult.OK);
                    AddButton("Отмена", MessageBoxResult.Cancel, isCancel: true);
                    break;
                case MessageBoxButton.YesNo:
                    AddButton("Да", MessageBoxResult.Yes);
                    AddButton("Нет", MessageBoxResult.No);
                    break;
                case MessageBoxButton.YesNoCancel:
                    AddButton("Да", MessageBoxResult.Yes);
                    AddButton("Нет", MessageBoxResult.No);
                    AddButton("Отмена", MessageBoxResult.Cancel, isCancel: true);
                    break;
                default:
                    throw new ArgumentException("Unknown button value", "buttons");
            }
        }

        void AddButton(string text, MessageBoxResult result, bool isCancel = false)
        {
            var button = new Button() {Content = text, IsCancel = isCancel};
            button.Click += (o, args) =>
            {
                Result = result;
                DialogResult = true;
            };
            ButtonContainer.Children.Add(button);
        }

        void AddImage(MessageBoxImage image)
        {
            Uri uriSource = null;

            switch (image)
            {
                case MessageBoxImage.None:
                    break;
                case MessageBoxImage.Error:
                    uriSource = new Uri(@"../Resources/Icons/MessageBox/error.png", UriKind.Relative);
                    break;
                case MessageBoxImage.Question:
                    uriSource = new Uri(@"../Resources/Icons/MessageBox/question.png", UriKind.Relative);
                    break;
                case MessageBoxImage.Warning:
                    uriSource = new Uri(@"../Resources/Icons/MessageBox/warning.png", UriKind.Relative);
                    break;
                case MessageBoxImage.Information:
                    uriSource = new Uri(@"../Resources/Icons/MessageBox/information.png", UriKind.Relative);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(image), image, null);
            }

            if (uriSource != null) ImageContainer.Source = new BitmapImage(uriSource);
        }

        MessageBoxResult Result = MessageBoxResult.None;

        public static async Task<MessageBoxResult> ShowAsync(string message, string caption,
            MessageBoxButton buttons, MessageBoxImage image = MessageBoxImage.None)
        {
            var result = MessageBoxResult.None;
             await AsyncHelper.ExecuteAtUI(() =>
            {
                result = Show(message, caption, buttons, image);
            }).ConfigureAwait(false);

            return result;
        }

        public static MessageBoxResult Show(string message, string caption,
            MessageBoxButton buttons, MessageBoxImage image = MessageBoxImage.None)
        {
            var dialog = new MessageBox
            {
                Title = caption,
                MessageContainer = { Text = message }
            };
            dialog.AddButtons(buttons);
            dialog.AddImage(image);

            dialog.ShowDialog();

            return dialog.Result;
        }
    }
}