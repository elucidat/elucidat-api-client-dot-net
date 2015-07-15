using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElucidatClient.Models {
    /// <summary>
    /// Account details as retrieved from the API.
    /// </summary>
    public class AccountModel {
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string SubscriptionLevel { get; set; }
        public DateTime Created { get; set; }
        public int CourseViewsThisMonth { get; set; }
        public string CourseViewsAllowedPerMonth { get; set; }
        public int TotalCourseViews { get; set; }
        public int ProjectPasses { get; set; }
        public int ProjectFails { get; set; }
        public int PageViewsThisMonth { get; set; }
        public int TotalPageViews { get; set; }
        public int ProjectsAllowedInAccount { get; set; }
        public int AuthorsAllowedInAccount { get; set; }
        public int AuthorsUsedInAccount { get; set; }
    }
}
