using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using SkladMe.Infrastructure;

namespace SkladMe.Models
{
    public sealed class KeywordCollection : ObservableCollection<string>
    {
        private ICommand _removeKeywordCommand;
        private List<string> _model;

        public KeywordCollection()
        {
            CollectionChanged += OnCollectionChanged;
        }

        public List<string> Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnCollectionChanged(null, null);
            }
        }

        public ICommand RemoveKeywordCommand
            => _removeKeywordCommand ?? (_removeKeywordCommand = new RelayCommand<string>(RemoveKeyword));

        private void RemoveKeyword(string keyword)
        {
            Remove(keyword);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Model.Clear();
            Model.AddRange(this);
        }
    }
}
