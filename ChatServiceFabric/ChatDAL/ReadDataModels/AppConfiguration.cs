using System;
using System.Collections.Generic;

namespace ChatDAL.ReadDataModels
{
    public partial class AppConfiguration
    {
        public int AppConfigurationId { get; set; }
        public string? AppKey { get; set; }
        public string? AppValue { get; set; }
        public string? Group { get; set; }
    }
}
