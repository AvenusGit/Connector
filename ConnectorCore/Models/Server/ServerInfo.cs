﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorCore.Interfaces;

namespace ConnectorCore.Models.Server
{
    public class ServerInfo
    {
        public string Name { get; set; }
        public int Port { get; set; }
        public string HostOrIP { get; set; }
    }
}
