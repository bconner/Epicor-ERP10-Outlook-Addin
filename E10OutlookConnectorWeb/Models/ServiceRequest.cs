using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E10OutlookConnectorWeb.Models
{
    public class ServiceRequest
    {
        public string token { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string context { get; set; }

    }
}
