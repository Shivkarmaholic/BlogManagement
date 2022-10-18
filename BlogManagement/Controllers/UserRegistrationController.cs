using BlogManagement.Models;
using BlogManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace BlogManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        private readonly IUserRegistrationAsyncRepository userRegistrationAsync ;
        IConfiguration configuration;
        private readonly ILogger logger;

        public UserRegistrationController(ILoggerFactory loggerFactory,IConfiguration configuration,IUserRegistrationAsyncRepository asyncRepository)
        {
            this.logger = loggerFactory.CreateLogger<UserRegistrationController>();
            this.configuration = configuration;
            userRegistrationAsync = asyncRepository;
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult> GetAllUsers(int pageno, int pagesize, string? searchText)
        {
            BaseResponseModel responseDetails = new BaseResponseModel();
            var userList = await userRegistrationAsync.GetAllUsers(pageno, pagesize, searchText);
            List<UserSelectModel> users = (List<UserSelectModel>)userList.Response1;
            if (users.Count == 0)
            {
                var rtnmsg1 = string.Format("UserRegistrationController-getAllByPagination : Calling GetAllUsersByPagination");
                logger.LogDebug(rtnmsg1);
                responseDetails.StatusCode = StatusCodes.Status404NotFound.ToString();
                responseDetails.StatusMessage = rtnmsg1;
                return Ok(responseDetails);
            }
            var rtnmsg = string.Format("All UserList records are fetched successfully.");
            logger.LogDebug(rtnmsg);
            responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
            responseDetails.StatusMessage = rtnmsg;
            responseDetails.Response1 = users;
            responseDetails.Response2 = userList.Response2;
            return Ok(responseDetails);            
        }


        [HttpGet("GetUserById")]
        public async Task<ActionResult> GetUserById(int id)
        {
            BaseResponseModel responseDetails = new BaseResponseModel();
            var user = await userRegistrationAsync.GetUserById(id);            
            if (user.UserName==null)
            {
                var rtnmsg1 = string.Format("UserRegistrationController-getbyID : Calling GetBYId");
                logger.LogDebug(rtnmsg1);
                responseDetails.StatusCode = StatusCodes.Status404NotFound.ToString();
                responseDetails.StatusMessage = rtnmsg1;
                return Ok(responseDetails);
            }
            var rtnmsg = string.Format("All UserList records are fetched successfully.");
            logger.LogDebug(rtnmsg);
            responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
            responseDetails.StatusMessage = rtnmsg;
            responseDetails.Response1 = user;        
            return Ok(responseDetails);
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            DeleteModel deleteUser = new DeleteModel();
            deleteUser.Id = id;
            BaseResponseModel baseResponse = new BaseResponseModel();
            logger.LogDebug(string.Format($"UserregistrationController-DeleteUser : Calling DeleteUser action with Id {deleteUser.Id}"));
            deleteUser.ModifiedBy = 2;
            deleteUser.ModifiedDate = DateTime.Now;
            var Execution = await userRegistrationAsync.DeleteUser(deleteUser);
            if (Execution == 0)
            {
                var retunmsg = string.Format($"Record with Id {deleteUser.Id} not found");
                logger.LogDebug(retunmsg);
                baseResponse.StatusCode = StatusCodes.Status404NotFound.ToString();
                baseResponse.StatusMessage = retunmsg;
                return Ok(baseResponse);
            }
            else
            {
                var rtnmsg = string.Format($"Record with Id {deleteUser.Id} is deleted!");
                logger.LogDebug(rtnmsg);
                baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
                baseResponse.StatusMessage = rtnmsg;
                baseResponse.Response1 = Execution;
                return Ok(baseResponse);
            }
        }



        [HttpPut]
        public async Task<ActionResult> UpdateUser(UserUpdateModel user)
        {
            BaseResponseModel baseResponse = new BaseResponseModel();
            logger.LogDebug(string.Format($"UserregistrationController-UpdateUser : {user.Id} : Calling UpdateUser action"));
            if (user != null)
            {
                var Execution = await userRegistrationAsync.UpdateUser(user);
                if (Execution == -1)
                {
                    var returnmsg = string.Format($"Record is already exists with Email :{user.EmailId}.");
                    logger.LogDebug(returnmsg);
                    baseResponse.StatusCode = StatusCodes.Status409Conflict.ToString();
                    baseResponse.StatusMessage = returnmsg;
                    return Ok(baseResponse);
                }
                else if (Execution >= 1)
                {
                    var rtnmsg = string.Format("Record updated successfully..");
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
                var rtnmsg = string.Format("Record updated successfully..");
                logger.LogDebug(rtnmsg);
                baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
                baseResponse.StatusMessage = rtnmsg;
                return Ok(baseResponse);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddNewUser(UserModel user)
        {
            BaseResponseModel baseResponse = new BaseResponseModel();
            logger.LogDebug(String.Format($"UserregistrationController-AddNewUserregistration:Calling By addNewUser action."));
            if (user != null)
            {
                var Execution = await userRegistrationAsync.InsertUser(user);
                if (Execution == -1)
                {
                    var returnmsg = string.Format($"Record Is Already saved With UserName {user.UserName}");
                    logger.LogDebug(returnmsg);
                    baseResponse.StatusCode = StatusCodes.Status409Conflict.ToString();
                    baseResponse.StatusMessage = returnmsg;
                    return Ok(baseResponse);
                }
                else if (Execution >= 1)
                {
                    var rtnmsg = string.Format("User Record added successfully..");
                    logger.LogInformation(rtnmsg);
                    logger.LogDebug(string.Format("UserregistrationController-AddNewUser : Completed Adding User record"));
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
