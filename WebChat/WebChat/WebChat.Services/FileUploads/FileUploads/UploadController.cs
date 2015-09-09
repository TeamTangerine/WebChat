using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;

namespace FileUploads
{
    public class UploadController : ApiController
    {
        [Route("upload")] //uploads to disc
        //Physically creates a file on the server and then exposes the stream representing the file for the developer to process further. Additionally,
        //MultipartFormDataStreamProvider will look for the filename in a Content-Disposition header. If this header is not
        //present, it will not write to the disk directly (will not use FileStream), but will instead expose the uploaded file as a
        //MemoryStream only. MultipartMemoryStreamProvider will always load the contents of the uploaded file into a MemoryStream.

        public async Task Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted"));

            var streamProvider = new CustomMultipartFormDataStreamProvider(Path.Combine(Assembly.GetExecutingAssembly().Location, @"..\..\..\uploads"));
            await Request.Content.ReadAsMultipartAsync(streamProvider);
        }

        [Route("uploadToMemory")]// uploads directly to the memory e.g. cloud

        //The code to accept a file upload directly into memory appart from the validation is very similar, except the fact that once the file is
        //loaded into a MemoryStream, it’s up to the developer to handle it further. For example, you may wish to save the
        //Stream to a database or upload it to the cloud.

        public async Task<List<string>> PostToMemory()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted"));

            var provider = new ValidatedMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            var contents = new List<string>();

            foreach (HttpContent ctnt in provider.Contents)
            {
                var stream = await ctnt.ReadAsStreamAsync();

                if (stream.Length != 0)
                {
                    var sr = new StreamReader(stream);
                    contents.Add(sr.ReadToEnd());
                }
            }

            return contents;
        }
    }
}