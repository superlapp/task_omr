using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace task_omr.Helpers
{
    public class OrderInfo
    {
        [Key]
        public int id { get; set; }
        public int OrderId { get; set; }
        public int VoyageId { get; set; }
        public string VoyageName { get; set; }
        public DateTime DepDT { get; set; }
        public DateTime ArrDT { get; set; }
        public int SeatNumber { get; set; }
        public string Price { get; set; }
        public string Status { get; set; }
    }
}