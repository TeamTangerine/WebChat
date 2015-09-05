using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FileUploads
{
    public class ValidatedMemoryStreamProvider : MultipartMemoryStreamProvider
    {
        //If the Extension of the uploaded file doesn’t match the user requirement, just simply nullify the stream.
        private static readonly string[] Extensions = { "txt", "log" }; //or other formats 'png', 'jpg', etc.

        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {

            var filename = headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            if (filename.IndexOf('.') < 0)
                return Stream.Null;

            var extension = filename.Split('.').Last();

            return Extensions.Any(i => i.Equals(extension, StringComparison.InvariantCultureIgnoreCase)) ? base.GetStream(parent, headers) : Stream.Null;
        }
    }
}