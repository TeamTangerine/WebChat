namespace WebChat.Services.Controllers
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Web.Http;
    using Microsoft.AspNet.Identity;
    using WebChat.Models;
    using WebChat.Services.Models.BindingModels;
    using WebChat.Services.Models.ViewModels;

    [Authorize]
    [Route("api/Messages")]
    public class MessagesController : BaseApiController
    {
        // GET /api/messages/{friendId}
        [HttpGet]
        [Route("api/messages/{friendId}")]
        public IHttpActionResult GetMessages(string friendId)
        {
            var userId = this.User.Identity.GetUserId();

            // It returns "ok" because it's not an actual error.
            if (!this.Data.Messages.Any(m => m.Sender.Id == friendId && m.Receiver.Id == userId))
            {
                return this.Ok("No messages..");
            }

            if (friendId == userId)
            {
                return this.BadRequest("You canot have conversation with yourself!");
            }

            var messages = Data.Messages
                .Where(m => m.Sender.Id == friendId && m.Receiver.Id == userId)
                .OrderBy(m => m.SentOn)
                .Select(m => new MessageViewModel
                {
                    SenderId = m.SenderId,
                    Message = m.MessageString,
                    SentOn = m.SentOn
                });

            var notification = this.Data.Notifications
                .First(n => n.SenderId == userId && n.ReceiverId == friendId);

            notification.Amount = 0;

            this.Data.SaveChanges();

            return this.Ok(messages);
        }

        // POST /api/messasges/{frienId}
        [HttpPost]
        [Route("api/messages/{friendId}")]
        public IHttpActionResult PostMessages(string friendId, [FromBody] MessageBindingModel message)
        {
            var userId = this.User.Identity.GetUserId();

            if (friendId == userId)
            {
                return this.BadRequest("You canot send messages to yourself!");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            // This can be removed later on if we implement the validation in the front-end.
            if (message.Message.Length == 0)
            {
                return this.BadRequest("Message cannot be empty!");
            }

            if (!this.Data.Notifications.Any(n => n.SenderId == userId && n.ReceiverId == friendId))
            {
                this.Data.Notifications.Add(new Notification
                {
                    SenderId = userId,
                    ReceiverId = friendId,
                    Amount = 0
                });
            }

            this.Data.Messages.Add(new Message
            {
                MessageString = message.Message,
                SentOn = DateTime.Now,
                SenderId = userId,
                ReceiverId = friendId
            });

            var notification = this.Data.Notifications
                .First(n => n.SenderId == userId && n.ReceiverId == friendId);

            notification.Amount++;

            this.Data.SaveChanges();

            return this.Ok("Message sent successfully.");
        }

        // POST /api/messages/{friendId}/image
        [HttpPost]
        [Route("api/messages/{friendId}/image")]
        public IHttpActionResult PostImage(string friendId, Image img)
        {
            var userId = this.User.Identity.GetUserId();

            if (friendId == userId)
            {
                return this.BadRequest("You canot send images to yourself!");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (!this.Data.Notifications.Any(n => n.SenderId == userId && n.ReceiverId == friendId))
            {
                this.Data.Notifications.Add(new Notification
                {
                    SenderId = userId,
                    ReceiverId = friendId,
                    Amount = 0
                });
            }

            var imageBase64 = string.Empty;

            using (var m = new MemoryStream())
            {
                img.Save(m, img.RawFormat);
                var imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                imageBase64 = Convert.ToBase64String(imageBytes);
            }

            this.Data.Messages.Add(new Message
            {
                MessageString = imageBase64,
                SentOn = DateTime.Now,
                SenderId = userId,
                ReceiverId = friendId
            });

            var notification = this.Data.Notifications
                .First(n => n.SenderId == userId && n.ReceiverId == friendId);

            notification.Amount++;

            this.Data.SaveChanges();

            return this.Ok("Image sent successfully.");
        }
    }
}