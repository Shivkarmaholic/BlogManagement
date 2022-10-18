using BlogManagement.Models;

namespace BlogManagement.Repositories.Interfaces
{
    public interface IUserLoginAsyncRepository
    {
        public Task<int> ValidateUser(UserLoginModel user);
        public Task<int> ChangePassword(UserPasswordChangeModel userPassword);
    }
}
