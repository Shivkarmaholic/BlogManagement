using BlogManagement.Context;
using BlogManagement.Models;
using BlogManagement.Repositories.Interfaces;
using Dapper;
using System.Reflection;

namespace BlogManagement.Repositories
{
    public class UserLoginAsyncRepository : IUserLoginAsyncRepository
    {
        private readonly DapperContext _context;

        public UserLoginAsyncRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> ChangePassword(UserPasswordChangeModel userPassword)
        {
            int result = 0;
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var res = await connection.QueryFirstOrDefaultAsync<string>(@"
                                 Select (CASE when PasswordHash =  HASHBYTES('SHA2_512', (@OldPassword+ convert(nvarchar(max),salt))) 
                                        THEN 'Valid' 
                                        ELSE 'Invalid'
                                        END )as result from tblUser where  userName=@UserName", userPassword);

                if(res == "Valid")
                {
                    userPassword.ModifiedBy = 3;
                    userPassword.ModifiedDate = DateTime.Now;
                    result = await connection.ExecuteAsync(@"update tblUser 
                            set passwordHash=HASHBYTES('SHA2_512', (@NewPassword+convert(nvarchar(max),salt))),
                            ModifiedBy=@ModifiedBy,ModifiedDate=@ModifiedDate where userName=@UserName and isDeleted=0", userPassword);
                }
            }
            return result;
        }

        public async Task<int> ValidateUser(UserLoginModel user)
        {
            int result=0;
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var res = await connection.QueryFirstOrDefaultAsync<UserLoginModel>(@"
                                 Select * from tblUser where userName=@UserName 
                                        and PasswordHash =HASHBYTES('SHA2_512', (@Password + convert(nvarchar(max),salt)))", user);

                if (res == null)
                    return -1;
                else
                    result = 1;
                
            }
            return result;
        }
    }
}
