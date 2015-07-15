using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElucidatClient.Models {
    /// <summary>
    /// Account details as passed to the API for update
    /// </summary>
    public class AccountDetailModel {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyEmail { get; set; }
        public string Telephone { get; set; }
        public string Address1 { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
    }
}
