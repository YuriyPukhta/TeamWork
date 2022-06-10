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
    public class Queue
    {
        //public List<Event> Event_idEvent { get; set; }
        //public List<User> User_idUser { get; set; }
        [Key]
        public Int64 ID { get; set; }
        [ForeignKey("User")]
        public Int64 idUser { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        [ForeignKey("Event")]
        public Int64 EventId { get; set; }
        [JsonIgnore]
        public Event Event { get; set; }
        [Required]
        public Int64 Number { get; set; }
        [Required]
        public DateTime Time_queue { get; set; }
        [Required]
        [DefaultValue("in queue")]
        public string Status { get; set; }
        [Required]
        [DefaultValue("Renamed_user")]
        public string Username { get; set; }
    }
}
