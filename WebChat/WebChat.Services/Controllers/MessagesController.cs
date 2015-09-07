using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using WebChat.Models;
using WebChat.Services.Models.BindingModels;
using WebChat.Services.Models.ViewModels;

namespace WebChat.Services.Controllers
{
    [Authorize]
    [Route("api/Messages")]
    public class MessagesController : BaseApiController
    {
        //GET /api/messages/{friendId}
        [HttpGet]
        [Route("api/messages/{friendId}")]
        public IHttpActionResult GetMessages(string friendId)
        {
            var userId = User.Identity.GetUserId();

            //It returns "ok" because it's not an actual error.
            if (!Data.Messages.Any(m => m.Sender.Id == friendId && m.Receiver.Id == userId))
                return Ok("No messages..");

            if (friendId == userId)
                return BadRequest("You canot have conversation with yourself!");

            var messages = Data.Messages
                .Where(m => m.Sender.Id == friendId && m.Receiver.Id == userId)
                .OrderBy(m => m.SentOn)
                .Select(m => new MessageViewModel
                {
                    SenderId = m.SenderId,
                    Message = m.MessageString,
                    SentOn = m.SentOn
                });

            var notification = Data.Notifications
                .First(n => n.SenderId == userId && n.ReceiverId == friendId);

            notification.Amount = 0;

            Data.SaveChanges();

            return Ok(messages);
        }

        //POST /api/messasges/{frienId}
        [HttpPost]
        [Route("api/messages/{friendId}")]
        public IHttpActionResult PostMessages(string friendId, [FromBody] MessageBindingModel message)
        {
            var userId = User.Identity.GetUserId();

            if (friendId == userId)
                return BadRequest("You canot send messages to yourself!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //This can be removed later on if we implement the validation in the front-end.
            if (message.Message.Length == 0)
                return BadRequest("Message cannot be empty!");

            if (!Data.Notifications.Any(n => n.SenderId == userId && n.ReceiverId == friendId))
            {
                Data.Notifications.Add(new Notification
                {
                    SenderId = userId,
                    ReceiverId = friendId,
                    Amount = 0
                });
            }

            Data.Messages.Add(new Message
            {
                MessageString = message.Message,
                SentOn = DateTime.Now,
                SenderId = userId,
                ReceiverId = friendId
            });

            var notification = Data.Notifications
                .First(n => n.SenderId == userId && n.ReceiverId == friendId);

            notification.Amount++;

            Data.SaveChanges();

            return Ok("Message sent successfully.");
        }

        //POST /api/messages/{friendId}/image
        [HttpPost]
        [Route("api/messages/{friendId}/image")]
        public IHttpActionResult PostImage(string friendId, Image img)
        {
            var userId = User.Identity.GetUserId();

            if (friendId == userId)
                return BadRequest("You canot send images to yourself!");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!Data.Notifications.Any(n => n.SenderId == userId && n.ReceiverId == friendId))
            {
                Data.Notifications.Add(new Notification
                {
                    SenderId = userId,
                    ReceiverId = friendId,
                    Amount = 0
                });
            }

            var imageBase64 = "";

            using (var m = new MemoryStream())
            {
                img.Save(m, img.RawFormat);
                var imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                imageBase64 = Convert.ToBase64String(imageBytes);
            }

            Data.Messages.Add(new Message
            {
                MessageString = imageBase64,
                SentOn = DateTime.Now,
                SenderId = userId,
                ReceiverId = friendId
            });

            var notification = Data.Notifications
                .First(n => n.SenderId == userId && n.ReceiverId == friendId);

            notification.Amount++;

            Data.SaveChanges();

            return Ok("Image sent successfully.");
        }
    }
}