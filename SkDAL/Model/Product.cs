using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SkDAL.EFUtilities.Model;

namespace SkDAL.Model
{
    public class Product : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Color { get; set; }
        public string Note { get; set; }
        public int? Price { get; set; }
        public int? Fee { get; set; }
        public int RealFee { get; set; }
        public int ReviewCount { get; set; }
        public int ViewCount { get; set; }
        public int UsersTotalCount { get; set; }
        public int PeopleForMin { get; set; }
        public int MembersAsMainCount { get; set; }
        public int MembersAsReserveCount { get; set; }
        public int UsersAsAuthorCount { get; set; }
        public double? Rating { get; set; }
        public double Popularity { get; set; }
        public double OrganizerRating { get; set; }
        public double ClubMemberRating { get; set; }
        public bool IsRepeat { get; set; }
        public bool Important { get; set; }
        public DateTime DateOfCreation { get; set; }    
        public DateTime DateUpdate { get; set; }
        public DateTime? DateFee { get; set; }

        public int CreatorId { get; set; }
        public User Creator { get; set; }
        public int? OrganizerId { get; set; }
        public User Organizer { get; set; }
        public int PrefixId { get; set; }
        public Prefix Prefix { get; set; }
        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; }
        public int SubchapterId { get; set; }
        public Subñhapter Subñhapter { get; set; }

        public ICollection<User> MembersAsMain { get; set; }
        public ICollection<User> MembersAsReserve { get; set; }
        public ICollection<User> UsersAsAuthor { get; set; }

        [NotMapped]
        public ICollection<Tag> Tags { get; set; }
        public ICollection<ProductCategories> ProductCategories { get; set; }
    }
}