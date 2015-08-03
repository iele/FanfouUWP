using System;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace FanfouUWP.FanfouAPI
{
    public static class XAuthHelper
    {
        public static Random RndSeed = new Random();
        public static DateTime UnixTimestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static byte[] CreateHMAC(byte[] data, byte[] key)
        {
            MacAlgorithmProvider crypt = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            IBuffer keyBuffer = CryptographicBuffer.CreateFromByteArray(key);
            CryptographicKey cryptKey = crypt.CreateKey(keyBuffer);

            IBuffer dataBuffer = CryptographicBuffer.CreateFromByteArray(data);
            IBuffer signBuffer = CryptographicEngine.Sign(cryptKey, dataBuffer);

            byte[] result;
            CryptographicBuffer.CopyToByteArray(signBuffer, out result);
            return result;
        }

        public static string GenerateSignature(string secret, string tokenSecret, string requestMethod,
            string requestUrl, Parameters parameters)
        {
            var data = new StringBuilder(1024);
            data.AppendFormat("{0}&{1}&", requestMethod.ToUpper(), UrlEncode(requestUrl));

            if (parameters != null)
            {
                parameters.Sort();
                data.Append(UrlEncode(parameters.BuildQueryString(true)));
            }

            string key = string.Format("{0}&{1}", UrlEncode(secret), UrlEncode(tokenSecret));
            byte[] hashBytes = CreateHMAC(Encoding.UTF8.GetBytes(data.ToString()), Encoding.UTF8.GetBytes(key));

            return UrlEncode(Convert.ToBase64String(hashBytes));
        }

        public static string UrlEncode(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            var buffer = new StringBuilder(text.Length);
            byte[] data = Encoding.UTF8.GetBytes(text);
            foreach (byte b in data)
            {
                var c = (char) b;
                if (!(('0' <= c && c <= '9') || ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z'))
                    && "-_.~".IndexOf(c) == -1)
                {
                    buffer.Append('%' + Convert.ToString(c, 16).ToUpper());
                }
                else
                {
                    buffer.Append(c);
                }
            }
            return buffer.ToString();
        }

        public static string GenerateRndNonce()
        {
            return string.Concat(
                RndSeed.Next(1, 99999999).ToString("00000000"),
                RndSeed.Next(1, 99999999).ToString("00000000"),
                RndSeed.Next(1, 99999999).ToString("00000000"),
                RndSeed.Next(1, 99999999).ToString("00000000"));
        }

        public static long GenerateTimestamp(DateTime time)
        {
            return (long) (time.ToUniversalTime() - UnixTimestamp).TotalSeconds;
        }
    }
}