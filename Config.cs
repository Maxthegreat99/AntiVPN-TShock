using TShockAPI;
using TShockAPI.Configuration;
using System.IO;

namespace AntiVPN_TShock
{
    public class AConfig
    {
        public string Key = "YourKeyHere";

        public string PositiveKickMessage = "AntiProxy: Proxy and VPN connections are not permitted.";

        public bool KickWhenError = false;
        
        public string ErrorKickMessage = "Server join failed, try again in a few minutes.";
    }
    public class AntiVPNConfig : ConfigFile<AConfig>
    {
    }
}
