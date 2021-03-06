﻿namespace WebChat.Services.Controllers
{
    using System.Linq;
    using System.Web.Http;

    using Microsoft.AspNet.Identity;

    using WebChat.Services.Models.ViewModels;

    [Authorize]
    public class NotificationsController : BaseApiController
    {
        // GET /api/notifications
        [HttpGet]
        [Route("api/notifications")]
        public IHttpActionResult GetAllNotifications()
        {
            var userId = this.User.Identity.GetUserId();

            if (!this.Data.Notifications.Any(n => n.ReceiverId == userId))
            {
                return this.Ok("No new notifications.");
            }

            var notifications = this.Data.Notifications
                .Where(n => n.ReceiverId == userId)
                .Select(n => new NotificationsViewModel
                {
                    SenderName = n.Sender.UserName,
                    UnseenMessages = n.UnseenMessages
                });

            return this.Ok(notifications);
        }

        // GET /api/notifications/{friendId}
        [HttpGet]
        [Route("api/notifications/{friendId}")]
        public IHttpActionResult GetNotificationsForUser(string friendId)
        {
            var userId = this.User.Identity.GetUserId();

            var notifications =
                this.Data.Notifications
                .Where(n => n.SenderId == friendId && n.ReceiverId == userId)
                .Select(n => new NotificationsViewModel
                {
                    SenderName = n.Sender.UserName, 
                    UnseenMessages = n.UnseenMessages
                });

            return this.Ok(notifications);
        }
    }
}