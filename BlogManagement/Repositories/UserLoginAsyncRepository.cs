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
            try
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
            catch (Exception ex)
            {
                throw;
            }
}

        public async Task<UserModel> ValidateUser(UserLoginModel user)
        {
            try
            {
                int result=0;
            UserModel user1=new UserModel();
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                user1 = await connection.QueryFirstOrDefaultAsync<UserModel>(@"
                                 Select * from tblUser where userName=@UserName 
                                        and PasswordHash =HASHBYTES('SHA2_512', (@Password + convert(nvarchar(max),salt)))", user);

                if (user1 == null)
                    return user1 ;
                else
                    return user1;
                
            }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
