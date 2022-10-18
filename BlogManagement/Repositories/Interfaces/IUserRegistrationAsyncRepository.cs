using BlogManagement.Models;

namespace BlogManagement.Repositories.Interfaces
{
    public interface IUserRegistrationAsyncRepository
    {
        public Task<BaseResponseModel> GetAllUsers(int pageno, int pagesize, string? searchText);
        public Task<UserSelectModel> GetUserById(int id);
        public Task<int> InsertUser(UserModel model);
        public Task<int> DeleteUser(DeleteModel model);
        public Task<int> UpdateUser(UserUpdateModel model);
    }
}
