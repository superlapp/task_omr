using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace task_omr.Helpers
{
    public class SearchOptions
    {
        public string BusStopName { get; set; }
        public bool IsDepStation { get; set; }
        public bool IsArrStation { get; set; }
        public bool UseDT { get; set; }
        public string DT { get; set; }
    }
}