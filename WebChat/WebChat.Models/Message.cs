using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebChat.Models
{
    public class Message
    {
        public Message()
        {
            Notifications = new HashSet<Notification>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string MessageString { get; set; }

        [Required]
        public string SenderId { get; set; }

        public virtual ApplicationUser Sender { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        public virtual ApplicationUser Receiver { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}