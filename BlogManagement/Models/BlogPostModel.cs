using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace BlogManagement.Models
{
    public class BlogPostModel:BaseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleImage { get; set; }
        public string SDesc { get; set; }
        public string LDesc { get; set; }
        public bool IsHidden { get; set; }

    }

}
