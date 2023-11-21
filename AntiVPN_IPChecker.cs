using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AntiVPN_TShock.VpnChecker
{
    public static class AntiVPN_IPChecker
    {
        public static Func<string, string, Task<bool>> IpHubChecker = async ( ip, key) =>
        {

            string getVPN;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://v2.api.iphub.info/ip/" + ip);

            requestMessage.Headers.Remove("X-Key");
            requestMessage.Headers.Add("X-Key", key);

            var httpResponse = await AntiVPN_TShock.client.SendAsync(requestMessage);
            using (var streamReader = new StreamReader(httpResponse.Content.ReadAsStream()))
            {
                var result = streamReader.ReadToEnd();
                AntiVPN_TShock.LastApiResult = result;
                dynamic data = JObject.Parse(result);
                getVPN = (string)data.SelectToken("block");
            }

            var statusCode = httpResponse.StatusCode;

            if (statusCode.ToString() == "OK"
                && getVPN == "1")
            {
                return true;

            }
            else
            {
                return false;
            }

        };

        public static Func<string, string, Task<bool>> ProxyCheckChecker = async (ip, key) =>
        {

            string getVPN;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://proxycheck.io/v2/{ip}?vpn=1&key={key}");

            var httpResponse = await AntiVPN_TShock.client.SendAsync(requestMessage);

            using (var streamReader = new StreamReader(httpResponse.Content.ReadAsStream()))
            {
                var result = streamReader.ReadToEnd();
                AntiVPN_TShock.LastApiResult = result;
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                var d1 = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
                var d = d1[ip];
                getVPN = d["proxy"];
            }

            if (getVPN == "yes")
                return true;

            else
                return false;

        };

        public static Func<string, string, Task<bool>> GetIpIntelChecker = async (ip, contact) =>
        {
            float getVPN;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://check.getipintel.net/check.php?ip={ip}&contact={contact}&format=json");

            var httpResponse = await AntiVPN_TShock.client.SendAsync(requestMessage);
            using (var streamReader = new StreamReader(httpResponse.Content.ReadAsStream()))
            {
                var result = streamReader.ReadToEnd();
                AntiVPN_TShock.LastApiResult = result;
                dynamic data = JObject.Parse(result);
                getVPN = (float)data.result;
            }

            if (getVPN >= 0.98)
                return true;

            else
                return false;

        };

        public static Func<string, string, Task<bool>> IpTrooperChecker = async (ip, key) =>
        {
            string getVPN;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://api.iptrooper.net/check/{ip}?key={key}&full=1");

            var httpResponse = await AntiVPN_TShock.client.SendAsync(requestMessage);
            using (var streamReader = new StreamReader(httpResponse.Content.ReadAsStream()))
            {
                var result = streamReader.ReadToEnd();
                AntiVPN_TShock.LastApiResult = result;
                dynamic data = JObject.Parse(result);
                getVPN = data.type;
            }

            if (String.Equals(getVPN, "proxy", StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
                return false;
        };

        public static Func<string, string, Task<bool>> IpQualityScoreChecker = async (ip, key) =>
        {
            bool getVPN;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://ipqualityscore.com/api/json/ip/{key}/{ip}?strictness=1&allow_public_access_points=false&fast=false");

            var httpResponse = await AntiVPN_TShock.client.SendAsync(requestMessage);
            using (var streamReader = new StreamReader(httpResponse.Content.ReadAsStream()))
            {
                var result = streamReader.ReadToEnd();
                AntiVPN_TShock.LastApiResult = result;
                dynamic data = JObject.Parse(result);
                getVPN = data.proxy;
            }

            if (getVPN)
                return true;

            else
                return false;
        };

        public static Func<string, string, Task<bool>> IpHunterChecker = async (ip, key) =>
        {
            int getVPN;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://www.iphunter.info:8082/v1/ip/" + ip);

            requestMessage.Headers.Remove("X-Key");
            requestMessage.Headers.Add("X-Key", key);

            var httpResponse = await AntiVPN_TShock.client.SendAsync(requestMessage);
            using (var streamReader = new StreamReader(httpResponse.Content.ReadAsStream()))
            {
                var result = streamReader.ReadToEnd();
                AntiVPN_TShock.LastApiResult = result;
                dynamic data = JObject.Parse(result);
                getVPN = data.data.block;
            }

            if (getVPN == 1)
                return false;

            else
                return false;

           
        };

        public static Func<string, string, Task<bool>> VpnBlockerChecker = async (ip, key) =>
        {
            bool getVPN;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api.vpnblocker.net/v2/json/" + ip);

            var httpResponse = await AntiVPN_TShock.client.SendAsync(requestMessage);
            using (var streamReader = new StreamReader(httpResponse.Content.ReadAsStream()))
            {
                var result = streamReader.ReadToEnd();
                AntiVPN_TShock.LastApiResult = result;
                dynamic data = JObject.Parse(result);
                getVPN = data["host-ip"];
            }

            if (getVPN)
                return true;

            else
                return false;

        };

        public static Func<string, string, Task<bool>> Ip2LocationChecker = async (ip, key) =>
        {
            bool getVPN;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api.ip2location.io/?key={key}&ip=" + ip);

            var httpResponse = await AntiVPN_TShock.client.SendAsync(requestMessage);
            using (var streamReader = new StreamReader(httpResponse.Content.ReadAsStream()))
            {
                var result = streamReader.ReadToEnd();
                AntiVPN_TShock.LastApiResult = result;
                dynamic data = JObject.Parse(result);
                getVPN = data.is_proxy;
            }

            if (getVPN)
                return true;

            else
                return false;

        };

        public static Func<string, string, Task<bool>> ShodanChecker = async (ip, key) =>
        {
            string[] getVPN;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api.shodan.io/shodan/host/{ip}?key={key}");

            var httpResponse = await AntiVPN_TShock.client.SendAsync(requestMessage);
            using (var streamReader = new StreamReader(httpResponse.Content.ReadAsStream()))
            {
                var result = streamReader.ReadToEnd();
                AntiVPN_TShock.LastApiResult = result;
                dynamic data = JObject.Parse(result);
                getVPN = data.tags;
            }

            if (getVPN.Count() > 0 && getVPN.Any(i => i.Equals("proxy", StringComparison.InvariantCultureIgnoreCase) || i.Equals("vpn", StringComparison.InvariantCultureIgnoreCase)))
                return true;

            else
                return false;


        };
    }
}
