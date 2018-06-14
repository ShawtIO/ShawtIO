﻿using System;
using System.Collections.Generic;

namespace Links.Data
{
    public partial class Log
    {
        public Guid Id { get; set; }
        public int LinkId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime? Timestamp { get; set; }
        public string Browser { get; set; }
        public string Os { get; set; }
        public string Device { get; set; }

        public Link Link { get; set; }
    }
}
