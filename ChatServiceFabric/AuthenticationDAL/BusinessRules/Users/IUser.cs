using AuthenticationDAL.DTO.Common;


namespace AuthenticationDAL.BusinessRules.User
{
    public interface IUser
    {
        Task<string> CreateUsers(DtoRegisterInput ObjRegister);
        Task<DtoLoginResponse> Login(DtoLogin LoginData);
        Task<DtoLoginResponse> OAuthLogin(DtoOAuthLogin LoginData); 

    }
}
