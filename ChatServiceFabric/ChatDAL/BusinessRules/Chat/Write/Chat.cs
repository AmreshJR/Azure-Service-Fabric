using ChatDAL.Constant;
using ChatDAL.DataModels;
using ChatDAL.DTO.Chat;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using Microsoft.EntityFrameworkCore;
using static ChatDAL.Constant.BusinessConstants;

namespace ChatDAL.BusinessRules.Chat.Write
{
    public class Chat : IChat
    {
        #region Common
        public ChatContext Context { get; set; }
        private readonly Log Logger;
        public Chat(Log _logger, ChatContext context)
        {

            Logger = _logger;
            Context = context;
        }

        #endregion


        #region Post Methods
        public (DtoPostResponse, string) PostMessage(DtoPostChatMessage ObjMessage)
        {
            try
            {

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
                PostNotification(Notify);

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
                    var response = new DtoPostResponse()
                    {
                        LastMessage = LastMessage,
                        SenderId = ObjMessage.SenderId,
                        ReceiverId = ObjMessage.ReceiverId,
                        MessageList = null
                    };

                    return (response, string.Empty);
                }
                else
                {
                    var SenderList = GetSelectedUserChat(ObjMessage.SenderId, ObjMessage.ReceiverId);
                    var ReceiverList = GetSelectedUserChat(ObjMessage.ReceiverId, ObjMessage.SenderId);
                    var response = new DtoPostResponse()
                    {
                        SenderId = ObjMessage.SenderId,
                        ReceiverId = ObjMessage.ReceiverId,
                        MessageList = new DtoReceiverList()
                        {
                            ForSender = SenderList,
                            ForReceiver = ReceiverList
                        }
                    };
                    return (response, string.Empty);
                }



            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : ChatRepository ", "PostMessage", ex);

                return (new DtoPostResponse(), ex.Message);
            }
        }

        public void PostNotification(DtoPostNotification ObjNotify)
        {
            try
            {
                Notification notification = new Notification();
                notification.NotificationTypeId = NotificationTypes.CA0007;
                notification.UserId = ObjNotify.ReceiverId;
                notification.Title = ObjNotify.Title;
                notification.Message = ObjNotify.Message;
                notification.CreatedOn = DateTime.UtcNow;
                notification.UpdatedOn = DateTime.UtcNow;
                notification.CreatedBy = ObjNotify.SenderId;
                notification.CommonId = ObjNotify.ChatId;

                Context.Notifications.Add(notification);
                Context.SaveChanges();

            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : ChatRepository ", "PostNotification", ex);

            }
        }
        #endregion

        #region Update Method
        public string ReadMessage(DtoReadMessage objReadMessage)
        {
            try
            {

                using (ChatContext dbContext = new ChatContext())
                {
                    var Conversation = dbContext.Chats.FirstOrDefault(x => x.ConversationUserIdList.Contains(objReadMessage.SenderId)
                                                                        && x.ConversationUserIdList.Contains(objReadMessage.ReceiverId));

                    if (Conversation != null)
                    {
                        var AllMessage = dbContext.ChatMessages.Where(x => x.ChatId == Conversation.ChatUid && x.SenderId != objReadMessage.SenderId && x.ReadAt == null).ToList();
                        var AllNotification = dbContext.Notifications.Where(x => x.CommonId == Conversation.ChatUid && x.UserId == long.Parse(objReadMessage.SenderId)).ToList();
                        if (AllMessage.Count > 0)
                        {
                            AllMessage.ForEach(Message =>
                            {
                                Message.ReadAt = DateTime.UtcNow;
                            });
                            dbContext.UpdateRange(AllMessage);
                            dbContext.SaveChanges();
                        }
                        if (AllNotification.Count > 0)
                        {
                            AllNotification.ForEach(Notify =>
                            {
                                Notify.IsReaded = true;
                            });
                            dbContext.UpdateRange(AllNotification);
                            dbContext.SaveChanges();
                        }
                    }
                }


                return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : ChatRepository ", "ReadMessage", ex);
                return ex.Message;

            }
        }

        public string ReadNotication(int NotificationId)
        {
            try
            {
                var notification = Context.Notifications.FirstOrDefault(x => x.NotificationId == NotificationId);
                if (notification != null)
                {
                    notification.UpdatedOn = DateTime.UtcNow;
                    notification.IsReaded = true;
                    Context.Notifications.Update(notification);
                    Context.SaveChanges();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : ChatRepository ", "ReadNotication", ex);
                return ex.Message;

            }
        }
        #endregion


        public DtoChat GetSelectedUserChat(string SenderId, string ReceiverId)
        {
            try
            {
                var chat = Context.Chats.FirstOrDefault(x => x.ConversationUserIdList.Contains(SenderId)
                                                                        && x.ConversationUserIdList.Contains(ReceiverId));
                var ChatUid = chat != null ? chat.ChatUid : 0;

                var Message = (from message in Context.ChatMessages
                               where message.ChatId == ChatUid
                               orderby message.CreatedOn
                               select new DtoMessage()
                               {
                                   ChatId = message.ChatId,
                                   MessageId = message.MessageId,
                                   SenderId = message.SenderId,
                                   ReceiverId = message.ReceiverId,
                                   MessageContent = message.MessageContent,
                                   ImageUrl = message.DocumentId,
                                   CreatedOn = message.CreatedOn,
                                   ReadAt = message.ReadAt,
                                   UpdatedOn = message.UpdatedOn,

                               }).ToList();
                var LastMessage = Message.Last();
                var senderId = long.Parse(SenderId);
                var receiverId = long.Parse(ReceiverId);
                var SenderDetail = Context.Users.FirstOrDefault(x => x.UserId == senderId);
                var ReceiverDetail = Context.Users.FirstOrDefault(x => x.UserId == receiverId);
                //Counting UnreadMesssage
                var UnreadMessagesCount = Message.Where(x => x.SenderId == ReceiverId.ToString() && x.ReadAt == null).ToList().Count;
                DateTime convertedDate = DateTime.SpecifyKind((LastMessage.CreatedOn ?? DateTime.UtcNow), DateTimeKind.Utc).ToLocalTime();
                DtoChat obj = new DtoChat()
                {
                    Message = Message,
                    UnreadMessageCount = UnreadMessagesCount,
                    LastMessage = LastMessage.MessageContent,
                    SenderName = SenderDetail?.UserName,
                    SenderId = SenderDetail?.UserId,
                    ReceiverName = ReceiverDetail?.UserName,
                    ReceiverId = ReceiverDetail?.UserId,
                    ReceiverProfilePicture = "",
                    MessageTime = convertedDate,
                    ChatId = ChatUid,
                    IsCompleteChat = true
                };
                return obj;
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : ChatRepository ", "GetSelectedUserChat", ex);

                return new DtoChat();
            }
        }


    }
}
