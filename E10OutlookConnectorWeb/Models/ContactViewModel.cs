using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E10OutlookConnectorWeb.Models
{
    /// <summary>
    /// This class is the mapping between the Office UI and the E10 API
    /// </summary>
    public class ContactViewModel
    {
        public string Company { get; set; }
        public string Contact { get; set; }
        public string Title { get; set; }
        public string Telephone { get; set; }
        public string Cell { get; set; }
        public string Email { get; set; }
        public string OnHold { get; set; }
        public string SalesYTD { get; set; }
        public string OpenOrders { get; set; }
        public string AR90Days { get; set; }
        public string OpenAR { get; set; }
        public ValueByRegion[] SalesByRegion { get; set; }
    }

    public class ValueByRegion
    {
        public string Value { get; set; }
        public string Region { get; set; }
    }
}
