using SkDAL.Services;

namespace SkDAL
{
    public static class Db
    {
        public static ColorService Colors { get; } = new ColorService();
        public static ProductService Products { get; } = new ProductService();
        public static TagService Tags { get; } = new TagService();
        public static UserService Users { get; } = new UserService();
        public static CategoryService Categories { get; } = new CategoryService();
    }
}
