using BlogManagement.Models;
using BlogManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostAsyncRepository blogPostAsyncRepository;
        IConfiguration configuration;
        private readonly ILogger logger;

        public BlogPostController(ILoggerFactory loggerFactory, IConfiguration configuration, IBlogPostAsyncRepository asyncRepository)
        {
            this.logger = loggerFactory.CreateLogger<UserRegistrationController>();
            this.configuration = configuration;
            blogPostAsyncRepository = asyncRepository;
        }

        [HttpGet("GetAllPosts")]
        public async Task<ActionResult> GetAllPosts(int pageno, int pagesize, string? searchText)
        {
            BaseResponseModel responseDetails = new BaseResponseModel();
            var result = await blogPostAsyncRepository.GetAllPosts(pageno, pagesize, searchText);
            List<BlogPostModel> blogPosts = (List<BlogPostModel>)result.Response1;
            if (blogPosts.Count == 0)
            {
                var rtnmsg1 = string.Format("BlogPostRegistrationController-getAllByPagination : Calling GetAllBlogPostsByPagination");
                logger.LogDebug(rtnmsg1);
                responseDetails.StatusCode = StatusCodes.Status404NotFound.ToString();
                responseDetails.StatusMessage = rtnmsg1;
                return Ok(responseDetails);
            }
            var rtnmsg = string.Format("All BlogPosts records are fetched successfully.");
            logger.LogDebug(rtnmsg);
            responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
            responseDetails.StatusMessage = rtnmsg;
            responseDetails.Response1 = blogPosts;
            responseDetails.Response2 = result.Response2;
            return Ok(responseDetails);
        }


        
        [HttpDelete("DeleteBlogPost")]
        public async Task<ActionResult> DeleteBlogPost(int id)
        {
            DeleteModel deleteModel = new DeleteModel();
            deleteModel.Id = id;
            BaseResponseModel baseResponse = new BaseResponseModel();
            logger.LogDebug(string.Format($"BlogPostregistrationController-DeleteBlogPost : Calling DeleteBlogPost action with Id {deleteModel.Id}"));
            deleteModel.ModifiedBy = 2;
            deleteModel.ModifiedDate = DateTime.Now;
            var Execution = await blogPostAsyncRepository.DeletePost(deleteModel);
            if (Execution == 0)
            {
                var retunmsg = string.Format($"BlogPost with Id {deleteModel.Id} not found");
                logger.LogDebug(retunmsg);
                baseResponse.StatusCode = StatusCodes.Status404NotFound.ToString();
                baseResponse.StatusMessage = retunmsg;
                return Ok(baseResponse);
            }
            else if(Execution==1)
            {
                var rtnmsg = string.Format($"Record with Id {deleteModel.Id} is deleted!");
                logger.LogDebug(rtnmsg);
                baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
                baseResponse.StatusMessage = rtnmsg;
                baseResponse.Response1 = Execution;
                return Ok(baseResponse);
            }
            else
            {
                var rtnmsg = string.Format($"Record with Id {deleteModel.Id} is Already Deleted!");
                logger.LogDebug(rtnmsg);
                baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
                baseResponse.StatusMessage = rtnmsg;
                baseResponse.Response1 = Execution;
                return Ok(baseResponse);
            }
        }



        [HttpPut("UpdatePost")]
        public async Task<ActionResult> UpdateBlogPost(BlogPostModel blogPost)
        {
            BaseResponseModel baseResponse = new BaseResponseModel();
            logger.LogDebug(string.Format($"BlogPostregistrationController-UpdateBlogPost : {blogPost.Id} : Calling UpdateBlogPost action"));
            if (blogPost != null)
            {
                var Execution = await blogPostAsyncRepository.UpdatePost(blogPost);
                if (Execution == -1)
                {
                    var returnmsg = string.Format($"Record is already exists with Title :{blogPost.Title}.");
                    logger.LogDebug(returnmsg);
                    baseResponse.StatusCode = StatusCodes.Status409Conflict.ToString();
                    baseResponse.StatusMessage = returnmsg;
                    return Ok(baseResponse);
                }
                else if (Execution >= 1)
                {
                    var rtnmsg = string.Format("BlogPost updated successfully..");
                    logger.LogDebug(rtnmsg);
                    baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
                    baseResponse.StatusMessage = rtnmsg;
                    baseResponse.Response1 = Execution;
                    return Ok(baseResponse);
                }
                else
                {
                    var rtnmsg = string.Format("Error while updating");
                    logger.LogDebug(rtnmsg);
                    baseResponse.StatusCode = StatusCodes.Status409Conflict.ToString();
                    baseResponse.StatusMessage = rtnmsg;
                    return Ok(baseResponse);
                }
            }
            else
            {
                var rtnmsg = string.Format("BlogPost updated successfully..");
                logger.LogDebug(rtnmsg);
                baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
                baseResponse.StatusMessage = rtnmsg;
                return Ok(baseResponse);
            }
        }

        [HttpPut("HidePost")]
        public async Task<ActionResult> HideBlogPost(HideModel model)
        {
            BaseResponseModel baseResponse = new BaseResponseModel();
            logger.LogDebug(string.Format($"BlogPostregistrationController-hideBlogPost : {model.Id} : Calling HideBlogPost action"));
            if (model.Id != 0)
            {
                var Execution = await blogPostAsyncRepository.HideBlogPost(model);
                if (Execution == -1)
                {
                    var returnmsg = string.Format($"Record is already Hidden with Id :{model.Id}.");
                    logger.LogDebug(returnmsg);
                    baseResponse.StatusCode = StatusCodes.Status409Conflict.ToString();
                    baseResponse.StatusMessage = returnmsg;
                    return Ok(baseResponse);
                }
                else if (Execution >= 1)
                {
                    var rtnmsg = string.Format("BlogPost Hidden successfully..");
                    logger.LogDebug(rtnmsg);
                    baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
                    baseResponse.StatusMessage = rtnmsg;
                    baseResponse.Response1 = Execution;
                    return Ok(baseResponse);
                }
                else
                {
                    var rtnmsg = string.Format("Error while Hiding");
                    logger.LogDebug(rtnmsg);
                    baseResponse.StatusCode = StatusCodes.Status409Conflict.ToString();
                    baseResponse.StatusMessage = rtnmsg;
                    return Ok(baseResponse);
                }
            }
            else
            {
                var rtnmsg = string.Format("BlogPost Hide successfully..");
                logger.LogDebug(rtnmsg);
                baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
                baseResponse.StatusMessage = rtnmsg;
                return Ok(baseResponse);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddNewBlogPost(BlogPostModel blogPost)
        {
            BaseResponseModel baseResponse = new BaseResponseModel();
            logger.LogDebug(String.Format($"BlogPostregistrationController-AddNewBlogPost:Calling By addNewBlogPost action."));
            if (blogPost != null)
            {
                var Execution = await blogPostAsyncRepository.InsertPost(blogPost);
                if (Execution == -1)
                {
                    var returnmsg = string.Format($"Blogpost Is Already Exists With Title {blogPost.Title}");
                    logger.LogDebug(returnmsg);
                    baseResponse.StatusCode = StatusCodes.Status409Conflict.ToString();
                    baseResponse.StatusMessage = returnmsg;
                    return Ok(baseResponse);
                }
                else if (Execution >= 1)
                {
                    var rtnmsg = string.Format("BlogPost added successfully..");
                    logger.LogInformation(rtnmsg);
                    logger.LogDebug(string.Format("BlogPostregistrationController-AddNewBlogPost : Completed Adding BlogPost"));
                    baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
                    baseResponse.StatusMessage = rtnmsg;
                    baseResponse.Response1 = Execution;
                    return Ok(baseResponse);
                }
                else
                {
                    var rtnmsg1 = string.Format("Error while Adding");
                    logger.LogError(rtnmsg1);
                    baseResponse.StatusCode = StatusCodes.Status409Conflict.ToString();
                    baseResponse.StatusMessage = "";
                    return Ok(baseResponse);
                }

            }
            else
            {
                var returnmsg = string.Format("Record added successfully..");
                logger.LogDebug(returnmsg);
                baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
                baseResponse.StatusMessage = returnmsg;
                return Ok(baseResponse);
            }
        }
    }
}
