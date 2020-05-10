using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSIS_Cource
{
    class TaskView
    {
        public string Source { get; set; }
        public string Dest { get; set; }
        public string Type { get; set; }
        public long Id { get; set; }
        public string Password { get; set; }
        public bool IsComplete { get; set; }
        public long MaxProcess { get; set; }
        public long CurrentProcess { get; set; }
        public TimeSpan Time { get; set; }
    }
}
