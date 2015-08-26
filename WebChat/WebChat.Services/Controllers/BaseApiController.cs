using System.Web.Http;
using WebChat.Data;

namespace WebChat.Services.Controllers
{
    public class BaseApiController : ApiController
    {
        public BaseApiController()
            : this(new WebChatContext())
        {
        }

        public BaseApiController(WebChatContext data)
        {
            Data = data;
        }

        protected WebChatContext Data { get; set; }
    }
}