using AuthenticationDAL.Constant;
using AuthenticationDAL.DataModels;
using AuthenticationDAL.DTO.Common;
using AuthenticationDAL.Logger;
using AuthenticationDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static AuthenticationDAL.Constant.BusinessConstants;

namespace AuthenticationDAL.BusinessRules.User
{
    public class User :IUser
    {

        #region Common
        public AuthenticationContext Context { get; set; }
        private readonly UserManager<IdentityUser> UserManager;
        private readonly SignInManager<IdentityUser> SignInManager;
        private readonly DtoToken TokenManager;
        private readonly Log Logger;
        public User(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IOptions<DtoToken> tokenManager, Log _logger)
        {
            Context = new AuthenticationContext();
            UserManager = userManager;
            SignInManager = signInManager;
            TokenManager = tokenManager.Value;
            Logger = _logger;
        }
        #endregion

        #region Register
        public async Task<string> CreateUsers(DtoRegisterInput ObjRegister)
        {
            try
            {
                var IsUserExist = GetUserByEmailId(ObjRegister.Email).Email;
                string result;

                if (!string.IsNullOrWhiteSpace(ObjRegister.Email) && IsUserExist == null)
                {
                    var user = new ApplicationUser { UserName = ObjRegister.Email, Email = ObjRegister.Email };
                    var UserCreated = await UserManager.CreateAsync(user, ObjRegister.Password);
                    if (UserCreated.Succeeded)
                    {
                        AuthenticationDAL.DataModels.User objUser = new AuthenticationDAL.DataModels.User();
                        AspNetUser netusers;
                        netusers = GetUserByEmailId(ObjRegister.Email);
                        string userAuthid = netusers.Id;
                        objUser.AuthId = netusers.Id;
                        objUser.UserName = ObjRegister.UserName;
                        objUser.EmailId = ObjRegister.Email;
                        objUser.CreatedOn = DateTime.UtcNow;
                        objUser.IsExternal = false;
                        objUser.Status = BusinessConstants.Status.Active;
                        Context.Users.Add(objUser);
                        Context.SaveChanges();
                        result = RegistrationStatus.CA0001;
                        return result;
                    }
                }
                result = RegistrationStatus.CA0002;
                return result;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : UserRepository ", "GetNotificationList", ex);
                return string.Empty;
            }
        }
        #endregion

        #region Login

        public async Task<DtoLoginResponse> Login(DtoLogin LoginData)
        {
            try
            {
                DtoLoginResponse Response = null;
                var User = await UserManager.FindByEmailAsync(LoginData.Email);
                var UserDetail = Context.Users.FirstOrDefault(x => x.EmailId == LoginData.Email);
                if (User != null && await UserManager.CheckPasswordAsync(User, LoginData.Password) && UserDetail != null)
                {
                    var Token = GenerateToken(UserDetail);
                    Response = new DtoLoginResponse()
                    {
                        Token = Token,
                        UserName = UserDetail.UserName,
                        UserId = UserDetail.UserId
                    };
                }
                return Response;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : UserRepository ", "Login", ex);

                return new DtoLoginResponse();
            }
        }

        public string GenerateToken(AuthenticationDAL.DataModels.User UserDetail)
        {
            try
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {

                    Subject = new ClaimsIdentity(new Claim[]
    {
                        new Claim("UserID",UserDetail.UserName),
                        new Claim("UserLoginId",UserDetail.UserId.ToString()),
    }
    ),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenManager.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return token;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : UserRepository ", "GenerateToken", ex);

                return string.Empty;
            }
        }

        public async Task<DtoLoginResponse> OAuthLogin(DtoOAuthLogin LoginData)
        {
            DtoLoginResponse response = new();
            try
            {

                GoogleJsonWebSignature.ValidationSettings settings = new();
                settings.Audience = new List<string> { "468618361803-deviechkcmr3asjciqm0kfr00rovcn0e.apps.googleusercontent.com" };
                var _payload = await GoogleJsonWebSignature.ValidateAsync(LoginData.IdToken, settings);
                if (_payload != null)
                {
                    var _user = Context.Users.FirstOrDefault(x => x.EmailId == _payload.Email);
                    if (!_payload.EmailVerified)
                    {
                        response.ErroMsg = "Email not verified";
                    }
                    else if (_user != null && !_user.IsExternal)
                    {
                        response.ErroMsg = "Email already Exist, Try login with email & password.";
                    }
                    else if (_user != null && _user.IsExternal)
                    {
                        var token = GenerateToken(_user);
                        response.Token = token;
                        response.UserName = _user.UserName;
                        response.UserId = _user.UserId;
                    }
                    else
                    {
                        AuthenticationDAL.DataModels.User newUser = new()
                        {
                            UserName = _payload.Name,
                            EmailId = _payload.Email,
                            CreatedOn = DateTime.UtcNow,
                        };
                        Context.Add(newUser);
                        Context.SaveChanges();
                        var token = GenerateToken(newUser);
                        response.Token = token;
                        response.UserName = newUser.UserName;
                        response.UserId = newUser.UserId;
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                response.ErroMsg = ex.Message;
                Logger.ErrorLog("RealTimeChatService : UserRepository ", "OAuthLogin", ex);

                return response;
            }
        }
        #endregion

        #region GetNetUser
        public AspNetUser GetUserByEmailId(string emailId)
        {
            return Context.AspNetUsers.FirstOrDefault(x => x.Email == emailId) ?? new AspNetUser();
        }
        public string GetusernameByAuthid(string authid)
        {
            return Context.AspNetUsers.FirstOrDefault(x => x.Id == authid).UserName ?? "";
        }
        public AuthenticationDAL.DataModels.User GetUserById(int Id)
        {
            return Context.Users.FirstOrDefault(x => x.UserId == Id) ?? new AuthenticationDAL.DataModels.User();
        }
        public AspNetUser GetUserByUserName(string UserName)
        {
            return Context.AspNetUsers.FirstOrDefault(x => x.UserName.Equals(UserName, StringComparison.OrdinalIgnoreCase)) ?? new AspNetUser();
        }
        #endregion

    }
}
