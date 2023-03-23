using ChatDAL.Constant;
using ChatDAL.DataModels;
using ChatDAL.DTO.Chat;
using ChatDAL.DTO.Common;
using ChatDAL.Logger;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using ChatDAL.ReadDataModels;
using Microsoft.EntityFrameworkCore;
using static ChatDAL.Constant.BusinessConstants;

namespace ChatDAL.BusinessRules.Chat.Read
{
    public class Chat : IChat
    {
        #region Common
        public ChatReadContext Context { get; set; }
        private readonly Log Logger;
        public Chat(Log _logger, ChatReadContext context)
        {

            Logger = _logger;
            Context = context;
        }

        #endregion

        #region Get Methods
        public (List<DtoGetAllUsers>, string ErrorMsg) GetAllUsers()
        {
            try
            {
                var Users = (from user in Context.Users
                             select new DtoGetAllUsers()
                             {
                                 userId = user.UserId,
                                 userName = user.UserName,
                                 profilePicture = ""
                             }).ToList();
                return (Users, string.Empty);
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : ChatRepository ", "GetAllUsers", ex);
                return (new List<DtoGetAllUsers>(), ex.Message);
            }
        }

        public (DtoNotificationList, string) GetNotificationList(long UserId)
        {
            try
            {
                var Notification = (from notification in Context.Notifications
                                    join user in Context.Users on notification.CreatedBy equals user.UserId
                                    where notification.UserId == UserId && notification.IsReaded == false
                                    orderby notification.CreatedOn descending 
                                    select new DtoNotification()
                                    {
                                        NotificationId = notification.NotificationId,
                                        UserName = user.UserName,
                                        CreatedOn = notification.CreatedOn,
                                        Message = notification.Message,
                                        UserId = notification.CreatedBy


                                    }).ToList();
                var notificationList = new DtoNotificationList()
                {
                    NotificationList = Notification,
                    NotificationCount = Notification.Count,
                };
                return (notificationList, string.Empty);
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : ChatRepository ", "GetNotificationList", ex);
                return (new DtoNotificationList(), ex.Message);
            }
        }

        public (List<DtoChat>, string) GetUserChatList(long UserId)
        {
            List<DtoChat> ChatList = new List<DtoChat>();
            try
            {
                var userId = UserId.ToString();

                using (AppDbContext dbContext = new())
                {
                    var GroupedMessage = dbContext.GetMessages.FromSqlRaw("USP_GetGroupedMessage  {0}", userId).ToList().Select(message => new DtoMessage
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
                        EditMode = false,
                    }).GroupBy(Chat => Chat.ChatId).Select((message) => message.OrderBy(x => x.CreatedOn).ToList()).ToList();

                    GroupedMessage.ForEach(message =>
                    {
                        /*                        message.ForEach(x =>
                                                {
                                                    DateTime convertedDate = DateTime.SpecifyKind((DateTime)x.CreatedOn, DateTimeKind.Utc).ToLocalTime();
                                                    x.CreatedOn = convertedDate;
                                                });*/
                        var LastMessage = message.Last();
                        var SenderId = long.Parse(userId);
                        var _recevierString = LastMessage.ReceiverId == userId ? LastMessage.SenderId : LastMessage.ReceiverId;
                        var ReceiverId = _recevierString != null ? long.Parse(_recevierString) : 0;
                        var SenderDetail = Context.Users.FirstOrDefault(x => x.UserId == SenderId);
                        var ReceiverDetail = Context.Users.FirstOrDefault(x => x.UserId == ReceiverId);
                        //Counting UnreadMesssage
                        var UnreadMessagesCount = message.Where(x => x.SenderId == ReceiverId.ToString() && x.ReadAt == null).ToList().Count;
                        DtoChat obj = new DtoChat()
                        {
                            Message = message,
                            UnreadMessageCount = UnreadMessagesCount,
                            LastMessage = LastMessage.MessageContent,
                            SenderName = SenderDetail?.UserName,
                            SenderId = SenderDetail?.UserId,
                            ReceiverName = ReceiverDetail?.UserName,
                            ReceiverId = ReceiverDetail?.UserId,
                            ReceiverProfilePicture = "",
                            MessageTime = LastMessage.CreatedOn,
                            ChatId = LastMessage.ChatId,
                            IsCompleteChat = message.Count == 10
                        };
                        ChatList.Add(obj);
                    });
                    List<DtoChat> SortedChatList = ChatList.OrderByDescending(x => x.MessageTime).ToList();
                    return (SortedChatList, string.Empty);
                }

            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : ChatRepository ", "GetUserChatList", ex);
                return (new List<DtoChat>(), ex.Message);
            }

        }

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
                                   EditMode = false
                               }).ToList();
                var LastMessage = Message.Last();
                var senderId = long.Parse(SenderId);
                var receiverId = long.Parse(ReceiverId);
                var SenderDetail = Context.Users.FirstOrDefault(x => x.UserId == senderId);
                var ReceiverDetail = Context.Users.FirstOrDefault(x => x.UserId == receiverId);
                //Counting UnreadMesssage
                var UnreadMessagesCount = Message.Where(x => x.SenderId == ReceiverId.ToString() && x.ReadAt == null).ToList().Count;
                DateTime convertedDate = DateTime.SpecifyKind((DateTime)LastMessage.CreatedOn, DateTimeKind.Utc).ToLocalTime();
                DtoChat obj = new DtoChat()
                {
                    Message = Message,
                    UnreadMessageCount = UnreadMessagesCount,
                    LastMessage = LastMessage.MessageContent,
                    SenderName = SenderDetail.UserName,
                    SenderId = SenderDetail.UserId,
                    ReceiverName = ReceiverDetail.UserName,
                    ReceiverId = ReceiverDetail.UserId,
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


        public (DtoPagenationResponse, string) GetMessage(DtoPagenationData PageData)
        {
            try
            {
                var Message = Context.ChatMessages.Where(x => x.ChatId == PageData.ChatId)
                    .OrderByDescending(x => x.CreatedOn).Skip(PageData.OldListCount).Take(PageData.NoOfData)
                    .Select(message => new DtoMessage()
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
                        EditMode = false
                    }).OrderBy(x => x.CreatedOn).ToList();

                var isListCompleted = Message.Count == 0;
                DtoPagenationResponse response = new()
                {
                    MessageList = Message,
                    IsListCompleted = isListCompleted
                };

                return (response, string.Empty);
            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : ChatRepository ", "GetMessage", ex);

                return (new DtoPagenationResponse(), ex.Message);
            }
        }
        #endregion




    }
}
