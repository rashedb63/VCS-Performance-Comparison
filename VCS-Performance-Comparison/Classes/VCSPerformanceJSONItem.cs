using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Git_Repositories_Performance_Comparison.Classes
{
    internal class VCSPerformanceJSONItem
    {
        public VCSPerformanceJSONItem(long id, string name, long value)
        {
            this.id = id;
            this.name = name;
            this.value = value;
        }

        public long id { get; set; }
        public string name { get; set; }
        public long value { get; set; }
    }
}
