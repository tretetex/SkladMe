using System;
using System.Collections;
using System.Linq;
using System.Windows.Input;
using SkDAL;
using SkDAL.Model;
using SkladMe.Infrastructure;

namespace SkladMe.ViewModel
{
    public class ColorVM : BaseVM, ICloneable
    {
        private ICommand _changeProductColorCommandCommand;

        public ColorVM(Color model)
        {
            Model = model;
        }
        public Color Model { get; }

        public string Title
        {
            get { return Model.Title; }
            set
            {
                Model.Title = value;
                OnPropertyChanged();
            }
        }

        public string Value
        {
            get { return Model.Value; }
            set
            {
                Model.Value = value;
                OnPropertyChanged();
            }
        }

        public ICommand ChangeProductColorCommand => _changeProductColorCommandCommand ?? (_changeProductColorCommandCommand = new RelayCommand<IList>(ChangeProductColor));


        private async void ChangeProductColor(IList items)
        {
            var products = items.Cast<ProductVM>().Select(p =>
            {
                p.Color = Value;
                return p.Model;
            }).ToList();

            await AsyncHelper.ProcessCollectionAsync(products, Db.Products.UpdateColorAsync)
                .ConfigureAwait(false);
        }


        public object Clone()
        {
            return MemberwiseClone();
        }

    }
}
