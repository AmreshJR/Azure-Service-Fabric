using System;
using System.Collections.Generic;

namespace ChatDAL.DataModels
{
    public partial class AppConfiguration
    {
        public int AppConfigurationId { get; set; }
        public string? AppKey { get; set; }
        public string? AppValue { get; set; }
        public string? Group { get; set; }
    }
}
