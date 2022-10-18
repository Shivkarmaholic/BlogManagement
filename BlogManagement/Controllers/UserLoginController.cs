using BlogManagement.Models;
using BlogManagement.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlogManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        private readonly ILogger logger;
        IConfiguration configuration;
        private readonly IUserLoginAsyncRepository loginAsyncRepository;

        public UserLoginController(IConfiguration configuration,ILoggerFactory logger,IUserLoginAsyncRepository userLoginRepository)
        {
            this.configuration = configuration;
            this.logger = logger.CreateLogger<UserRegistrationController>();
            loginAsyncRepository = userLoginRepository;
        }

        [HttpOptions("Login")]
        public async Task<ActionResult> ValidateUser(UserLoginModel userLogin)
        {
            BaseResponseModel baseResponse=new BaseResponseModel();
            var result =await loginAsyncRepository.ValidateUser(userLogin);
            if(result==1)
            {
                var rtnmsg = string.Format("Validation Successful");
                logger.LogDebug(rtnmsg);
                baseResponse.StatusCode= StatusCodes.Status200OK.ToString();
                baseResponse.StatusMessage= rtnmsg;
                return Ok(baseResponse);
            }
            else if(result==-1)
            {

                var rtnmsg = string.Format("Login Failed. Please Enter Valid Username or Password.");
                logger.LogDebug(rtnmsg);
                baseResponse.StatusCode = StatusCodes.Status404NotFound.ToString();
                baseResponse.StatusMessage = rtnmsg;
                return Ok(baseResponse);
            }
            else
            {

                var rtnmsg = string.Format("UserLoginController : UserValidation Action");
                logger.LogDebug(rtnmsg);
                baseResponse.StatusCode = StatusCodes.Status400BadRequest.ToString();
                baseResponse.StatusMessage = "Bad Request";
                return Ok(baseResponse);
            }
            return Ok(baseResponse);
        }



        [HttpOptions("ChangePassword")]
        public async Task<ActionResult> ChangePassword(UserPasswordChangeModel userPassword)
        {
            BaseResponseModel baseResponse = new BaseResponseModel();
            var result = await loginAsyncRepository.ChangePassword(userPassword);
            if (result >=1)
            {
                var rtnmsg = string.Format("Password Changed Successfully");
                logger.LogDebug(rtnmsg);
                baseResponse.StatusCode = StatusCodes.Status200OK.ToString();
                baseResponse.StatusMessage = rtnmsg;
                return Ok(baseResponse);
            }
            else if (result == 0)
            {

                var rtnmsg = string.Format("UserLoginController : UserValidation Action");
                logger.LogDebug(rtnmsg);
                baseResponse.StatusCode = StatusCodes.Status404NotFound.ToString();
                baseResponse.StatusMessage = "Enter Valid Old Password";
                return Ok(baseResponse);
            }
            else
            {

                var rtnmsg = string.Format("UserLoginController : UserValidation Action");
                logger.LogDebug(rtnmsg);
                baseResponse.StatusCode = StatusCodes.Status400BadRequest.ToString();
                baseResponse.StatusMessage = rtnmsg;
                return Ok(baseResponse);
            }
            return Ok(baseResponse);
        }



    }
}
