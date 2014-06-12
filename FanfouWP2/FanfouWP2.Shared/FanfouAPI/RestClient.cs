using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace FanfouWP2.FanfouAPI
{
    public class RestClient
    {
        private string baseUrl;
        private string consumer;
        private string secret;
        public string token { get; private set; }
        public string tokenSecret { get; private set; }
        public RestClient(string baseUrl, string consumer = "", string secret = "", string token = "", string tokenSecret = "")
        {
            this.baseUrl = baseUrl;
            this.consumer = consumer;
            this.secret = secret;
            this.token = token;
            this.tokenSecret = tokenSecret;
        }

        private string generateXAuthHeader(string url, string username, string password)
        {
            Parameters oParameters = new Parameters();

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
            xauth += oParameters.Items[oParameters.Items.Count - 1].Key + "=\"" + oParameters.Items[oParameters.Items.Count - 1].Value + '"';
            return xauth;
        }
        private string generateOAuthHeader(Parameters parameters, string url)
        {
            Parameters oParameters = new Parameters();
            oParameters.Add("oauth_consumer_key", consumer);
            oParameters.Add("oauth_token", token);
            oParameters.Add("oauth_signature_method", "HMAC-SHA1");
            oParameters.Add("oauth_timestamp", XAuthHelper.GenerateTimestamp(DateTime.Now).ToString());
            oParameters.Add("oauth_nonce", XAuthHelper.GenerateRndNonce());
            oParameters.Add("oauth_version", "1.0");

            foreach (var p in parameters.Items)
                oParameters.Add(p.Key, p.Value);

            oParameters.Add("oauth_signature", XAuthHelper.GenerateSignature(secret, tokenSecret, "GET", baseUrl + "/" + url, oParameters));

            foreach (var p in parameters.Items)
                oParameters.Items.Remove(p);

            string oauth = "";
            for (int i = 0; i < oParameters.Items.Count - 1; i++)
            {
                oauth += oParameters.Items[i].Key + "=\"" + oParameters.Items[i].Value + '"' + ",";
            }
            oauth += oParameters.Items[oParameters.Items.Count - 1].Key + "=\"" + oParameters.Items[oParameters.Items.Count - 1].Value + '"';
            return oauth;
        }
        public async Task Login(string url, string username, string password)
        {
            using (var client = new HttpClient())
            {
                var oauth = generateXAuthHeader(url, username, password);
                client.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", oauth);

                using (var response = await client.GetAsync(new Uri(baseUrl + "/" + url)))
                {
                    var result = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    string[] content = result.Split(new char[] { '=', '&' });
                    this.token = content[1];
                    this.tokenSecret = content[3];
                }
            }
        }

        public async Task<string> GetRequest(string url, Parameters parameters = null)
        {
            using (var client = new HttpClient())
            {
                var urlStr = baseUrl + "/" + url;

                var str = "?";
                if (parameters != null)
                {
                    foreach (var i in parameters.Items)
                    {
                        str += WebUtility.UrlEncode(i.Key) + "=" + WebUtility.UrlEncode(i.Value) + "&";
                    }
                }
                else {
                    parameters = new Parameters();
                }

                var oauth = generateOAuthHeader(parameters, url);
                client.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", oauth);

                str = str.Substring(0, str.Length - 1);
                using (var response = await client.GetAsync(new Uri(urlStr + str)))
                {
                    var result = await response.Content.ReadAsStringAsync();
                    response.EnsureSuccessStatusCode();
                    return result;
                }
            }
        }
        public async Task<T> GetRequestObject<T>(string url, Parameters parameters = null) where T : Item
        {
            var ds = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(await GetRequest(url, parameters))))
            {
                var obj = ds.ReadObject(ms) as T;
                return obj;
            }
        }

        public async Task<List<T>> GetRequestObjectCollection<T>(string url, Parameters parameters = null)
            where T : Item
        {
            var ds = new DataContractJsonSerializer(typeof(List<T>));
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(await GetRequest(url, parameters))))
            {
                var obj = ds.ReadObject(ms) as List<T>;
                return obj;
            }
        }

        public async Task<string> PostRequest(string url, Parameters parameters)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Method", "POST");
                var content = new HttpStringContent("", Windows.Storage.Streams.UnicodeEncoding.Utf8);
                using (var response = await client.PostAsync(new Uri(baseUrl + "/" + url), content))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        public async Task<string> PostRequestWithFile(string url, Parameters parameters, Stream file)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Method", "POST");
                Windows.Storage.Streams.Buffer buff = new Windows.Storage.Streams.Buffer(1024);
                var content = new HttpBufferContent(buff);
                using (var response = await client.PostAsync(new Uri(baseUrl + "/" + url), content))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
