using ChatDAL.Command.Chat;
using ChatDAL.DataModels;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChatDAL.Constant.BusinessConstants;

namespace ChatDAL.Handlers.Chat
{
    public class PostNotificationHandler : IRequestHandler<PostNotificationCommand, Unit>
    {
        #region Common
        public ChatContext Context { get; set; }
        private readonly Log Logger;
        public PostNotificationHandler(Log _logger, ChatContext context)
        {

            Logger = _logger;
            Context = context;
        }

        #endregion
        public async Task<Unit> Handle(PostNotificationCommand request, CancellationToken cancellationToken)
        {
            var ObjNotify = request.DtoNotification;
            try
            {
                DataModels.Notification notification = new DataModels.Notification
                {
                    NotificationTypeId = NotificationTypes.CA0007,
                    UserId = ObjNotify.ReceiverId,
                    Title = ObjNotify.Title,
                    Message = ObjNotify.Message,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    CreatedBy = ObjNotify.SenderId,
                    CommonId = ObjNotify.ChatId
                };

                Context.Notifications.Add(notification);
                Context.SaveChanges();

            }
            catch (Exception ex)
            {
                Logger.ErrorLog("RealTimeChatService : ChatRepository ", "PostNotification", ex);

            }
            return Unit.Value;
        }
    }
}
