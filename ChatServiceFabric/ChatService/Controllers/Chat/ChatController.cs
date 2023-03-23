using ChatDAL.DTO.Chat;
using ChatDAL.DTO.Common;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using ChatDAL.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static ChatDAL.Constant.BusinessConstants;
using MediatR;
using ChatDAL.Queries.Chat;
using ChatDAL.Queries.Notification;
using ChatDAL.Command.Notification;
using ChatDAL.Command.Chat;

namespace ChatService.Controllers.Chat
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class ChatController : ControllerBase
    {
        #region Common
        private readonly Log Logger;
        private readonly ISender Sender;
        public ChatController(Log _logger, ISender sender)
        {
            Logger = _logger;
            Sender = sender;
        }
        #endregion

        #region Get Methods
        /// <summary>
        /// </summary>
        /// <returns>Return the all users </returns>
        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                //var result = ChatRepository.GetAllUsers();
                var result = Sender.Send(new GetAllUsersQuery()).Result;
                if (!string.IsNullOrWhiteSpace(result.Item2))
                {
                    return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });
                }
                return StatusCode(StatusCodes.Status200OK, result.Item1);
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("ChatService : Chat ", "GetAllUsers", ex);
                return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>Returns the list of notificaition for the specific user.</returns>
        [HttpGet]
        [Route("GetNotificationList")]
        public IActionResult GetNotificationList(long UserId)
        {
            try
            {

                var result = Sender.Send(new GetNotificationListQuery(UserId)).Result;
                if (!string.IsNullOrEmpty(result.Item2))
                {
                    return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });
                }
                return StatusCode(StatusCodes.Status200OK, result.Item1);
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("ChatService : Chat ", "GetNotificationList", ex);
                return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });
            }
        }

 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>Retrun all the conversation of the specific user.</returns>
        [HttpGet]
        [Route("GetUserChatList")]
        public IActionResult GetUserChatList(long UserId)
        {
            try
            {
                var result = Sender.Send(new GetUserChatListQuery(UserId)).Result;
                if (!string.IsNullOrWhiteSpace(result.Item2))
                {
                    return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });
                }
                //var Response = ChatRepository.GetUserChatList(UserId);
                return StatusCode(StatusCodes.Status200OK, result.Item1);

            }
            catch (Exception ex)
            {
                Logger.ErrorLog("ChatService : Chat ", "GetUserChatList", ex);
                return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PageData"></param>
        /// <returns>Return the messages of specific conversation.</returns>
        [HttpPost]
        [Route("GetMessage")]
        public IActionResult GetMessage(DtoPagenationData PageData)
        {
            try
            {
                var result = Sender.Send(new GetMessageQuery(PageData)).Result;
                if (!string.IsNullOrWhiteSpace(result.Item2))
                {
                    return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });
                }
                return StatusCode(StatusCodes.Status200OK, result.Item1);
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("ChatService : Chat ", "GetMessage", ex);
                return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });
            }
        }
        #endregion

        #region Post Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objInputData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostMessage")]
        public IActionResult PostMessage(DtoInputData objInputData)
        {
            try
            {
                var inputString = Utilities.DecryptFrontEndData(objInputData.InputString);
                var PostData = JsonConvert.DeserializeObject<DtoPostChatMessage>(inputString);
                if (PostData != null)
                {
                    var result = Sender.Send(new PostMessageCommand(PostData)).Result;
                    if (string.IsNullOrWhiteSpace(result))
                        return StatusCode(StatusCodes.Status200OK);

                }
                return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("ChatService : Chat ", "PostMessage", ex);
                return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ObjPost"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ReadMessage")]
        public IActionResult ReadMessage(DtoReadMessage ObjPost)
        {
            try
            {
                var result = Sender.Send(new ReadMessageCommand(ObjPost)).Result;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });
                }
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("ChatService : Chat ", "ReadMessage", ex);
                return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="NotificationId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ReadNotification")]
        public IActionResult ReadNotification(int NotificationId)
        {
            try
            {
                var result = Sender.Send(new ReadNotificationCommand(NotificationId)).Result;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });
                }
                return StatusCode(StatusCodes.Status200OK, new { Message = "Notificatio Readed." });

            }
            catch (Exception ex)
            {
                Logger.ErrorLog("ChatService : Chat ", "ReadNotificationReadNotification", ex);
                return StatusCode(StatusCodes.Status501NotImplemented, new { Messgae = Common.CA0100 });
            }
        }
        #endregion


    }
}
