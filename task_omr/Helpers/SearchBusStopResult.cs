using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace task_omr.Helpers
{
    public class SearchBusStopResult
    {
        [Key]
        public int id { get; set; }
        public string BusStopName { get; set; }
        public string DepDateTime { get; set; }
        public string VoyageName { get; set; }
    }
}