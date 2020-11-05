using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Final.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; }
    }

    public enum TicketStatus
    {
        Pending,
        Completed,
        Closed
    }
}
