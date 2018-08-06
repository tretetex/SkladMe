using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SkladMe.API;
using SkladMe.ViewModel;

namespace SkladMe.Infrastructure.Collections
{
    public class ChapterVM : BaseVM, ICloneable
    {
        private bool _isSelected;

        public ChapterVM()
        {
            _isSelected = true;
        }

        public event EventHandler SelectionChanged;

        public int Id { get; set; }

        public string Name { get; set; }

        public string Url => SkladchikApiClient.ForumsUrl + Id;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnSelectionChanged();
                OnPropertyChanged();
            }
        }

        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public sealed class ChapterCollection : ObservableCollection<ChapterVM>, IXmlSerializable
    {
        private bool? _isSelectedAll;
        private EventHandler _selectionChangedEventHandler;

        public ChapterCollection()
        {
            CollectionChanged += OnCollectionChanged;
            _selectionChangedEventHandler = (o, args) => SetIsAllSelected();
        }

        public bool ShouldSerializeIsSelectedAll => IsSelectedAll.HasValue;

        public bool? IsSelectedAll
        {
            get { return _isSelectedAll; }
            set
            {
                if (_isSelectedAll == value) return;
                _isSelectedAll = value;
                SetSelectionToAll();
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsSelectedAll)));
            }
        }

        private void SetSelectionToAll()
        {
            if (!IsSelectedAll.HasValue) return;
            foreach (var chapter in this)
            {
                chapter.SelectionChanged -= _selectionChangedEventHandler;
                chapter.IsSelected = IsSelectedAll.Value;
                chapter.SelectionChanged += _selectionChangedEventHandler;
            }
        }

        private void SetIsAllSelected()
        {
            bool isAllSelected = this.All(c => c.IsSelected);
            if (isAllSelected)
            {
                IsSelectedAll = true;
                return;
            }

            bool isAllUnselected = this.All(c => !c.IsSelected);
            if (isAllUnselected)
            {
                IsSelectedAll = false;
                return;
            }
            IsSelectedAll = null;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    ((ChapterVM) newItem).SelectionChanged += _selectionChangedEventHandler;
                }
            }
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            // Читаем из потока дополнительные свойства вручную
            try
            {
                IsSelectedAll = reader.ReadElementContentAsBoolean(nameof(IsSelectedAll), "");
            }
            catch (XmlException)
            {
            }

            // Читаем элементы коллекции
            var ser = new XmlSerializer(typeof(ChapterVM));
            while (reader.NodeType == XmlNodeType.Element)
                Add((ChapterVM) ser.Deserialize(reader));

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            // Сериализуем дополнительные свойства вручную
            if (ShouldSerializeIsSelectedAll)
            {
                writer.WriteStartElement(nameof(IsSelectedAll));
                writer.WriteValue(IsSelectedAll);
                writer.WriteEndElement();
            }
            // Сериализуем элементы коллекции
            var ser = new XmlSerializer(typeof(ChapterVM));
            foreach (var chapter in this)
                ser.Serialize(writer, chapter);
        }
    }
}