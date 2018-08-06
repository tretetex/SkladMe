using System.Collections.Generic;
using SkDAL.Base;

namespace SkDAL.Model
{
    public class Tag : BaseModel
    {
        public string Title { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}