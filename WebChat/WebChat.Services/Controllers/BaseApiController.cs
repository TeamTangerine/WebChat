using System.Web.Http;
using WebChat.Data;

namespace WebChat.Services.Controllers
{
    public class BaseApiController : ApiController
    {
        private WebChatContext data;

        public BaseApiController()
            : this(new WebChatContext())
        {
        }

        public BaseApiController(WebChatContext data)
        {
            this.data = data;
        }

        protected WebChatContext Data
        {
            get
            {
                return this.data;
            }
        }
    }
}