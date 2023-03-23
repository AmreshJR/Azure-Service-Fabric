using ChatDAL.DTO.Chat;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using ChatDAL.Queries.Chat;
using ChatDAL.ReadDataModels;
using MediatR;

namespace ChatDAL.Handlers.Chat
{
    public class GetMessageHandler : IRequestHandler<GetMessageQuery, (DtoPagenationResponse,string)>
    {
        public ChatReadContext Context { get; set; }
        private readonly Log Logger;
        public GetMessageHandler(Log logger, ChatReadContext context)
        {
            Logger = logger;
            Context = context;
        }
        public async Task<(DtoPagenationResponse,string)> Handle(GetMessageQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var PageData = request.PagenationData;
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
    }
}
