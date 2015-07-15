using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElucidatClient.Models {
    /// <summary>
    /// Project details as retrieved from the API
    /// </summary>
    public class ProjectModel {
        public string ProjectCode { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public int ProjectViews { get; set; }
        public int ProjectViewsThisMonth { get; set; }
        public int ProjectPasses { get; set; }
        public int ProjectFails { get; set; }
        public string Theme { get; set; }
        public string ThemeVersionNumber { get; set; }
        public string EditUrl { get; set; }
    }
}
