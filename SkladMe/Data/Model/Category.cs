using System.Collections.Generic;
using SkDAL.Base;

namespace SkDAL.Model
{
    public class Category : BaseModel
    {
        public string Title { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public bool IsFixed { get; set; }
        public bool IsReadonly { get; set; }
        public int SortOrder { get; set; }

        public int? ParentId { get; set; }
        public Category Parent { get; set; }

        public ICollection<Category> Children { get; set; }
        public ICollection<ProductCategories> ProductCategories { get; set; }
    }
}
