using System.Collections.Generic;
using SkDAL.EFUtilities.Model;

namespace SkDAL.Model
{
    public class ProductInfo : IEntity
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public string Note { get; set; }
        
        public ICollection<ProductCategories> ProductCategories { get; set; }
    }
}