using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace ConnectorCore.Models
{
    public class RdpSettings : ICloneable
    {
        [XmlIgnore]
        [JsonIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int ColorDepth { get; set; }
        public bool SmartSizing { get; set; }
        public bool AutoFullScreen { get; set; }
        public bool DisableCtrlAltDel { get; set; }
        public bool RedirectDrives { get; set; }
        public bool RedirectPorts { get; set; }
        public bool RedirectPrinters { get; set; }
        public bool RedirectSmartCards { get; set; }
        public bool RedirectDirectX { get; set; }
        public bool EnableAutoReconnect { get; set; }

        public static RdpSettings GetDefault()
        {
            return new RdpSettings()
            {
                ColorDepth = 32,
                SmartSizing = true,
                AutoFullScreen = true,
                DisableCtrlAltDel = false,
                RedirectDrives = true,
                RedirectPorts = true,
                RedirectPrinters = true,
                RedirectSmartCards = true,
                RedirectDirectX = true,
                EnableAutoReconnect = true
            };
        }
        public object Clone()
        {
            return new RdpSettings()
            {
                ColorDepth=ColorDepth,
                SmartSizing=SmartSizing,
                AutoFullScreen=AutoFullScreen,
                DisableCtrlAltDel=DisableCtrlAltDel,
                RedirectDrives=RedirectDrives,
                RedirectPorts=RedirectPorts,
                RedirectPrinters=RedirectPrinters,
                RedirectSmartCards=RedirectSmartCards,
                RedirectDirectX=RedirectDirectX,
                EnableAutoReconnect=EnableAutoReconnect
            };
        }
    }
}
