using BlogManagement.Context;
using BlogManagement.Models;
using BlogManagement.Repositories.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Data.Common;
using System.Reflection;
using System.Security.Cryptography;

namespace BlogManagement.Repositories
{
    public class UserRegistrationAsyncRepository : IUserRegistrationAsyncRepository
    {
        DapperContext _context;
        public UserRegistrationAsyncRepository(DapperContext context)
        {
            _context=context;
        }
        public async Task<int> DeleteUser(DeleteModel obj)
        {
            int result = 0;
            if (obj.Id != 0)
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    obj.ModifiedDate = DateTime.Now;
                    obj.ModifiedBy = 1;
                    var checkUser = await connection.QuerySingleOrDefaultAsync<UserModel>(@"select * from tblUser 
                                                                where id=@Id and isDeleted=1", obj);
                    if (checkUser == null)
                    {
                        result = await connection.QuerySingleAsync(@"update tblUser set isDeleted=1,ModifiedDate=@ModifiedDate
                                                    ModifiedBy=@ModifiedBy where id=@id ", obj);
                    }
                    else return -1;
                }

            }
            return result;
        }

        public async Task<BaseResponseModel> GetAllUsers(int pageno, int pagesize, string? searchText)
        {
            BaseResponseModel responseModel=new BaseResponseModel();
            List<UserSelectModel> userList = new List<UserSelectModel>();
            PaginationModel pagination = new PaginationModel();

            if (pageno == 0)
            {
                pageno = 1;
            }
            if (pagesize == 0)
            {
                pagesize = 10;
            }

            using (var connection = _context.CreateConnection())
            {
                connection.Open();


                if (searchText == null)
                    searchText = "";


                var sql = (@"Select ROW_NUMBER() OVER(ORDER BY tu.Id desc) as SrNo,id,userName,PasswordHash,FirstName,Lastname,emailId,mobile,shortDesc,
                        createdBy,createdDate,modifiedBy,modifiedDate,isDeleted from tblUser tu where tu.isDeleted=0 order by tu.id desc
                           OFFSET(@pageno - 1) * @pagesize ROWS FETCH NEXT @pagesize ROWS ONLY; 

                           select @pageno as PageNo, count(distinct tu.Id) as TotalRecords 
                           from tblUser tu where tu.isDeleted=0
                          ");

                                
                var result = await connection.QueryMultipleAsync(sql, new { pageno, pagesize, searchText });
                var cList = await result.ReadAsync<UserSelectModel>();
                userList = cList.ToList();
                var paginations = await result.ReadAsync<PaginationModel>();
                pagination = paginations.FirstOrDefault();
                int PageCount = 0;
                int last = 0;
                last = pagination.TotalRecords % pagesize;
                PageCount = pagination.TotalRecords / pagesize;
                pagination.PageCount = PageCount;
                if (last > 0)
                {
                    pagination.PageCount = PageCount + 1;
                }

                responseModel.Response1 = userList;
                responseModel.Response2 = pagination;
            }
            return responseModel;
        }

        public async Task<UserSelectModel> GetUserById(int id)
        {
            UserSelectModel model = new UserSelectModel();
            using (var connection= _context.CreateConnection())
            {
                connection.Open();
                model = await connection.QueryFirstOrDefaultAsync<UserSelectModel>(@"Select id as SrNo,id,
                        userName,PasswordHash,FirstName,Lastname,emailId,mobile,shortDesc,createdBy,createdDate,
                        modifiedBy,modifiedDate,isDeleted from tblUser where isDeleted=0 and id=@id", new {id});
            }
            return model;
        }

        public async Task<int> InsertUser(UserModel model)
        {
            int result = 0;
            using (var connection= _context.CreateConnection())
            {
                connection.Open();
                model.CreatedBy = 1;
                model.CreatedDate = DateTime.Now;
                model.ModifiedBy = 1;
                model.ModifiedDate = DateTime.Now;
                model.IsDeleted = false;
                var checkUser = await connection.QuerySingleOrDefaultAsync<UserModel>(@"select * from tblUser 
                                                                where emailId=@emailId or userName=@userName",model);

                if (checkUser == null)
                    result = await connection.ExecuteAsync(@"DECLARE @salt UNIQUEIDENTIFIER=NEWID();
                                insert into tblUser(userTypeid,userName,PasswordHash,salt,FirstName,
                               LastName,emailId,mobile,ShortDesc,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted)
                               values (@UserTypeId,@userName,HASHBYTES('sha2_512',@password+cast(@salt as varchar(100))),@salt,@FirstName,
                               @LastName,@emailId,@mobile,@ShortDesc,@createdBy,@createdDate,@modifiedBy,@modifiedDate,@isDeleted)", model);
                else
                    return -1;
            }
                return result;
        }

        public async Task<int> UpdateUser(UserUpdateModel model)
        {
            int result;
            using (var connection= _context.CreateConnection())
            {
                var res = await connection.QueryFirstOrDefaultAsync<UserModel>(@"select * from tblUser
                                    where ((firstName=@firstName and lastName=@lastName) or EmailId=@EmailId)
                                    and isDeleted=0 and Id != @Id", model);
                if (res != null)
                    return -1;

                connection.Open();
                model.ModifiedDate = DateTime.Now;
                model.ModifiedBy = 1;

                result = await connection.ExecuteAsync(@"update tblUser set 
                FirstName=@FirstName,LastName=@LastName,emailId=@emailId,mobile=@mobile,ShortDesc=@ShortDesc,
                ModifiedBy=@ModifiedBy,ModifiedDate=@ModifiedDate where Id=@Id and isDeleted=0", model);
                if (result == 0)
                    return 0;
                else return model.Id;
            }
        }
    }
}
