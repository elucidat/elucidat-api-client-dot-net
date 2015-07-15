using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace ElucidatClient.Support {
    class UnderscoreContractResolver : DefaultContractResolver {
        /// <summary>
        /// Map PHP style property names (e.g. project_code) in JSON to .NET style property names (e.g. ProjectCode) in class definitions
        /// and vice-versa.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override string ResolvePropertyName(string propertyName) {
            return propertyName.TransformPropertyName();
        }
    }
}
