using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using WebChat.Services.Models.ViewModels;

namespace WebChat.Services.Controllers
{
    [Authorize]
    public class NotificationsController : BaseApiController
    { 
        //GET /api/notifications
        [HttpGet]
        [Route("api/notifications")]
        public IHttpActionResult GetAllNotifications()
        {
            var userId = User.Identity.GetUserId();

            if (!this.Data.Notifications.Any(n => n.ReceiverId == userId))
                return this.Ok("No new notifications.");

            var notifications = this.Data.Notifications
                .Where(n => n.ReceiverId == userId)
                .Select(n =>  new NotificationsViewModel
                {
                    SenderName = n.Sender.UserName,
                    NotificationsAmount = n.Amount
                });

            return this.Ok(notifications);
        }

        //GET /api/notifications/{friendId}
        [HttpGet]
        [Route("api/notifications/{userId}")]
        public IHttpActionResult GetNotificationsForUser(string friendId)
        {
            var userId = User.Identity.GetUserId();

            if (!this.Data.Notifications.Any(n => n.ReceiverId == userId))
                return this.Ok("No new notifications.");

            var notifications = this.Data.Notifications
                .Where(n => n.SenderId == friendId && n.ReceiverId == userId).
                Select(n => new NotificationsViewModel
                {
                    SenderName = n.Sender.UserName,
                    NotificationsAmount = n.Amount
                });

            return this.Ok(notifications);
        }
    }
}