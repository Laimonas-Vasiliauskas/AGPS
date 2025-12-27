using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGPS.Models
{
    public class Project
    {
        public int id { get; set; }
        public string projectname { get; set; } = string.Empty;
    
    }

    public class Part
    {
        public int id { get; set; }

        public int project_id { get; set; }
        public string partname { get; set; } = string.Empty;
        public string madeby { get; set; } = string.Empty;
        public string typeofwork { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        public string comments { get; set; } = string.Empty;
        public int remaining { get; set; } = 0;
        public int done { get; set; } = 0;

    }
}
