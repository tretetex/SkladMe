using System.Collections.Generic;
using SkDAL.EFUtilities.Model;

namespace SkDAL.Model
{
    public class Chapter : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public ICollection<Sub�hapter> Subchapters { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}