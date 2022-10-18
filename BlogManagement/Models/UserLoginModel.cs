using System.ComponentModel.DataAnnotations;

namespace BlogManagement.Models
{
    public class UserLoginModel 
    {
        public string UserName { get; set; }
        public string Password { get; set; }

    }

    public class UserPasswordChangeModel:BaseModel
    {
        public string UserName { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        [MinLength(5),MaxLength(15)]
        public string NewPassword { get; set; }
    }
}
