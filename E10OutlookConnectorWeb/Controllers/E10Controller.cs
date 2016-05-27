using E10OutlookConnectorWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace E10OutlookConnectorWeb.Controllers
{
    public class E10Controller : ApiController
    {
        public ContactViewModel Get(string id)
        {
            // seed some random data
            // ordinarily the data would come from the E10 APIs

            var rnd = new Random();

            var contacts = new List<string>()
            {
                "Rodney Barrister",
                "Richard Hemmings",
                "Harry Carpenter",
                "Billy Harper",
                "Fred Jefferies",
                "James Burton",
                "Gary Butler"
            };

            var titles = new List<string>()
            {
                "Owner",
                "Developer",
                "Architect",
                "CEO",
                "Designer",
                "CIO"
            };

            var companies = new List<string>()
            {
                "Beneficial Technologies",
                "Billingsgate Hall",
                "Short Tech",
                "Dalton Industries",
                "Field Engineers"
            };

            var telephones = new List<string>()
            {
                "(874)555-2146",
                "(874)555-3745",
                "(874)555-2790",
                "(874)555-1736",
                "(874)555-4756",
                "(874)555-8747"
            };

            var cells = new List<string>()
            {
                "(874)555-5146",
                "(874)555-1715",
                "(874)555-0710",
                "(874)555-3739",
                "(874)555-0753",
                "(874)555-9748"
            };

            var salesByRegion = new List<ValueByRegion>()
            {
                new ValueByRegion() {Region="North", Value=rnd.Next(700,1477).ToString() },
                new ValueByRegion() {Region="South", Value=rnd.Next(100,1977).ToString() },
                new ValueByRegion() {Region="East", Value=rnd.Next(400,1097).ToString() },
                new ValueByRegion() {Region="West", Value=rnd.Next(800,1127).ToString() }
            };

            var contact = new ContactViewModel()
            {
                Company = companies[rnd.Next(companies.Count())],
                Contact = contacts[rnd.Next(contacts.Count())],
                Cell = cells[rnd.Next(cells.Count())],
                Telephone = telephones[rnd.Next(telephones.Count)],
                Email = id,
                Title = titles[rnd.Next(titles.Count())],
                OnHold = "Y",
                AR90Days = string.Format("${0}k", rnd.Next(300, 500)),
                OpenAR = string.Format("${0}k", rnd.Next(10, 876)),
                OpenOrders = string.Format("${0}k", rnd.Next(67, 187)),
                SalesYTD = string.Format("${0}M", rnd.Next(3, 98)),
                SalesByRegion = salesByRegion.ToArray()
            };

            return contact;
        }
    }
}
