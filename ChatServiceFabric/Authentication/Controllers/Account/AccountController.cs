using AuthenticationDAL.BusinessRules.User;
using AuthenticationDAL.Constant;
using AuthenticationDAL.DTO.Common;
using AuthenticationDAL.Logger;
using AuthenticationDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static AuthenticationDAL.Constant.BusinessConstants;

namespace AuthenticationDAL.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        #region Common
        private readonly UserManager<IdentityUser> UserManager;
        private readonly SignInManager<IdentityUser> SignInManager;
        private readonly DtoToken TokenManager;
        protected IUser UserRepository;
        private readonly Log Logger;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IOptions<DtoToken> tokenManager, IUser userRepository, Log _logger)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            TokenManager = tokenManager.Value;
            UserRepository = userRepository;
            Logger = _logger;
        }

        #endregion


        #region Register Method

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(DtoInputData objInputData)
        {
            try
            {

                string inputString = Utilities.Utilities.DecryptFrontEndData(objInputData.InputString);
                DtoRegisterInput model = JsonConvert.DeserializeObject<DtoRegisterInput>(inputString);
                var Response = UserRepository.CreateUsers(model);
                if (Response.Result == "Success")
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    await SignInManager.SignInAsync(user, isPersistent: false);
                    var savedUser = await UserManager.FindByEmailAsync(model.Email);
                    return StatusCode(StatusCodes.Status200OK, new { Message = Response.Result });
                }
                else
                {
                    return StatusCode(StatusCodes.Status208AlreadyReported, new { Message = RegistrationStatus.CA0004 });
                }

            }
            catch (Exception ex)
            {
                Logger.ErrorLog("ChatService : Account ", "Register", ex);
                return StatusCode(StatusCodes.Status501NotImplemented, new { Common.CA0100 });
            }
        }

        #endregion

        #region Login
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(DtoInputData objInputData)
        {
            try
            {
                string inputString = Utilities.Utilities.DecryptFrontEndData(objInputData.InputString);
                DtoLogin? Model = JsonConvert.DeserializeObject<DtoLogin>(inputString);
                if (Model != null)
                {
                    var result = UserRepository.Login(Model);
                    if (result.Result != null)
                    {
                        Logger.InfoLog("User has been logged Successfully.");
                        return StatusCode(StatusCodes.Status200OK, result.Result);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status501NotImplemented, new { Message = LoginStatus.CA0006 });
                    }
                }
                return StatusCode(StatusCodes.Status501NotImplemented, new { Message = Common.CA0100 });

            }
            catch (Exception ex)
            {
                Logger.ErrorLog("ChatService : Account ", "Login", ex);
                return StatusCode(StatusCodes.Status501NotImplemented, new { Common.CA0100 });

            }
        }

        [HttpPost]
        [Route("OAuthLogin")]
        public IActionResult OAuthLogin(DtoInputData objInputData)
        {
            try
            {
                string inputString = Utilities.Utilities.DecryptFrontEndData(objInputData.InputString);
                DtoOAuthLogin Model = JsonConvert.DeserializeObject<DtoOAuthLogin>(inputString);
                var result = UserRepository.OAuthLogin(Model).Result;
                if (result.ErroMsg == null)
                {
                    Logger.InfoLog("Google User has been logged Successfully.");
                    return StatusCode(StatusCodes.Status200OK, result);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent, new { Message = result.ErroMsg });
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("ChatService : Account ", "OAuthLogin", ex);
                return StatusCode(StatusCodes.Status501NotImplemented, new { Common.CA0100 });
            }
        }
        #endregion
    }
}
