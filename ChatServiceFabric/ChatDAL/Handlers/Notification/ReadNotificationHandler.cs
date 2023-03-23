using ChatDAL.Command.Notification;
using ChatDAL.DataModels;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using MediatR;


namespace ChatDAL.Handlers.Notification
{
    public class ReadNotificationHandler : IRequestHandler<ReadNotificationCommand,string>
    {
        #region Common
        public ChatContext Context { get; set; }
        private readonly Log Logger;
        public ReadNotificationHandler(Log _logger, ChatContext context)
        {

            Logger = _logger;
            Context = context;
        }

        #endregion
        public async Task<string> Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var NotificationId = request.NotificationId;
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
    }
}
