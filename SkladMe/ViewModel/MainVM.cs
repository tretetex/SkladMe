using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SkladMe.Infrastructure;
using SkladMe.Infrastructure.EnhancedwDataGridExample;
using SkladMe.Properties;
using SkladMe.View;
using SkServ;
using SkServ.Model;
using SkServ.Model.RequestParams;
using MessageBox = SkladMe.View.MessageBox;

namespace SkladMe.ViewModel
{
    public class MainVM : BaseVM
    {
        private NoticeWindow _noticeWindow = new NoticeWindow();
        private ICommand _closeWindowCommand;
        private ObservableCollection<ColumnInfo> _columnInfo = new ObservableCollection<ColumnInfo>();

        public AuthorizationVM AuthorizationVM { get; }
        public SearchVM SearchVM { get; }
        public ColumnVisibilityVM ColumnVisibilityVM { get; private set; }
        public CategoryManagerVM CategoryManagerVM { get; }
        public ColorManagerVM ColorManagerVM { get; }

        public MainVM()
        {
            SetClosingNotification();
            AuthorizationVM = new AuthorizationVM();
            SearchVM = new SearchVM();
            CategoryManagerVM = new CategoryManagerVM(SearchVM.Products);
            ColorManagerVM = new ColorManagerVM();
            SetColumnVisibility();
            LoadColumnsSize();
        }

        public ObservableCollection<ColumnInfo> ColumnInfo
        {
            get => _columnInfo;
            set
            {
                _columnInfo = value;
                OnPropertyChanged();
            }
        }

        private void SetColumnVisibility()
        {
            try
            {
                ColumnVisibilityVM = ColumnVisibilityVM.LoadingVisibility();
            }
            catch
            {
                if (ColumnVisibilityVM == null)
                {
                    ColumnVisibilityVM = new ColumnVisibilityVM();
                }
            }
        }


        public ICommand CloseWindowCommand
            => _closeWindowCommand ?? (_closeWindowCommand = new RelayCommand(CloseWindow));

        public void ColumnsSizeChangedHandle(ObservableCollection<ColumnInfo> info) => ColumnInfo = info;
        
        private void CloseWindow()
        {
            SaveColumnsSize();
            SaveOpenedCategories();
            SaveExpandedCategoriesId();
            var isNoticeExist = ShowNotification();
            SearchVM.FiltersManagerVM.SaveFilters(FiltersManagerVM.TemporaryFiltersPath);
            ColumnVisibilityVM.SaveTempVisibility();
            if(!isNoticeExist) Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
        }

        private static string _columnSizeTempPath = Environment.CurrentDirectory + "\\columnssize.temp";

        private void SaveColumnsSize()
        {
            Serialization.SerializeToXml(_columnSizeTempPath, ColumnInfo);
        }

        private void LoadColumnsSize()
        {
            try
            {
                ColumnInfo = (ObservableCollection<ColumnInfo>)Serialization.DeserializeFromXml(_columnSizeTempPath, typeof(ObservableCollection<ColumnInfo>));
            }
            catch
            {
                
            }
        }

        public const char SettingsDelimiter = ';';

        private void SaveOpenedCategories()
        {
            var ids = new List<int>();

            foreach (var category in CategoryManagerVM.OpenCategories)
            {
                ids.Add(category.Model.Id);
            }

            Settings.Default.OpenedCategories = string.Join(SettingsDelimiter.ToString(), ids.Select(i => i.ToString()).ToArray());
            Settings.Default.SelectedCategoryId = CategoryManagerVM.SelectedCategory?.Model.Id.ToString();
            Settings.Default.Save();
        }

        private void SaveExpandedCategoriesId()
        {
            var ids = new List<int>();

            void ProcessChild(CategoryVM category)
            {
                if (category.IsExpanded)
                {
                    ids.Add(category.Model.Id);
                }

                foreach (var childCategory in category.ChildCategories)
                {
                    ProcessChild(childCategory);
                }
            }
            foreach (var category in CategoryManagerVM.Categories)
            {
                ProcessChild(category);
            }

            string value = string.Join(SettingsDelimiter.ToString(), ids.Select(i => i.ToString()).ToArray());
            Settings.Default.ExpandedCategories = value;

            Settings.Default.Save();
        }

        private bool ShowNotification()
        {
            if (_noticeWindow?.Image != null)
            {
                _noticeWindow.Show();
                return true;
            }
            return false;
        }

        private void SetClosingNotification()
        {
            List<NoticeInfo> notices;

            try
            {
                notices = new ServerApiClient(new CommonRequestParams()).Notice.Get();
                if (notices.Count <= 0) return;

                int randomIndex = new Random().Next(0, notices.Count);
                var randomNotice = notices[randomIndex];
                var image = LoadNotificationImage(randomNotice.Image);
                _noticeWindow.Image = image;
                _noticeWindow.Link = randomNotice.Link;
            }
            catch (AggregateException ae)
            {
               ae.Handle(ex =>
               {
                   var message = ex.InnerException?.Message ?? ex.Message;
                   MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                   return true;
               });
            }
            catch { }
        }

        private BitmapImage LoadNotificationImage(string url)
        {
            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();
            var bitmap = new Bitmap(responseStream);
            return ConvertBitmapToBitmapImage(bitmap);
        }

        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}