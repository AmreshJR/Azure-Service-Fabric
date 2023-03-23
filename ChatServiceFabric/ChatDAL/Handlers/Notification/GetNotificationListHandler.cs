using ChatDAL.DTO.Chat;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using ChatDAL.Queries.Notification;
using ChatDAL.ReadDataModels;
using MediatR;

namespace ChatDAL.Handlers.Notification
{
    public class GetNotificationListHandler : IRequestHandler<GetNotificationListQuery, (DtoNotificationList,string)>
    {
        #region Common
        public ChatReadContext Context { get; set; }
        private readonly Log Logger;
        public GetNotificationListHandler(Log _logger, ChatReadContext context)
        {

            Logger = _logger;
            Context = context;
        }

        #endregion
        public async Task<(DtoNotificationList,string)> Handle(GetNotificationListQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var UserId = request.UserId;
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
                var notificationList = new DtoNotificationList
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
    }
}
