using System;

namespace WebChat.Services.Models.ViewModels
{
    public class MessageViewModel
    {
        public string SenderId { get; set; }

        public string ContentString { get; set; }

        public string ContentType { get; set; }

        public DateTime SentOn { get; set; }
    }
}