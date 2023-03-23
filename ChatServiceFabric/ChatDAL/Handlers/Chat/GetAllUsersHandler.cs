using ChatDAL.DTO.Chat;
using ChatDAL.Logger.MicroFrontendDal.BusinessRules.Logger;
using ChatDAL.Queries.Chat;
using ChatDAL.ReadDataModels;
using MediatR;


namespace ChatDAL.Handlers.Chat
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, (List<DtoGetAllUsers>,string)>
    {
        #region Common
        public ChatReadContext Context { get; set; }
        private readonly Log Logger;
        public GetAllUsersHandler( Log logger, ChatReadContext context)
        {
            Logger = logger;
            Context = context;
        }

        #endregion
        public async Task<(List<DtoGetAllUsers>, string)> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
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
    }
}
