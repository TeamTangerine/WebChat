using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.SqlServer.Server;
using WebChat.Models;
using WebChat.Services.Models.BindingModels;

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
            if (!this.Data.Messages.Any(m => m.Sender.Id == friendId && m.Receiver.Id == userId))
                return this.Ok("No messages..");

            if (friendId == userId)
                return this.BadRequest("You canot have conversation with yourself!");

            var messages = Data.Messages
                .Where(m => m.Sender.Id == friendId && m.Receiver.Id == userId)
                .OrderBy(m => m.SentOn)
                .Select(m => new
                {
                    Sender = m.Sender.UserName,
                    Message = m.MessageString,
                    Receiver = m.Receiver.UserName
                });

            //TODO: Remove all notifications for the current converation.

            return Ok(messages);
        }

        //POST /api/messasges/{frienId}
        [HttpPost]
        [Route("api/messages/{friendId}")]
        public IHttpActionResult PostMessages(string friendId, [FromBody]MessageBindingModel message)
        {
            var userId = User.Identity.GetUserId();

            if (friendId == userId)
                return this.BadRequest("You canot send messages to yourself!");

            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            //This can be removed later on if we implement the validation in the front-end.
            if (message.Message.Length == 0)
                return this.BadRequest("Message cannot be empty!");


            this.Data.Messages.Add(new Message
            {
                MessageString = message.Message,
                SentOn = DateTime.Now,
                SenderId = userId,
                ReceiverId = friendId
            });

            //TODO: Add notification.


            this.Data.SaveChanges();

            return this.Ok("Message sent successfully.");
        }

        //POST /api/messages/{friendId}/image
        [HttpPost]
        [Route("api/messages/{friendId}/image")]
        public IHttpActionResult PostImage(string friendId, Image img)
        {
            var userId = User.Identity.GetUserId();

            if (friendId == userId)
                return this.BadRequest("You canot send images to yourself!");

            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            string imageBase64 = "";

           
            using (MemoryStream m = new MemoryStream())
            {
                img.Save(m, img.RawFormat);
                byte[] imageBytes = m.ToArray();

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

            //TODO: Add notification.


            this.Data.SaveChanges();

            return this.Ok("Image sent successfully.");

        }

    }
}