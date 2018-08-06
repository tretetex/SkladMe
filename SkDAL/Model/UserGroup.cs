using System.Collections.Generic;
using SkDAL.EFUtilities.Model;

namespace SkDAL.Model
{
    public class UserGroup : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Color { get; set; }
        public string Background { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}