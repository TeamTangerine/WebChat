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

            if (friendId == userId)
                return BadRequest("You canot have conversation with yourself!");

            if (!Data.Users.Any(u => u.Id == friendId))
            {
                return BadRequest("You cannot have conversation with an unexisting user!");
            }


            var messages = Data.Messages
                .Where(m => m.SenderId == friendId && m.ReceiverId == userId || m.SenderId == userId && m.ReceiverId == friendId)
                .OrderBy(m => m.SentOn)
                .Select(m => new MessageViewModel
                {
                    SenderId = m.SenderId,
                    SenderUserName = m.Sender.UserName,
                    RecieverId = m.ReceiverId,
                    RecieverUserName = m.Receiver.UserName,
                    ContentString = m.ContentString,
                    ContentType = m.ContentType,
                    SentOn = m.SentOn
                });

            if (messages.Any())
            {
                var notification = Data.Notifications
                    .First(n => n.SenderId == userId && n.ReceiverId == friendId);
                notification.UnseenMessages = 0;
                Data.SaveChanges();
            }
            
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

            if (!Data.Users.Any(u => u.Id == friendId))
            {
                return BadRequest("You cannot sent message to an unexisting user!");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newMessage = new Message
            {
                ContentString = message.ContentString,
                ContentType = "text",
                SentOn = DateTime.Now,
                SenderId = userId,
                ReceiverId = friendId
            };

            Data.Messages.Add(newMessage);

            if (!Data.Notifications.Any(n => n.SenderId == userId && n.ReceiverId == friendId))
            {
                Data.Notifications.Add(new Notification
                {
                    SenderId = userId,
                    ReceiverId = friendId,
                    UnseenMessages = 0
                });

                Data.SaveChanges();
            }

            var notification = Data.Notifications
                .First(n => n.SenderId == userId && n.ReceiverId == friendId);

            notification.UnseenMessages++;

            Data.SaveChanges();

            return Ok(newMessage);
        }

        //POST /api/messages/{friendId}/image
        [HttpPost]
        [Route("api/messages/{friendId}/image")]
        public IHttpActionResult PostImage(string friendId, Image img)
        {
            var userId = User.Identity.GetUserId();

            if (friendId == userId)
                return BadRequest("You canot send image to yourself!");

            if (!Data.Users.Any(u => u.Id == friendId))
            {
                return BadRequest("You can not sent image to an unexisting user!");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
                ContentString = imageBase64,
                ContentType = "image",
                SentOn = DateTime.Now,
                SenderId = userId,
                ReceiverId = friendId
            });


            if (!Data.Notifications.Any(n => n.SenderId == userId && n.ReceiverId == friendId))
            {
                Data.Notifications.Add(new Notification
                {
                    SenderId = userId,
                    ReceiverId = friendId,
                    UnseenMessages = 0
                });
            }

            var notification = Data.Notifications
                .First(n => n.SenderId == userId && n.ReceiverId == friendId);

            notification.UnseenMessages++;

            Data.SaveChanges();

            return Ok("Image sent successfully.");
        }
    }
}