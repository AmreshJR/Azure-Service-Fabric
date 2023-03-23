using System;
using System.ComponentModel.DataAnnotations;

namespace ChatDAL.DTO.Query
{
    public class DtoUspGetMessage
    {
        [Key]
        public long MessageId { get; set; }
        public long ChatId { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public string? MessageContent { get; set; }
        public string? DocumentId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
