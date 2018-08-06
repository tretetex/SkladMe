using System.Collections.Generic;
using SkDAL.Base;

namespace SkDAL.Model
{
    public class Chapter : BaseModel
    {
        public string Title { get; set; }

        public ICollection<Subñhapter> Subchapters { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}