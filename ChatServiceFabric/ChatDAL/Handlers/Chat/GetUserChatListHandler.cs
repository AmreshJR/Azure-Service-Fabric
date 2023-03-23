using ChatDAL.DataModels;
using ChatDAL.DTO.Chat;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using ChatDAL.Queries.Chat;
using ChatDAL.ReadDataModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ChatDAL.Handlers.Chat
{
    public class GetUserChatListHandler : IRequestHandler<GetUserChatListQuery, (List<DtoChat>,string)>
    {
        #region Common
        public ChatReadContext Context { get; set; }
        private readonly Log Logger;
        public GetUserChatListHandler(Log _logger, ChatReadContext context)
        {

            Logger = _logger;
            Context = context;
        }

        #endregion
        public async Task<(List<DtoChat>,string)> Handle(GetUserChatListQuery request, CancellationToken cancellationToken)
        {
            List<DtoChat> ChatList = new List<DtoChat>();
            try
            {

                var userId = request.UserId.ToString();

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
                return (ChatList, ex.Message);
            }

        }
    }
}
