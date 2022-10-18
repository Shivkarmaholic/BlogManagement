using System.ComponentModel.DataAnnotations;

namespace BlogManagement.Models
{
    public class BaseModel
    {        
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; }        
        public DateTime ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class DeleteModel
    {
        public int Id { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
    public class HideModel
    {
        public int Id { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
