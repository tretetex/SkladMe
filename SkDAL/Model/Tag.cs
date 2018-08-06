using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SkDAL.EFUtilities.Model;

namespace SkDAL.Model
{
    public class Tag : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }

        [NotMapped]
        public virtual ICollection<Product> Products { get; set; }
    }
}