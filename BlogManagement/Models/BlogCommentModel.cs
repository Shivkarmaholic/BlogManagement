using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace BlogManagement.Models
{
    public class BlogCommentModel
    {
        public int Id { get; set; }
        public string Comment { get; set; } 
        public int UserId { get; set; }
        public int PostId { get; set; }
        public bool IsDeleted { get; set; }
        
    }
}
