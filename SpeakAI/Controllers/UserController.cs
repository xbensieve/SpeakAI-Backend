using BLL.Interface;
using Common.DTO;
using Common.Enum;
using Common.Message.UserMessage;
using DTO.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace Api_InnerShop.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get address of user by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        [SwaggerOperation(Summary = "Hello")]
        public async Task<IActionResult> GetUserByUserId(Guid userId)
        {
            var user = await _userService.GetUserResponseDtoByUserId(userId);
            if(user == null)
            {
                return NotFound(new ResponseDTO(UserMessage.UserIdNotExist, StatusCodeEnum.NotFound, false, null));
            }

            return Ok(new ResponseDTO(UserMessage.GetUserSuccessfully, StatusCodeEnum.OK, true, user));
        }
    }
}
