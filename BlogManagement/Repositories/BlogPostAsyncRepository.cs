using BlogManagement.Context;
using BlogManagement.Models;
using BlogManagement.Repositories.Interfaces;
using Dapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Reflection;

namespace BlogManagement.Repositories
{
    public class BlogPostAsyncRepository : IBlogPostAsyncRepository
    {
        DapperContext _context;
        public BlogPostAsyncRepository(DapperContext dapperContext)
        {
            _context = dapperContext;
        }

        public async Task<int> DeletePost(DeleteModel obj)
        {
            int result = 0;
            if (obj.Id != 0)
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    var checkPost = await connection.QuerySingleOrDefaultAsync<BlogPostModel>(@"select * from tblBlogPost 
                                                                where id=@Id and isDeleted=1", obj);

                    obj.ModifiedDate = DateTime.Now;
                    obj.ModifiedBy = 1;

                    if (checkPost == null)
                    {
                        result = await connection.QuerySingleAsync(@"update tblBlogPost set isDeleted=1,ModifiedDate=@ModifiedDate,
                                                    ModifiedBy=@ModifiedBy where id=@id ", obj);
                        return result;
                    }                        
                    else return -1;
                }

            }
            return result;
        }

        public async Task<BaseResponseModel> GetAllPosts(int pageno, int pagesize, string? searchText)
        {
            BaseResponseModel responseModel = new BaseResponseModel();
            List<BlogPostModel> blogPosts = new List<BlogPostModel>();
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
                

                var sql = (@"Select ROW_NUMBER() OVER(ORDER BY Id desc) as SrNo,id,title,titleImage,sDesc,lDesc,
                        createdBy,createdDate,modifiedBy,modifiedDate,isDeleted from tblBlogPost where isDeleted=0 order by id desc
                           OFFSET(@pageno - 1) * @pagesize ROWS FETCH NEXT @pagesize ROWS ONLY; 

                           select @pageno as PageNo, count(distinct Id) as TotalRecords 
                           from tblBlogPost where isDeleted=0
                          ");


                var result = await connection.QueryMultipleAsync(sql, new { pageno, pagesize, searchText });
                var cList = await result.ReadAsync<BlogPostModel>();
                blogPosts = cList.ToList();
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

                responseModel.Response1 = blogPosts;
                responseModel.Response2 = pagination;
            }
            return responseModel;
        }

        public async Task<int> HideBlogPost(HideModel model)
        {
            int result = 0;
            if (model.Id != 0)
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    var checkPost = await connection.QuerySingleOrDefaultAsync<BlogPostModel>(@"select * from tblBlogPost 
                                                                where id=@Id and isDeleted=0 and isHidden=1", model);

                    model.ModifiedDate = DateTime.Now;
                    model.ModifiedBy = 1;

                    if (checkPost == null)
                    {
                        result = await connection.QuerySingleAsync<int>(@"update tblBlogPost set isHidden=1,ModifiedDate=@ModifiedDate,
                                                    ModifiedBy=@ModifiedBy where id=@id and isDeleted=0 ", model);
                        return result;
                    }
                    else return -1;
                }

            }
            return result;
        }

        public async Task<int> InsertPost(BlogPostModel model)
        {
            int result = 0;
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                model.CreatedBy = 1;
                model.CreatedDate = DateTime.Now;
                model.ModifiedBy = 1;
                model.ModifiedDate = DateTime.Now;
                model.IsDeleted = false;
                model.IsHidden=false;
                var checkUser = await connection.QuerySingleOrDefaultAsync<BlogPostModel>(@"select * from tblBlogPost 
                                                                where title=@title", model);

                if (checkUser == null)
                    result = await connection.ExecuteAsync(@"
                               insert into tblBlogPost(title,titleImage,sDesc,lDesc,createdBy,createdDate,modifiedBy,modifiedDate,isDeleted,isHidden)
                               values (@title,@TitleImage,@sDesc,@lDesc,@createdBy,@createdDate,@modifiedBy,@modifiedDate,@isDeleted,@IsHidden)"
                               ,model);
                else
                    return -1;
            }
            return result;
        }

        public async Task<int> UpdatePost(BlogPostModel model)
        {
            int result;
            using (var connection = _context.CreateConnection())
            {
                var res = await connection.QueryFirstOrDefaultAsync<BlogPostModel>(@"select * from tblBlogPost
                                    where title=@title
                                    and isDeleted=0 and IsHidden=0 and Id != @Id", model);
                if (res != null)
                    return -1;

                connection.Open();
                model.ModifiedDate = DateTime.Now;
                model.ModifiedBy = 1;

                result = await connection.ExecuteAsync(@"update tblBlogPost set 
                title=@title,titleImage=@TitleImage,sDesc=@sDesc,lDesc=@lDesc,
                ModifiedBy=@ModifiedBy,ModifiedDate=@ModifiedDate where Id=@Id and isDeleted=0 ", model);
                if (result == 0)
                    return 0;
                else return model.Id;
            }
        }
    }
}
