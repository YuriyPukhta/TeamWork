using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace QuiQue.Models
{
    public class UserModel
    {
        [Required]
        public string Username { get; set; }
        public string PhoneNumber { get; set; }

        //public List<Event> Events { get; set; }
        //public List<Queue> Queues { get; set; }
    }
}
