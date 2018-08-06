using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SkDAL;
using SkDAL.Model;
using SkladMe.Controls;
using SkladMe.Infrastructure;

namespace SkladMe.ViewModel
{
    public class ColorManagerVM
    {
        private ICommand _editColorsCommand;

        public ColorManagerVM()
        {
            GetColorsFromDb().Wait();
        }

        #region properties

        public ObservableCollection<ColorVM> Colors { get; } = new ObservableCollection<ColorVM>()
        {
            new ColorVM(new Color() {Title = "Green", Value = "#a0db8e"}),
            new ColorVM(new Color()  {Title = "Red", Value = "#b44545"}),
            new ColorVM(new Color()  {Title = "Orange", Value = "#e28356"}),
            new ColorVM(new Color()  {Title = "Blue", Value = "#2b90f5"})
        };
        
        #endregion

        public ICommand EditColorsCommand => _editColorsCommand ?? (_editColorsCommand = new RelayCommand(EditColors));
       
        private async Task GetColorsFromDb()
        {
            var dbColors = await Db.Colors.AllAsync().ConfigureAwait(false);
            if (dbColors.Count <= 0) return;
            Colors.Clear();
            foreach (var dbColor in dbColors)
            {
                var colorVm = new ColorVM(dbColor);
                Colors.Add(colorVm);
            }
        }
        
        private void EditColors()
        {
            var cn = new ColorNotes
            {
                ItemsSource = Colors
            };
            cn.ShowDialog();
        }


    }
}