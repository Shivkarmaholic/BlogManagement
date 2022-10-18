using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace BlogManagement.Models
{
    public class BlogLikeModel
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int LikedBy { get; set; }
        public DateTime LikedOn { get; set; }
        public bool IsActive { get; set; }
        
    }
}
