using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RequestData
    {
        public string ExchangeToken { get; set; }
        public string IdentityToken { get; set; }
        public string EWSUrl { get; set; }
        public string MailItemId { get; set; }
    }
}
