using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace BlogManagement.Models
{
    public class UserModel:BaseModel
    {        
        public int Id { get; set; }
        public int UserTypeId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public long Mobile { get; set; }
        public string ShortDesc { get; set; }

    }

    public class UserUpdateModel : BaseModel
    {
        public int Id { get; set; }  
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public long Mobile { get; set; }
        public string ShortDesc { get; set; }

    }

    public class UserSelectModel : BaseModel
    {
        public int SrNo { get; set; }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public long Mobile { get; set; }
        public string ShortDesc { get; set; }

    }
       

}
