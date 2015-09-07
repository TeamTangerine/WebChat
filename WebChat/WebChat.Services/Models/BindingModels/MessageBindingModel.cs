namespace WebChat.Services.Models.BindingModels
{
    using System.ComponentModel.DataAnnotations;

    public class MessageBindingModel
    {
        [Required]
        public string Message { get; set; }
    }
}