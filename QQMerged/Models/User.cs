using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace QuiQue.Models
{
    public class User
    {
        [Key]
        [Required]
        public Int64 idUser { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        
        [Required]
        [DefaultValue(true)]
        public bool Confirm { get; set; }
        //public List<Event> Events { get; set; }
        //public List<Queue> Queues { get; set; }
    }
}
