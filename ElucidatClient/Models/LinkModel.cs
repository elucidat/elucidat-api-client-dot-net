using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElucidatClient.Models {
    /// <summary>
    /// Launch link data as retrived from the API
    /// </summary>
    public class LinkModel {
        public string Url { get; set; }
        public string LinkType { get; set; }
    }
}
