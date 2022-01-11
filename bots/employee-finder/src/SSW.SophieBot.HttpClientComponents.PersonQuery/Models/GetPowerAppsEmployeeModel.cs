using SSW.SophieBot.HttpClientAction.Models;
using System.Collections.Generic;
using System.Linq;

namespace SSW.SophieBot.HttpClientComponents.PersonQuery.Models
{
    public class GetPowerAppsEmployeeModel
    {
        public List<GetPowerAppsEmployeeModelItem> value { get; set; }

        public List<GetEmployeeModel> ToEmployeeModels()
        {
            return value.Select(item => new GetEmployeeModel
            {
                FirstName = item.ssw_firstname,
                LastName = item.ssw_lastname,
                EmailAddress = item.ssw_emailaddress
            }).ToList();
        }
    }

    public class GetPowerAppsEmployeeModelItem
    {
        public string ssw_lastname { get; set; }

        public string ssw_emailaddress { get; set; }

        public string ssw_firstname { get; set; }

        public string ssw_userid { get; set; }
    }
}
