using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElucidatClient.Models {
    public class PollResultsModel {
        public string ReleaseCode { get; set; }
        public PollResultsAnswers Answers { get; set; }
    }

    public class PollResultsAnswers {
        public IDictionary<string, IDictionary<string, int>> Summary { get; set; }
        public IDictionary<string, IDictionary<string, IDictionary<string, int>>> ReleaseVersions { get; set; } 
    }
}
