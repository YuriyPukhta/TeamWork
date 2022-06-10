using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuiQue.Models.View 
{
    public class  QueueModel
    {
        public string User { get; set; }
        public Int64 idUser { get; set; }
        public Int64 EventId { get; set; }
        public DateTime Time_queue { get; set; }
        public Int64 Number { get; set; }
        public string Status { get; set; }
    }
}
