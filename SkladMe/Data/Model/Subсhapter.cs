using System.Collections.Generic;
using SkDAL.Base;

namespace SkDAL.Model
{
    public class Subñhapter : BaseModel
    {
        public string Title { get; set; }

        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}