using BLL.Interface;
using Common.DTO;
using Common.Enum;
using Common.Message.AuthMessage;
using Common.Message.EmailMessage;
using Common.Message.GlobalMessage;
using Common.Message.ValidationMessage;
using Common.Template;
using DAL.Entities;
using DTO.DTO;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Api_InnerShop.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public AuthController(ILoginService loginService, IUserService userService, IEmailService emailService)
        {
            _loginService = loginService;
            _userService = userService;
            _emailService = emailService;
        }
        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="loginRequestDTO"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public IActionResult Login(LoginRequestDTO loginRequestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDTO(GlobalNotificationMessage.InvalidModel, StatusCodeEnum.InteralServerError, false, ModelState));
            }
            var result = _loginService.Login(loginRequestDTO);
            if (result != null)
            {
                return Ok(new ResponseDTO(AuthNotificationMessage.LoginSuccessfully, StatusCodeEnum.Created, true, result));
            }
            return BadRequest(new ResponseDTO(AuthNotificationMessage.LoginFailed, StatusCodeEnum.NotFound, false));
        }
        [HttpPost("signin-google")]
        public async Task<IActionResult> SignInGoogle([FromBody] GoogleAuthTokenDTO googleAuthToken)
        {
            var response = await _loginService.SignInWithGoogle(googleAuthToken);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(new ResponseDTO(AuthNotificationMessage.LoginFailed, StatusCodeEnum.NotFound, false));
        }
        /// <summary>
        /// Generates a new access token using a valid refresh token
        /// </summary>
        /// <param name="tokenDTO"></param>
        /// <returns></returns>
        [HttpPost("tokens/refresh")]
        public IActionResult GetNewTokenFromRefreshToken([FromBody] RequestTokenDTO tokenDTO)
        {
            if (ModelState.IsValid)
            {
                var result = _loginService.RefreshAccessToken(tokenDTO);
                if (result == null || string.IsNullOrEmpty(result.AccessToken))
                {
                    return BadRequest(new ResponseDTO(MessageErrorInRefreshToken.CommonError, StatusCodeEnum.NotFound, false, result));
                }
                return Ok(new ResponseDTO(MessageErrorInRefreshToken.Successfully, StatusCodeEnum.Created, true, result));
            }
            return BadRequest(new ResponseDTO(GlobalNotificationMessage.InvalidModel, StatusCodeEnum.InteralServerError, false));
        }
        /// <summary>
        /// Logs out a user by invalidating their tokens
        /// </summary>
        /// <param name="logoutDTO"></param>
        /// <returns></returns>
        [HttpPost("logout")]
        public IActionResult Logout([FromBody] LogOutDTO logoutDTO)
        {
            if (ModelState.IsValid)
            {
                var result = _loginService.Logout(logoutDTO.UserId);
                return Ok(result);
            }
            else
            {
                return Ok(new ResponseDTO(AuthNotificationMessage.LogOutFailed, StatusCodeEnum.NotFound, false));
            }
        }





        /// <summary>
        /// Sign Up For Customer API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("register/customer")]
        [ProducesResponseType(200, Type = typeof(ResponseDTO))]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SignUpAsCustomer([FromBody] SignUpCustomerDTOResquest model)
        {
            try
            {
                var checkValidation = _userService.CheckValidationSignUpCustomer(model);
                if (!checkValidation.IsSuccess)
                {
                    return BadRequest(checkValidation);
                }

                var addNewCustomer = await _userService.SignUpCustomer(model);

                if (addNewCustomer)
                {
                    var response = new ResponseDTO(AuthNotificationMessage.SignUpSuccessfully, StatusCodeEnum.Created, true);
                    return Ok(response);
                }
                else
                {
                    var response = new ResponseDTO(AuthNotificationMessage.SignUpUnsuccessfully, StatusCodeEnum.NotFound, true);
                    return BadRequest(response);
                }

            }
            catch (Exception ex)
            {
                var response = new ResponseDTO(ex.Message, StatusCodeEnum.NotFound, false);
                return BadRequest(response);
            }
        }


    

        /// <summary>
        /// Forgot Password API
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("password/forgot")]
        public IActionResult ForgotPassword(string email)
        {
            var result = _userService.ForgotPassword(email);
            return Ok(result);
        }

        /// <summary>
        /// Reset password API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("password/reset")]
        public IActionResult ResetPassword(ForgotPasswordModelDTO request)
        {
            var validationResult = _userService.CheckValidationForgotPassword(request);

            if (!validationResult.IsSuccess)
            {
                return BadRequest(validationResult);
            }

            var result = _userService.ResetPassword(request);

            if (result.IsSuccess)
            {
                return StatusCode(201, new ResponseDTO(AuthNotificationMessage.PasswordUpdate, StatusCodeEnum.Created, true));
            }
            else
            {
                return BadRequest(new ResponseDTO(AuthNotificationMessage.ResetPassword, StatusCodeEnum.NotFound, false));
            }
        }
        /// <summary>
        /// Verify user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("verify/otp")]
        public IActionResult VerifyUser(string userId, string otpCode)
        {
            var parseUserId = _userService.ParseUserIdToGuid(userId);
            if (parseUserId == Guid.Empty)
            {
                return BadRequest(new ResponseDTO(ValidationErrorMessage.WrongFormatUserId, StatusCodeEnum.NotFound, false));
            }

            var checkUserExist = _userService.CheckUserExistByUserId(parseUserId);
            if (!checkUserExist)
            {
                return BadRequest(new ResponseDTO(ValidationErrorMessage.UserNotFound, StatusCodeEnum.NotFound, false));
            }

            var checkUserVerifiedStatus = _userService.CheckUserVerifiedStatus(parseUserId);
            if (checkUserVerifiedStatus)
            {
                return BadRequest(new ResponseDTO(AuthNotificationMessage.UserIsVerified, StatusCodeEnum.NotFound, false));
            }

            var checkOtp = _userService.CheckOTP(parseUserId, otpCode);
            if (!checkOtp)
            {
                return BadRequest(new ResponseDTO(AuthNotificationMessage.OtpInccorect, StatusCodeEnum.NotFound, false));
            }

            var checkOTPExpired = _userService.CheckOTPExpired(parseUserId);
            if (checkOTPExpired)
            {
                return BadRequest(new ResponseDTO(AuthNotificationMessage.OtpExpired, StatusCodeEnum.NotFound, false));
            }

            var result = _userService.VerifyUser(parseUserId);
            if (!result)
            {
                return BadRequest(new ResponseDTO(AuthNotificationMessage.VerifyUnsuccessfully, StatusCodeEnum.InteralServerError, false));
            }
            return Ok(new ResponseDTO(AuthNotificationMessage.VerifySuccessfully, StatusCodeEnum.Created, true));
        }

        /// <summary>
        /// Sign Up For Seller API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        

     
    
      
    }
}
