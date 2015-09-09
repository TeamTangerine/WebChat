using System.Collections.Generic;

namespace WebChat.Services.Models
{
    // Models returned by AccountController actions.



    public class UserInfoViewModel
    {
        public string Email { get; set; }
        public bool HasRegistered { get; set; }
        public string LoginProvider { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }
}