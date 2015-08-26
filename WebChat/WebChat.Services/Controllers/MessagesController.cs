using System.Linq;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace WebChat.Services.Controllers
{
    [Authorize]
    [Route("api/Messages")]
    public class MessagesController : BaseApiController
    {
        //GET /api/messages/{SenderId}
        [Route("api/messages/{SenderId}")]
        public IHttpActionResult GetMessages(string senderId)
        {
            var userId = User.Identity.GetUserId();

            var messages = Data.Messages
                .Where(m => m.Sender.Id == senderId && m.Receiver.Id == userId)
                .Select(m => new
                {
                    Sender = m.Sender.UserName,
                    Message = m.MessageString,
                    Receiver = m.Receiver.UserName
                });

            return Ok(messages);
        }
    }
}