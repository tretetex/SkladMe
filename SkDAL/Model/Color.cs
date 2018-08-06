using SkDAL.EFUtilities.Model;

namespace SkDAL.Model
{
    public class Color : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
    }
}
