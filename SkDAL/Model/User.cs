using System;
using System.Collections.Generic;
using SkDAL.EFUtilities.Model;

namespace SkDAL.Model
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public int? PaidMainListCount { get; set; }
        public int? PaidTotalCount { get; set; }
        public int? OrganizedCount { get; set; }
        public int? MessagesCount { get; set; }
        public int? LikesCount { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? RegistrationDate { get; set; }

        public int? UserGroupId { get; set; }
        public UserGroup UserGroup { get; set; }

        public virtual ICollection<Product> ProductsAsCreator { get; set; }
        public virtual ICollection<Product> ProductsAsOrganizer { get; set; }
        public virtual ICollection<Product> ProductsAsMain { get; set; }
        public virtual ICollection<Product> ProductsAsReserve { get; set; }
        public virtual ICollection<Product> ProductsAsAuthor { get; set; }
    }
}