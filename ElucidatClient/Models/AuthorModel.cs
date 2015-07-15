using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElucidatClient.Models {
    /// <summary>
    /// Author details as retrived from the API
    /// </summary>
    public class AuthorModel {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime LastLogin { get; set; }
        public string Created { get; set; } // can be all zeroes, which .NET cannot convert to a datetime value
        public int Banned { get; set; }
    }
}
