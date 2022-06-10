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
    public class EventModel
    { 
        public Int64 EventId { get; set; }
        
        public Int64 OwnerId { get; set; }
        
        public User Owner { get; set; }
       
        public string Title { get; set; }
        public bool isFastQueue { get; set; }
        
        public bool IsSuspended { get; set; }
        public string Description { get; set; }
        public string WaitingTimer { get; set; }
        //public List<Queue> Queues { get; set; }
    }
}
