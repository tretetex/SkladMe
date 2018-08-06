using System.Collections.Generic;
using SkDAL.EFUtilities.Model;

namespace SkDAL.Model
{
    public class Sub�hapter : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}