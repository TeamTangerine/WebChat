using System;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace WebChat.Services.Models.BindingModels
{
    public class MessageBindingModel
    {
        [Required]
        [MinLength(1)]
        public string ContentString { get; set; }
    }
}