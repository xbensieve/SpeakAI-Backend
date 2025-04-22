using Common.DTO;
using DTO.DTO;

namespace BLL.Interface
{
    public interface ILoginService
    {
  
        LoginResponseDTO? Login(LoginRequestDTO loginRequestDTO);
        TokenDTO RefreshAccessToken(RequestTokenDTO tokenDTO);
        ResponseDTO Logout(string userid);
        ResponseDTO GetUserByAccessToken(string accessToken);
        LocalUserDTO GetUserByUserId(string userId);
        Task<ResponseDTO> SignInWithGoogle(GoogleAuthTokenDTO googleAuthToken);
    }
}
