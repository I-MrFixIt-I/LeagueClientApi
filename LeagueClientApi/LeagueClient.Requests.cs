using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace LeagueClientApi
{
    public partial class LeagueClient
    {
        /// <summary>
        /// Creates a basic HttpWebRequest with the specified uri and method.
        /// </summary>
        /// <param name="path">Path to the requested resource</param>
        /// <param name="method">HttpMethod to be used by this request</param>
        /// <returns>Returns a basic HttpWebRequest</returns>
        private HttpWebRequest CreateRequest(string path, string method)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("https://127.0.0.1:{0}/{1}", leagueClientPort, path));

            request.Method = method;

            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.8");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36";

            // Add the Auth-Header which is needed for us to make successful requests
            request.Headers.Add("Authorization", GetAuthHeaderValue());

            // We need to accept all certificates, even if we do not know them, otherwise we can not make a successful request
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return request;
        }

        /// <summary>
        /// Submits the specified request
        /// </summary>
        /// <param name="request">The request to be sent</param>
        /// <param name="content">The variable where the response should be saved</param>
        /// <returns>Returns the response sent by the request</returns>
        private HttpWebResponse SubmitRequest(HttpWebRequest request, out string content)
        {
            return SubmitRequest(request, string.Empty,out content);
        }

        /// <summary>
        /// Submits the specified request
        /// </summary>
        /// <param name="request">The request to be sent</param>
        /// <param name="postData">The data that should be sent with this request</param>
        /// <param name="content">The variable where the response should be saved</param>
        /// <returns>Returns the response sent by the request</returns>
        private HttpWebResponse SubmitRequest(HttpWebRequest request, string postData, out string content)
        {
            HttpWebResponse response;

            if (request.Method != "GET")
            {
                request.ContentType = "application/json";

                // add post data to request if specified
                if (string.IsNullOrWhiteSpace(postData) == false)
                {
                    byte[] postBytes = new UTF8Encoding().GetBytes(postData);
                    request.ContentLength = postBytes.Length;

                    Stream postStream = request.GetRequestStream();
                    postStream.Write(postBytes, 0, postBytes.Length);
                    postStream.Flush();
                    postStream.Close();
                }
            }

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                try
                {
                    Stream stream = response.GetResponseStream();

                    if (response.ContentEncoding == "gzip")
                    {
                        stream = new GZipStream(stream, CompressionMode.Decompress);
                    }

                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    content = reader.ReadToEnd();

                    return response;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                {
                    content = string.Empty;
                }
                else
                {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        content = reader.ReadToEnd();
                    }
                }

                return (HttpWebResponse)ex.Response;
            }
        }
    }
}
