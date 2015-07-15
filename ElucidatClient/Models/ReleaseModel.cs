using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElucidatClient.Models {
    /// <summary>
    /// Release details as retrieved from the API
    /// </summary>
    public class ReleaseModel {
        public string ReleaseCode { get; set; }
        public DateTime Created { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public int ReleaseViews { get; set; }
        public int ReleaseViewsThisMonth { get; set; }
        public int ReleasePasses { get; set; }
        public int ReleaseFails { get; set; }
        public DateTime? ReleaseLastView { get; set; }
        public string VersionNumber { get; set; }
        public string ReleaseMode { get; set; }
        public string EditUrl { get; set; }
        public ProjectModel Project { get; set; } // will be null when listing releases for a given project
    }
}
