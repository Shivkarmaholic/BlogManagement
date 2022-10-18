using BlogManagement.Models;

namespace BlogManagement.Repositories.Interfaces
{
    public interface IBlogPostAsyncRepository
    {
        public Task<BaseResponseModel> GetAllPosts(int pageno, int pagesize, string? searchText);
        public Task<int> InsertPost(BlogPostModel model);
        public Task<int> DeletePost(DeleteModel model);
        public Task<int> UpdatePost(BlogPostModel model);
        public Task<int> HideBlogPost(HideModel model);
    }
}
