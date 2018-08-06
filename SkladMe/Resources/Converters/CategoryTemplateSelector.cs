using System.Windows;
using System.Windows.Controls;
using SkladMe.ViewModel;

namespace SkladMe.Resources.Converters
{
    class CategoryTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CategoryTemplate { get; set; }
        public DataTemplate SearchTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is CategoryVM) return CategoryTemplate;
            return SearchTemplate;
        }
    }
}