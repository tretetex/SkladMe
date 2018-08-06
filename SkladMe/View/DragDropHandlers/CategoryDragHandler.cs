using GongSolutions.Wpf.DragDrop;
using DragDrop = GongSolutions.Wpf.DragDrop.DragDrop;

namespace SkladMe.Controls.DragDropHandlers
{
    public class CategoryDragHandler : IDragSource
    {
        public void StartDrag(IDragInfo dragInfo)
        {
            DragDrop.DefaultDragHandler.StartDrag(dragInfo);
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
           return DropEventArgs.IsCategory(dragInfo.SourceItem) || DropEventArgs.IsProduct(dragInfo.SourceItem);
        }

        public void Dropped(IDropInfo dropInfo)
        {

        }

        public void DragCancelled()
        {

        }

        public bool TryCatchOccurredException(System.Exception exception)
        {
            return false;
        }
    }
}
