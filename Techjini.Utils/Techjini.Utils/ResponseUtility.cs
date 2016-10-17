using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Techjini.Utils
{
    public static class ResponseUtility
    {
        /// <summary>
        /// Attaches the stream in response.
        /// </summary>
        /// <param name="response">The response object.</param>
        /// <param name="stream">The stream to be attached.</param>
        /// <param name="attachmentName">Name of the attachment.</param>
        /// <returns>
        /// The <see cref="HttpResponseMessage" /> object containing the stream.
        /// </returns>
        public static HttpResponseMessage AttachStreamInResponse(HttpResponseMessage response, Stream stream, string attachmentName = "attachment")
        {
            StreamContent streamContent = new StreamContent(stream);

            // multipartData.Add(streamContent);
            response.Content = streamContent;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = attachmentName
            };

            return response;
        }

        /// <summary>
        /// Gets the response message containing a given file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>
        /// The <see cref="HttpResponseMessage" /> object containing the file
        /// </returns>
        public static HttpResponseMessage GetResponseWithFileContent(string path)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                if (File.Exists(path))
                {
                    FileStream stream = new FileStream(path, FileMode.Open);
                    response = AttachStreamInResponse(response, stream, Path.GetFileName(path).Trim('\"'));
                }
                else
                {
                    response.StatusCode = HttpStatusCode.PartialContent;
                    response.Content = new StringContent("File not found.");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return response;
        }

        /// <summary>
        /// Creates the response message from the given content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns>
        /// The <see cref="HttpResponseMessage" /> object containing the value of the content as JSON.
        /// </returns>
        public static HttpResponseMessage CreateResponseMessageFromContent(object content, HttpStatusCode statusCode)
        {
            HttpResponseMessage response = new HttpResponseMessage(statusCode);
            response.Content = new StringContent(JsonUtility.SerializeObjectToJson(content));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        }
    }
}
