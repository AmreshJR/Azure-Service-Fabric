using ChatDAL.BusinessRules;
using ChatDAL.BusinessRules.ChatGPT;
using ChatDAL.DTO.Common;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using ChatDAL.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Fabric;
using static ChatDAL.Constant.BusinessConstants;
using static ChatDAL.DTO.ChatGPT.ChatGPT;

namespace ChatService.Controllers.ChatGPT
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatGPTController : ControllerBase
    {
        #region Common
        private readonly IChatGPT ChatGPTRepository;
        private readonly Log Logger;
        private readonly StatelessServiceContext Context;
        public ChatGPTController(IChatGPT chatGPTRepository, Log _logger, StatelessServiceContext Context)
        {
            ChatGPTRepository = chatGPTRepository;
            Logger = _logger;
            this.Context = Context;
        }
        #endregion
        #region Get Methods
        [HttpPost]
        [Route("GetGPTResul")]
        public IActionResult GetGPTResul(DtoInputData dtoInputData)
        {
            try
            {
                var inputString = Utilities.DecryptFrontEndData(dtoInputData.InputString);
                var PostData = JsonConvert.DeserializeObject<DtoGPTQuestion>(inputString);
                var result = ChatGPTRepository.TriggerChatGPT(PostData.Question).Result;
                var res = new { Answer = result };

                //var encData = Utilities.EncryptString(JsonConvert.SerializeObject(result));
                return StatusCode(StatusCodes.Status200OK, res);
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("ChatService : Chat ", "GetAllUsers", ex);
                return StatusCode(StatusCodes.Status501NotImplemented, Common.CA0100);
            }
        }
        #endregion
    }
}
