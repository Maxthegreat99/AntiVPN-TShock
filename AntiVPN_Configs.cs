using System.Collections.Generic;
using Terraria.Plugins.Common;
using TShockAPI.Configuration;

namespace AntiVPN_TShock.Configs
{
    public class AConfig
    {

        [XmlComment(Value = "defines whether or not the plugin checks for user's IPs as they join.")]
        public bool Enabled { get; set; } = true;

        [XmlComment(Value = "The kick message to send the user when the plugin detects that they are using a VPN.")]
        public string PositiveKickMessage { get; set; } = "AntiProxy: Proxy and VPN connections are not permitted.";

        [XmlComment(Value = "Whether or not the plugin should kick users when an error occurs within the actual VPN check. (Might kick users when the API is down)")]
        public bool KickWhenError { get; set; } = false;

        [XmlComment(Value = "The kick message shown to kicked players if the field above is true.")]
        public string ErrorKickMessage { get; set; } = "Server join failed, try again in a few minutes.";

        [XmlComment(Value = "The amount of tries an VPN IP can have at connecting to the server before getting IP banned.")]
        public int ConnectionTriesLimit { get; set; } = 5;

        [XmlComment(Value = "The ipban reason showed to players when they have reached their connection attempts limit.")]
        public string AutoIPBanReason { get; set; } = "Your connection has been rejected too many times";

        [XmlComment(Value = "List of APIs the plugin can use, can only contain these values:" +
                            "\n  - 'proxycheck'" +
                            "\n  - 'iptrooper'" +
                            "\n  - 'getipintel'" +
                            "\n  - 'ipqualityscore'" +
                            "\n  - 'iphub'" +
                            "\n  - 'iphunter'" +
                            "\n  - 'vpnblocker'" +
                            "\n  - 'ip2location'" +
                            "\n  - 'shodan'" + 
                            "\n  You can get keys for most of them for free, please refer to the documentation for more details.")]
        public string[] IpCheckers { get; set; } = {"proxycheck", "getipintel"};

        [XmlComment(Value = "List of keys that the API needs, the order of the key for a specific api should be the same as the order of the API in 'IPCheckers'." +
                            "\n  Please refer to the documentation for more details.")]
        public string[] ApiKeys { get; set; } = { "YourKeyHere", "your.email@here.com" };

        [XmlComment(Value = "Days it takes for the plugin to remove a trusted ip from its Database")]
        public int DaysBeforeDeletingTrustedIps { get; set; } = 7;
    }
    public class AntiVPN_Config : XmlConfigFile<AConfig>
    {
        public AntiVPN_Config(string path) : base(path)
        {
        }
    }
}
