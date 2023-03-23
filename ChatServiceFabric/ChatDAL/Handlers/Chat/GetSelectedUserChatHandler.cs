using ChatDAL.DTO.Chat;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using ChatDAL.Queries.Chat;
using ChatDAL.ReadDataModels;
using MediatR;

namespace ChatDAL.Handlers.Chat
{
    public class GetSelectedUserChatHandler : IRequestHandler<GetSelectedUserChatQuery, DtoChat>
    {
        #region Common
        public ChatReadContext Context { get; set; }
        private readonly Log Logger;
        public GetSelectedUserChatHandler(Log _logger, ChatReadContext context)
        {

            Logger = _logger;
            Context = context;
        }

        #endregion
        public async Task<DtoChat> Handle(GetSelectedUserChatQuery request, CancellationToken cancellationToken)
        {
           try
            {
                var SenderId = request.SenderId;
                var ReceiverId = request.ReceiverId;
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
                Logger.ErrorLog("Handler | GetSelectedUserChat ", "GetSelectedUserChat", ex);
                return new DtoChat();
            }
        }
    }
}
