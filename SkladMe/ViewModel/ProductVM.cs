using System;
using SkDAL.Model;

namespace SkladMe.ViewModel
{
    public class ProductVM : BaseVM
    {
        private Product _model;
        private double? _feeToPrice;
        private double _involvement;

        public ProductVM(Product p)
        {
            Model = p;
        }

        public Product Model
        {
            get => _model;
            set
            {
                _model = value;
                UpdateFeeToPrice();
                UpdateInvolvement();
                OnPropertyChanged();
            }
        }

        public double? FeeToPrice
        {
            get => _feeToPrice;
            set
            {
                _feeToPrice = value;
                OnPropertyChanged();
            }
        }

        public double Involvement
        {
            get => _involvement;
            set
            {
                _involvement = value;
                OnPropertyChanged();
            }
        }

        public string Color
        {
            get => Model.Color;
            set
            {
                Model.Color = value;
                OnPropertyChanged();
            }
        }

        public string Note
        {
            get => Model.Note;
            set
            {
                Model.Note = value;
                OnPropertyChanged();
            }
        }

        private void UpdateInvolvement()
        {
            if (_model.ViewCount == 0)
            {
                Involvement = 0;
            }
            else
            {
                Involvement = Math.Round((double)(_model.MembersAsMainCount + _model.MembersAsReserveCount) / _model.ViewCount * 100, 3);
            }
        }


        private void UpdateFeeToPrice()
        {
            if (Model.Price.HasValue && Model.Fee.HasValue && Model.Price != 0)
            {
                FeeToPrice = (double) Model.Fee / Model.Price;
            }
        }
    }
}