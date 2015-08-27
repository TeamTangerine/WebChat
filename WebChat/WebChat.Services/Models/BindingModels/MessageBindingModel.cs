using System;
using System.Security.AccessControl;

namespace WebChat.Services.Models.BindingModels
{
    public class MessageBindingModel
    {
        public string Message { get; set; }

        public DateTime SentOn { get; set; }
    }
}