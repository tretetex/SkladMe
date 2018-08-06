using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SkDAL;
using SkladMe.ViewModel;

namespace SkladMe.Controls
{
    /// <summary>
    /// Логика взаимодействия для TextNotes.xaml
    /// </summary>
    public partial class TextNotes
    {
        public IEnumerable<ProductVM> ItemsSource
        {
            get { return (IEnumerable<ProductVM>) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<ProductVM>), typeof(TextNotes));

        public TextNotes()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Activated += OnActivated;
        }

        private void OnActivated(object sender, EventArgs eventArgs)
        {
            var list = ItemsSource?.ToList();
            if (list?.Count > 0)
            {
                string note = list.ElementAt(0).Model.Note;
                bool notesIdentical = true;
                for (int i = 1; i < list.Count; i++)
                {
                    if (list.ElementAt(i).Model.Note != note)
                    {
                        notesIdentical = false;
                        break;
                    }
                }
                if (notesIdentical)
                {
                    tbNote.Text = note;
                }
            }
            SetFocusToEndText();
            this.Activated -= OnActivated;
        }

        private void SetFocusToEndText()
        {
            tbNote.CaretIndex = tbNote.Text.Length;
            tbNote.ScrollToEnd();
            tbNote.Focus();
        }

        private async void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            foreach (ProductVM productVm in ItemsSource)
            {
                productVm.Note = tbNote.Text == String.Empty ? null : tbNote.Text.Trim();
            }
            
            var task = Db.Products.UpdateNoteAsync(ItemsSource.ToList().Select(p => p.Model).ToList());
            Close();
            await task.ConfigureAwait(false);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}