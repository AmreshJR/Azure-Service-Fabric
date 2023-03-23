using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NotificationService.BusinessRules.Hub;
using NotificationService.DTO.Chat;
using NotificationService.DTO.Common;
using NotificationService.Hubs;
using NotificationService.Logger;

namespace NotificationService.Controllers.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {

        #region Common
        private readonly IHubContext<ChatHub> HubContext;
        private readonly ConnectionMapping ConnectionList;
        private readonly Log Logger;
        public NotificationController(IHubContext<ChatHub> hubContext, ConnectionMapping connectionList, Log _logger)
        {
            HubContext = hubContext;
            ConnectionList = connectionList;
            Logger = _logger;
        }
        #endregion

        #region SignalR Methods
        [HttpPost]
        [Route("SignalrPostMessage")]
        public void PostActiveSignalr(DtoPostResponse objPostChat)
        {
            try
            {
                Task.Run(() =>
                {
                    var ActiveList = ConnectionList.GetConnectionArray();

                    var SenderList = ActiveList.Where(x => (x.Key == objPostChat.SenderId || x.Key == objPostChat.ReceiverId) && x.Value.Count > 0).Select(x => x.Value.ToList()).ToList();

                    if (SenderList.Count > 0)
                    {
                        SenderList.ForEach(async ConnectionIds =>
                        {
                            await HubContext.Clients.Clients(ConnectionIds).SendAsync("updateChatList", objPostChat.LastMessage);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogToTables(DtoLog.Error, "RealTimeChatService : Chat : SignalrPostMessage", ex.Message, "Problem in SignalrPostMessage method");
            }

        }
        [HttpPost]
        [Route("SignalrPostNewMessage")]
        public void SignalrPostNewMessage(DtoPostResponse objPostChat)
        {
            try
            {
                Task.Run(() =>
                {
                    var ActiveList = ConnectionList.GetConnectionArray();

                    var ForSender = objPostChat.MessageList?.ForSender;

                    var ForReceiver = objPostChat.MessageList?.ForReceiver;


                    var SenderList = ActiveList.Where(active => (active.Key == ForSender?.SenderId.ToString())
                                                                && active.Value.Count > 0).Select(x => x.Value.ToList())
                                                                .ToList();

                    var ReceiverList = ActiveList.Where(active => (active.Key == ForReceiver?.SenderId.ToString())
                                                                && active.Value.Count > 0).Select(x => x.Value.ToList())
                                                                .ToList();

                    if (SenderList.Count > 0)
                    {
                        SenderList.ForEach(async ConnectionIds =>
                        {
                            await HubContext.Clients.Clients(ConnectionIds).SendAsync("updateNewChat", ForSender);
                        });
                    }
                    if (ReceiverList.Count > 0)
                    {
                        ReceiverList.ForEach(async ConnectionIds =>
                        {
                            await HubContext.Clients.Clients(ConnectionIds).SendAsync("updateNewChat", ForReceiver);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogToTables(DtoLog.Error, "RealTimeChatService : Chat : SignalrPostNewMessage", ex.Message, "Problem in SignalrPostNewMessage method");
            }
        }

        [HttpPost]
        [Route("TriggerInAppNotification")]
        public void TriggerInAppNotification(DtoInput objInput)
        {
            try
            {
                var ActiveConnection = ConnectionList.GetConnectionArray();

                var IsUserActive = ActiveConnection.FirstOrDefault(x => x.Key == objInput.UserId);

                if (IsUserActive.Value?.Count > 0)
                {
                    var ConnectionIds = IsUserActive.Value.ToList();
                    HubContext.Clients.Clients(ConnectionIds).SendAsync("inAppNotification");
                }

            }
            catch (Exception ex)
            {
                Logger.LogToTables(DtoLog.Error, "RealTimeChatService : Chat : TriggerInAppNotification", ex.Message, "Problem in TriggerInAppNotification method");
            }
        }
        #endregion
    }
}
