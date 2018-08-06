using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using SkladMe.Infrastructure;

namespace SkladMe.ViewModel
{
    public class FiltersManagerVM : BaseVM
    {
        private ICommand _resetFilterSettingsCommand;
        private ICommand _saveFilterSettingsCommand;
        private ICommand _loadFilterSettingsCommand;
        private ICommand _reloadFiltersCommand;
        private ICommand _addFilterCommand;

        private FiltersVM _filtersVm;
        private string _path;

        public static string TemporaryFiltersPath = Environment.CurrentDirectory + "\\filters.temp";
        public static string TemporaryCategoryFiltersPath = Environment.CurrentDirectory + "\\filterscat.temp";
        public static string FiltersDir = Environment.CurrentDirectory + "\\filters\\";

        private bool _isFilterSetted;
        private static bool _isFiltersLoaded;
        
        public FiltersManagerVM(string path)
        {
            _path = path;
            try
            {
                if (!_isFiltersLoaded)
                {
                    LoadAllFilters();
                }
                else
                {
                    FiltersVM = Filters[0];
                }
            }
            catch(System.Exception e)
            {
                if (FiltersVM == null)
                {
                    FiltersVM = new FiltersVM(true);
                    FiltersVM.Title = "Новый";
                    Filters.Add(FiltersVM);
                }
            }
        }
        
        #region properties

        public static ObservableCollection<FiltersVM> Filters { get; } = new ObservableCollection<FiltersVM>();

        public FiltersVM FiltersVM
        {
            get { return _filtersVm; }
            set
            {
                _filtersVm = value;
                OnPropertyChanged();
            }
        }

        public bool IsFilterSetted
        {
            get { return _isFilterSetted; }
            set
            {
                _isFilterSetted = value; 
                OnPropertyChanged();
            }
        }

        #endregion

        #region commands

        public ICommand ResetSettingsCommand
            => _resetFilterSettingsCommand ?? (_resetFilterSettingsCommand = new RelayCommand(ResetFilterSettings));

        public ICommand SaveSettingsCommand
            => _saveFilterSettingsCommand ?? (_saveFilterSettingsCommand = new RelayCommand(SaveFilterSettings));

        public ICommand LoadSettingsCommand
            => _loadFilterSettingsCommand ?? (_loadFilterSettingsCommand = new RelayCommand(LoadFilterSettings));

        public ICommand ReloadFiltersCommand
            => _reloadFiltersCommand ?? (_reloadFiltersCommand = new RelayCommand(LoadAllFilters));

        public ICommand AddFilterCommand
            => _addFilterCommand ?? (_addFilterCommand = new RelayCommand(AddFilter));

        #endregion commands

        #region methods
            
        public void ResetFilterSettings()
        {
            FiltersVM = new FiltersVM(true);
        }

        private void SaveFilterSettings()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Фильтры|*.skfilter",
                Title = "Сохранение фильтров",
                InitialDirectory = FiltersDir
            };
            saveFileDialog.ShowDialog();
            var path = saveFileDialog.FileName;

            if (!string.IsNullOrWhiteSpace(path))
            {
                SaveFilters(path);
                FiltersVM.IsSaved = true;
            }
        }

        public bool SaveFilters(string path)
        {
            try
            {
                Serialization.SerializeToXml(path, FiltersVM);

                if (FiltersVM.Path == TemporaryCategoryFiltersPath || FiltersVM.Path == TemporaryFiltersPath)
                {
                    FiltersVM.Title = "Новый";
                }
                else if(path != TemporaryCategoryFiltersPath && path != TemporaryFiltersPath)
                {
                    FiltersVM.Title = Path.GetFileNameWithoutExtension(path);
                    FiltersVM.Path = path;
                    FiltersVM.IsSaved = true;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async void LoadFilterSettings()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Фильтры|*.skfilter",
                Title = "Выберите файл фильтров",
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            openFileDialog.ShowDialog();

            var path = openFileDialog.FileName;

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            var filter = Filters.FirstOrDefault(f => f.Path == path);
            if (filter != null)
            {
                FiltersVM = filter;
                return;
            }

            try
            {
                LoadingFilters(path);
            }
            catch (InvalidOperationException)
            {
                await View.MessageBox.ShowAsync("Загрузка фильтров не удалась.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error).ConfigureAwait(false);
            }
        }

        private void LoadAllFilters()
        {
            Filters.Clear();
            if (Directory.Exists(FiltersDir))
            {
                var filters = Directory.GetFiles(FiltersDir, "*.skfilter", SearchOption.TopDirectoryOnly);

                foreach (var f in filters)
                {
                    try
                    {
                        var fvm = LoadingFilters(f);
                        fvm.IsSaved = true;
                        Filters.Add(fvm);
                    }
                    catch(System.Exception e)
                    {
                    }
                }

            }
            else
            {
                Directory.CreateDirectory(FiltersDir);
            }

            if (Filters.Count == 0)
            {
                FiltersVM = LoadingFilters(_path);
                FiltersVM.Title = "Новый";
                Filters.Add(FiltersVM);
            }
            else
            {
                FiltersVM = Filters[0];
            }

            _isFiltersLoaded = true;
        }

        public FiltersVM LoadingFilters(string path)
        {
            var filters = (FiltersVM)Serialization.DeserializeFromXml(path, typeof(FiltersVM));

            var vm = new FiltersVM(false);
            
            filters.TitleKeywordIncluded.Model = filters.Model.TitleKeywordIncluded;
            filters.TitleKeywordExcluded.Model = filters.Model.TitleKeywordExcluded;
            filters.TagsIncluded.Model = filters.Model.TagsIncluded;
            filters.TagsExcluded.Model = filters.Model.TagsExcluded;

            filters.Model.TagsIncluded = filters.TagsIncluded.ToList();
            filters.Model.TagsExcluded = filters.TagsExcluded.ToList();
            filters.Model.TitleKeywordIncluded = filters.TitleKeywordIncluded.ToList();
            filters.Model.TitleKeywordExcluded = filters.TitleKeywordExcluded.ToList();

            vm = filters;
            vm.Model = filters.Model;
            
            if (path == TemporaryCategoryFiltersPath || path == TemporaryFiltersPath)
            {
                vm.Title = "Новый";
            }
            else
            {
                vm.Title = Path.GetFileNameWithoutExtension(path);
                vm.Path = path;
            }
            
            vm.IsSaved = true;

            return vm;
        }


        private void AddFilter()
        {
            Filters.Add(new FiltersVM(true){Title = "Новый"});
            FiltersVM = Filters.Last();
        }

        #endregion methods
    }
}