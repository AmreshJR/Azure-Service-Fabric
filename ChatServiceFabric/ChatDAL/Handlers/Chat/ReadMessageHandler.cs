using ChatDAL.Command.Chat;
using ChatDAL.DataModels;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatDAL.Handlers.Chat
{
    public class ReadMessageHandler : IRequestHandler<ReadMessageCommand, string>
    {
        #region Common
        public ChatContext Context { get; set; }
        private readonly Log Logger;
        public ReadMessageHandler(Log _logger, ChatContext context)
        {

            Logger = _logger;
            Context = context;
        }

        #endregion
        public async Task<string> Handle(ReadMessageCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var objReadMessage = request.ReadMessage;
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
    }
}
