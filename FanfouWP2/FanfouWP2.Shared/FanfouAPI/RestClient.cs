using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using FanfouWP2.FanfouAPI.Items;
using Buffer = Windows.Storage.Streams.Buffer;

namespace FanfouWP2.FanfouAPI
{
    public class RestClient
    {
        private readonly string baseUrl;
        private readonly string consumer;
        private readonly HttpBaseProtocolFilter protocolFilter = new HttpBaseProtocolFilter();
        private readonly string secret;

        public RestClient(string baseUrl, string consumer = "", string secret = "", string token = "",
            string tokenSecret = "")
        {
            this.baseUrl = baseUrl;
            this.consumer = consumer;
            this.secret = secret;
            this.token = token;
            this.tokenSecret = tokenSecret;

            protocolFilter.AllowUI = false;
        }

        public string token { get; private set; }
        public string tokenSecret { get; private set; }

        private string generateXAuthHeader(string url, string username, string password)
        {
            var oParameters = new Parameters();

            oParameters.Add("x_auth_username", username);
            oParameters.Add("x_auth_password", password);
            oParameters.Add("x_auth_mode", "client_auth");

            oParameters.Add("oauth_consumer_key", consumer);
            oParameters.Add("oauth_signature_method", "HMAC-SHA1");
            oParameters.Add("oauth_timestamp", XAuthHelper.GenerateTimestamp(DateTime.Now).ToString());
            oParameters.Add("oauth_nonce", XAuthHelper.GenerateRndNonce());
            oParameters.Add("oauth_version", "1.0");

            oParameters.Add("oauth_signature", XAuthHelper.GenerateSignature(secret, "", "GET", url, oParameters));

            string xauth = "";
            for (int i = 0; i < oParameters.Items.Count - 1; i++)
            {
                xauth += oParameters.Items[i].Key + "=\"" + oParameters.Items[i].Value + '"' + ",";
            }
            xauth += oParameters.Items[oParameters.Items.Count - 1].Key + "=\"" +
                     oParameters.Items[oParameters.Items.Count - 1].Value + '"';
            return xauth;
        }

        private string generateOAuthHeader(Parameters parameters, string url, string method)
        {
            var oParameters = new Parameters();
            oParameters.Add("oauth_consumer_key", consumer);
            oParameters.Add("oauth_token", token);
            oParameters.Add("oauth_signature_method", "HMAC-SHA1");
            oParameters.Add("oauth_timestamp", XAuthHelper.GenerateTimestamp(DateTime.Now).ToString());
            oParameters.Add("oauth_nonce", XAuthHelper.GenerateRndNonce());
            oParameters.Add("oauth_version", "1.0");

            foreach (var p in parameters.Items)
                oParameters.Add(p.Key, p.Value);

            oParameters.Add("oauth_signature",
                XAuthHelper.GenerateSignature(secret, tokenSecret, method, baseUrl + "/" + url, oParameters));

            foreach (var p in parameters.Items)
                oParameters.Items.Remove(p);

            string oauth = "";
            for (int i = 0; i < oParameters.Items.Count - 1; i++)
            {
                oauth += oParameters.Items[i].Key + "=\"" + oParameters.Items[i].Value + '"' + ",";
            }
            oauth += oParameters.Items[oParameters.Items.Count - 1].Key + "=\"" +
                     oParameters.Items[oParameters.Items.Count - 1].Value + '"';
            return oauth;
        }

        public async Task Login(string url, string username, string password)
        {
            using (var client = new HttpClient(protocolFilter))
            {
                string oauth = generateXAuthHeader(url, username, password);
                client.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", oauth);

                using (HttpResponseMessage response = await client.GetAsync(new Uri(baseUrl + "/" + url)))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    string[] content = result.Split(new[] {'=', '&'});
                    token = content[1];
                    tokenSecret = content[3];
                }
            }
        }

        public async Task<string> GetRequest(string url, Parameters parameters = null)
        {
            using (var client = new HttpClient(protocolFilter))
            {
                string urlStr = baseUrl + "/" + url;

                string str = "?";
                if (parameters != null)
                {
                    foreach (var i in parameters.Items)
                    {
                        str += WebUtility.UrlEncode(i.Key) + "=" + WebUtility.UrlEncode(i.Value) + "&";
                    }
                }
                else
                {
                    parameters = new Parameters();
                }

                string oauth = generateOAuthHeader(parameters, url, "GET");
                client.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", oauth);

                str = str.Substring(0, str.Length - 1);
                using (HttpResponseMessage response = await client.GetAsync(new Uri(urlStr + str)))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    return result;
                }
            }
        }

        public async Task<T> GetRequestObject<T>(string url, Parameters parameters = null) where T : Item
        {
            var ds = new DataContractJsonSerializer(typeof (T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(await GetRequest(url, parameters))))
            {
                var obj = ds.ReadObject(ms) as T;
                return obj;
            }
        }

        public async Task<List<T>> GetRequestObjectCollection<T>(string url, Parameters parameters = null)
            where T : Item
        {
            var ds = new DataContractJsonSerializer(typeof (List<T>));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(await GetRequest(url, parameters))))
            {
                var obj = ds.ReadObject(ms) as List<T>;
                return obj;
            }
        }

        public async Task<string> PostRequest(string url, Parameters parameters)
        {
            using (var client = new HttpClient(protocolFilter))
            {
                string urlStr = baseUrl + "/" + url;

                string oauth = generateOAuthHeader(parameters, url, "POST");
                client.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", oauth);

                var content = new HttpFormUrlEncodedContent(parameters.Items);
                using (HttpResponseMessage response = await client.PostAsync(new Uri(urlStr), content))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    return result;
                }
            }
        }

        public async Task<T> PostRequestObject<T>(string url, Parameters parameters = null) where T : Item
        {
            var ds = new DataContractJsonSerializer(typeof (T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(await PostRequest(url, parameters))))
            {
                var obj = ds.ReadObject(ms) as T;
                return obj;
            }
        }

        public async Task<List<T>> PostRequestObjectCollection<T>(string url, Parameters parameters = null)
            where T : Item
        {
            var ds = new DataContractJsonSerializer(typeof (List<T>));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(await PostRequest(url, parameters))))
            {
                var obj = ds.ReadObject(ms) as List<T>;
                return obj;
            }
        }

        public async Task<T> PostRequestWithFile<T>(string url, Parameters parameters, string filePara, StorageFile file)
            where T : Item
        {
            using (var client = new HttpClient(protocolFilter))
            {
                string urlStr = baseUrl + "/" + url;

                string oauth = generateOAuthHeader(new Parameters(), url, "POST");
                client.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", oauth);
                var buff = new Buffer(1024);
                var content = new HttpMultipartFormDataContent();
                foreach (var item in parameters.Items)
                {
                    var c = new HttpStringContent(item.Value);
                    content.Add(c, item.Key);
                }
                Stream s = await file.OpenStreamForReadAsync();
                var f = new HttpStreamContent(s.AsInputStream());
                f.Headers.ContentType = new HttpMediaTypeHeaderValue(file.ContentType);
                content.Add(f, filePara, file.Name);
                using (HttpResponseMessage response = await client.PostAsync(new Uri(urlStr), content))
                {
                    string result = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    var ds = new DataContractJsonSerializer(typeof (T));
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(result)))
                    {
                        var obj = ds.ReadObject(ms) as T;
                        return obj;
                    }
                }
            }
        }
    }
}