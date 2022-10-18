namespace BlogManagement.Models
{
    public class BaseResponseModel
    {
        public string StatusCode { get; set; }

        public string StatusMessage { get; set; }
        public object Response1 { get; set; }
        public object Response2 { get; set; }
        public object Response3 { get; set; }
    }
}