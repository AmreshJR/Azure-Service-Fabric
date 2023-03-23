using Azure;
using ChatDAL.Command.Chat;
using ChatDAL.Constant;
using ChatDAL.DataModels;
using ChatDAL.DTO.Chat;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using ChatDAL.Queries.Chat;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenAI.GPT3.ObjectModels.ResponseModels;
using System.Fabric;
using static ChatDAL.Constant.BusinessConstants;

namespace ChatDAL.Handlers.Chat
{
    public class PostMessageHandler : IRequestHandler<PostMessageCommand, string>
    {
        public ChatContext Context { get; set; }
        private readonly Log Logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StatelessServiceContext StatelessContext;
        private readonly ISender Sender;
        private readonly string Token;
        public PostMessageHandler(Log logger, IHttpContextAccessor httpContextAccessor, ChatContext context, StatelessServiceContext statelessContext, ISender sender)
        {
            Logger = logger;
            _httpContextAccessor = httpContextAccessor;
            Context = context;
            Token = GetToken();
            StatelessContext = statelessContext;
            Sender = sender;
        }
        public async Task<string> Handle(PostMessageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var ObjMessage = request.Message;
                var PostData = new DtoPostResponse();
                var conversation = Context.Chats.FirstOrDefault(x => x.ConversationUserIdList.Contains(ObjMessage.SenderId)
                                                                        && x.ConversationUserIdList.Contains(ObjMessage.ReceiverId));
                long ConversationId;
                var IsNewChat = false;
                if (conversation == null)
                {
                    var userList = new List<string>();
                    userList.Add(ObjMessage.SenderId);
                    userList.Add(ObjMessage.ReceiverId);
                    var ChatData = new DataModels.Chat()
                    {
                        ConversationUserIdList = string.Join(',', userList),
                        Status = BusinessConstants.Status.Active
                    };
                    Context.Chats.Add(ChatData);
                    Context.SaveChanges();
                    ConversationId = ChatData.ChatUid;
                    IsNewChat = true;
                }
                else
                {
                    ConversationId = conversation.ChatUid;
                }

                var ChatMessageData = new ChatMessage()
                {
                    ChatId = ConversationId,
                    MessageContent = ObjMessage.MessageContent,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    SenderId = ObjMessage.SenderId,
                    ReceiverId = ObjMessage.ReceiverId,
                    Status = BusinessConstants.Status.Active

                };
                Context.ChatMessages.Add(ChatMessageData);
                Context.SaveChanges();

                var Notify = new DtoPostNotification()
                {
                    SenderId = long.Parse(ObjMessage.SenderId),
                    ReceiverId = long.Parse(ObjMessage.ReceiverId),
                    Message = ObjMessage.MessageContent,
                    Title = "New Message.",
                    NotificationTypeId = NotificationTypes.CA0007,
                    ChatId = ConversationId
                };
                await Sender.Send(new PostNotificationCommand(Notify));

                if (!IsNewChat)
                {
                    var LastMessage = Context.ChatMessages.AsNoTracking()
                   .Where(m => m.ChatId == ConversationId)
                   .OrderBy(m => m.CreatedOn)
                   .Select(m => new DtoMessage()
                   {
                       ChatId = m.ChatId,
                       MessageId = m.MessageId,
                       SenderId = m.SenderId,
                       ReceiverId = m.ReceiverId,
                       MessageContent = m.MessageContent,
                       ImageUrl = m.DocumentId,
                       CreatedOn = m.CreatedOn,
                       ReadAt = m.ReadAt,
                       UpdatedOn = m.UpdatedOn,
                   }).LastOrDefault() ?? new DtoMessage();
                    /*                    DateTime convertedDate = DateTime.SpecifyKind((DateTime)LastMessage.CreatedOn, DateTimeKind.Utc).ToLocalTime();
                                        LastMessage.CreatedOn = convertedDate;*/
                    PostData = new DtoPostResponse()
                    {
                        LastMessage = LastMessage,
                        SenderId = ObjMessage.SenderId,
                        ReceiverId = ObjMessage.ReceiverId,
                        MessageList = null
                    };

                }
                else
                {
                    var SenderList = Sender.Send(new GetSelectedUserChatQuery(ObjMessage.SenderId, ObjMessage.ReceiverId)).Result;
                    var ReceiverList =Sender.Send(new GetSelectedUserChatQuery(ObjMessage.ReceiverId, ObjMessage.SenderId)).Result;
                    PostData = new DtoPostResponse()
                    {
                        SenderId = ObjMessage.SenderId,
                        ReceiverId = ObjMessage.ReceiverId,
                        MessageList = new DtoReceiverList()
                        {
                            ForSender = SenderList,
                            ForReceiver = ReceiverList
                        }
                    };

                }
                if (PostData.MessageList == null) PostActiveSignalr(PostData);

                else SignalrPostNewMessage(PostData);

                TriggerInAppNotification(request.Message.ReceiverId);

                return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : ChatRepository ", "PostMessage", ex);

                return ex.Message;
            }
        }
        #region SignalR Methods

        public async void PostActiveSignalr(DtoPostResponse objPostChat)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var accessToken = Token;
                    client.DefaultRequestHeaders.Add("Authorization", accessToken);

                    var IpOrDomine = StatelessContext.NodeContext.IPAddressOrFQDN;
                    var serviceUrl = $"http://{IpOrDomine}:19081/ChatServiceFabric/NotificationService/api/Notification/SignalrPostMessage";
                    HttpResponseMessage response = await client.PostAsJsonAsync(serviceUrl, objPostChat);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("PostMessageHandler", "PostActiveSignalr", ex);
            }

        }
        public async void SignalrPostNewMessage(DtoPostResponse objPostChat)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var accessToken = Token;
                    client.DefaultRequestHeaders.Add("Authorization", accessToken);

                    var IpOrDomine = StatelessContext.NodeContext.IPAddressOrFQDN;
                    var serviceUrl = $"http://{IpOrDomine}:19081/ChatServiceFabric/NotificationService/api/Notification/SignalrPostNewMessage";
                    HttpResponseMessage response = await client.PostAsJsonAsync(serviceUrl, objPostChat);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("PostMessageHandler", "SignalrPostNewMessage", ex);
            }
        }

        public async void TriggerInAppNotification(string UserId)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var accessToken = Token;
                    client.DefaultRequestHeaders.Add("Authorization", accessToken);
                    var data = new DtoInput() {UserId = UserId};
                    var IpOrDomine = StatelessContext.NodeContext.IPAddressOrFQDN;
                    var serviceUrl = $"http://{IpOrDomine}:19081/ChatServiceFabric/NotificationService/api/Notification/TriggerInAppNotification";
                    HttpResponseMessage response = await client.PostAsJsonAsync(serviceUrl, data);
                }

            }
            catch (Exception ex)
            {
                Logger.ErrorLog("PostMessageHandler", "TriggerInAppNotification", ex);
            }
        }

        public string GetToken()
        {
            var token = string.Empty;
            try
            {
                var headers = _httpContextAccessor.HttpContext.Request.Headers;
                if (headers.ContainsKey("Authorization"))
                {
                    var authorizationHeader = headers["Authorization"].FirstOrDefault();
                    if (!string.IsNullOrEmpty(authorizationHeader))
                    {
                        var authHeaderParts = authorizationHeader.Split(' ');
                        if (authHeaderParts.Length == 2 && authHeaderParts[0].ToLower() == "bearer")
                        {

                            token = authHeaderParts[1];
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.ErrorLog("PostMessageHandler", "GetToken", ex);

            }
            return token;
            #endregion
        }
    }
}
