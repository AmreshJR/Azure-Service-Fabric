namespace ChatDAL.DTO.Chat
{
    public class DtoGetAllUsers
    {
        public string userName { get; set; }
        public long userId { get; set; }
        public string profilePicture { get; set; }
    }

    public class DtoInput
    {
        public string UserId { get; set; }
    }

    public class DtoMessage
    {
        public long MessageId { get; set; }
        public long ChatId { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public string? MessageContent { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool EditMode { get; set; }
    }
    public class DtoChat
    {
        public List<DtoMessage> Message { get; set; }
        public int UnreadMessageCount { get; set; }
        public long? SenderId { get; set; }
        public long? ReceiverId { get; set; }
        public string? SenderName { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverProfilePicture { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? MessageTime { get; set; }
        public long ChatId { get; set; }
        public bool IsCompleteChat { get; set; }
    }

    public class DtoGroupedMessage
    {
        public List<DtoMessage> Message { get; set; }

        public long ChatId { get; set; }
    }

    public class DtoPostResponse
    {
        public DtoMessage LastMessage { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public DtoReceiverList? MessageList { get; set; }

    }

    public class DtoPostChatMessage
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string MessageContent { get; set; }

    }

    public class DtoReceiverList
    {
        public DtoChat? ForSender { get; set; }

        public DtoChat? ForReceiver { get; set; }
    }

    public class DtoReadMessage
    {
        public string SenderId { get; set; }

        public string ReceiverId { get; set; }
    }

    public class DtoPostNotification
    {
        public long ReceiverId { get; set; }

        public int NotificationTypeId { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public long SenderId { get; set; }

        public long ChatId { get; set; }

    }
    public class DtoNotification
    {
        public string? Message { get; set; }

        public string UserName { get; set; }

        public DateTime? CreatedOn { get; set; }

        public long UserId { get; set; }

        public int NotificationId { get; set; }

    }

    public class DtoNotificationList
    {
        public List<DtoNotification> NotificationList { get; set; }

        public int NotificationCount { get; set; }
    }

    public class DtoPagenationData
    {
        public int OldListCount { get; set; }

        public int NoOfData { get; set; }

        public long ChatId { get; set; }
    }

    public class DtoPagenationResponse
    {
        public List<DtoMessage> MessageList { get; set; }
        public bool IsListCompleted { get; set; }
    }
}
