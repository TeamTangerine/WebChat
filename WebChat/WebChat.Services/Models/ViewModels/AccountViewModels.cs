using System.Collections.Generic;
using WebChat.Models;

namespace WebChat.Services.Models.ViewModels
{
    // Models returned by AccountController actions.

    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }

        public static UserViewModel GetUserViewModel(ApplicationUser user)
        {
            return new UserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }
    }
    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }
}