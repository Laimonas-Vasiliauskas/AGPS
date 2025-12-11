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
        public string partname { get; set; } = string.Empty;
        public string madeby { get; set; } = string.Empty;
        public string typeofwork { get; set; } = string.Empty;
        public string created_at { get; set; } = string.Empty;
        public string comments { get; set; } = string.Empty;
        public int remaining { get; set; } = 0;
        public int done { get; set; } = 0;
    }
}
