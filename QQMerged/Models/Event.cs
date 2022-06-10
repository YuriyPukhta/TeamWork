using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuiQue.Models
{
    public class Event
    {
        [Key]
        [Required]
        public Int64 EventId { get; set; }
        [ForeignKey("User")]
        public Int64 OwnerId { get; set; }
        [JsonIgnore]
        public User Owner { get; set; }
        [Required]
        public string Title { get; set; }
        public bool isFastQueue { get; set; }
        [DefaultValue(false)]
        public bool IsSuspended { get; set; }
        public string Description { get; set; }
        public string WaitingTimer { get; set; }
        //public List<Queue> Queues { get; set; }
    }
}
