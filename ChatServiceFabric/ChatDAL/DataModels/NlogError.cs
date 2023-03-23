using System;
using System.Collections.Generic;

namespace ChatDAL.DataModels
{
    public partial class NlogError
    {
        public int Id { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string? Level { get; set; }
        public string? Host { get; set; }
        public string? Type { get; set; }
        public string? Logger { get; set; }
        public string? Message { get; set; }
        public string? Stacktrace { get; set; }
    }
}
