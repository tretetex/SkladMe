using ApiResponse = SkServ.Utils.ApiResponse;

namespace SkServ.Model
{
    public class NoticeInfo
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }

        public static NoticeInfo FromJson(ApiResponse response)
        {
            return new NoticeInfo
            {
                Id = response["id"],
                Text = response["text"],
                Image = response["image"],
                Link = response["link"]
            };
        }
    }
}
