using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GongSolutions.Wpf.DragDrop;
using SkladMe.ViewModel;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

namespace SkladMe.Controls.DragDropHandlers
{
    public enum InsertPlacement
    {
        AfterTarget,
        BeforeTarget,
        IntoTarget
    }
    public class DropEventArgs : EventArgs
    {
        public readonly InsertPlacement SourcePlacement;

        public DropEventArgs(InsertPlacement sourcePlacement)
        {
            SourcePlacement = sourcePlacement;
        }

        public static bool IsProduct(object data)
        {
            return data is ProductVM || data is IEnumerable<ProductVM>;
        }

        public static bool IsCategory(object data)
        {
            return data is CategoryVM category && !category.IsFixed && !category.IsReadonly;
        }

    }
    public class CategoryDropHandler : IDropTarget
    {
        public void DragOver(IDropInfo dropInfo)
        {
            bool isCategory = DropEventArgs.IsCategory(dropInfo.TargetItem);
            bool isProduct = DropEventArgs.IsProduct(dropInfo.Data);

            if (!isCategory && !isProduct)
            {
                dropInfo.Effects = DragDropEffects.None;
            }
            else
            {
                if (isProduct && dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter))
                {
                    var parentCategory = (dropInfo.DragInfo.VisualSource as Control)?.DataContext as CategoryVM;
                    var targetCategory = dropInfo.TargetItem as CategoryVM;
                    if (parentCategory != targetCategory)
                    {
                        dropInfo.Effects = DragDropEffects.Move;
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    }
                }
                DragDrop.DefaultDropHandler.DragOver(dropInfo);
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is CategoryVM)
            {
                DropCategory(dropInfo);
            }
            else if (dropInfo.Data is ProductVM || dropInfo.Data is IEnumerable<ProductVM>)
            {
                DropProduct(dropInfo);
            }
        }

        private void DropCategory(IDropInfo dropInfo)
        {
            var source = dropInfo.Data as CategoryVM;
            var target = dropInfo.TargetItem as CategoryVM;
            var insertPlacement = InsertPlacement.IntoTarget;

            bool isIntoTarget = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter);
            if (isIntoTarget && target != null)
            {
                source.ParentCollection = target.ChildCategories;
            }
            var isAfterTarget =
                dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.AfterTargetItem);

            var isBeforeTarget =
                dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.BeforeTargetItem);

            if (target != null)
            {
                if ((isAfterTarget || isBeforeTarget) && !isIntoTarget)
                {
                    insertPlacement = isAfterTarget ? InsertPlacement.AfterTarget : InsertPlacement.BeforeTarget;
                    source.ParentCollection = target.ParentCollection;
                }
            }
            else
            {
                source.ParentCollection = null;
            }
            DragDrop.DefaultDropHandler.Drop(dropInfo);
            source.Moved.Invoke(target, new DropEventArgs(insertPlacement));
        }

        private async void DropProduct(IDropInfo dropInfo)
        {
            var parentCategory = (dropInfo.DragInfo.VisualSource as Control)?.DataContext as CategoryVM; 
            
            var targetCategory = dropInfo.TargetItem as CategoryVM;
            var products = (dropInfo.Data as IEnumerable<ProductVM>)?.ToList();
            if (products is null)
            {
                var product = dropInfo.Data as ProductVM;
                if (product is null) return;
                products = new List<ProductVM> {product};
            }

            var dg = dropInfo.DragInfo.VisualSource as DataGrid;
            if (dg.DataContext is MainVM mainVm && mainVm.SearchVM.IsStarted)
            {
                await View.MessageBox.ShowAsync("Добавление складчин в категорию во время поиска недоступно.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Information).ConfigureAwait(false);
                return;
            }

            if (dropInfo.KeyStates != DragDropKeyStates.ControlKey)
            {
                if (parentCategory != null)
                {
                    await parentCategory.RemoveProductsAsync(products).ConfigureAwait(false);
                }
                else
                {
                    var itemsSource = dg.ItemsSource as ObservableCollection<ProductVM>;

                    foreach (var productVm in products)
                    {
                        itemsSource.Remove(productVm);
                    }
                }
                
            }
            await ProductManagerVM.AddProductToCategoryAsync(targetCategory, products).ConfigureAwait(false); 
        }
    }
}