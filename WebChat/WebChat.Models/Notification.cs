namespace WebChat.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UnseenMessages { get; set; }

        [Required]
        public string SenderId { get; set; }

        public virtual ApplicationUser Sender { get; set; }

        [Required]
        public string ReceiverId { get; set; }

        public virtual ApplicationUser Receiver { get; set; }
    }
}