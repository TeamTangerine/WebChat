using System;

namespace WebChat.Services.Models.ViewModels
{
    public class MessageViewModel
    {
        public string SenderId { get; set; }

        public string Message { get; set; }

        public DateTime SentOn { get; set; }
    }
}