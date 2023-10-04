using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace AntiVPN_TShock
{
    [ApiVersion(2, 1)]
    public class AntiVPN_TShock : TerrariaPlugin
    {
        public override string Author => "hdseventh + Maxthegreat99";
        public override string Description => "A simple anti-vpn plugin powered by iphub.info";
        public override string Name => "Anti-VPN Plugin";

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public const string pluginFolder = "Anti-VPN";
        private static string ConfigPath => Path.Combine(TShock.SavePath, pluginFolder, "AntiVPNConfig.json");

        public static AntiVPNConfig Config { get; set; } = new AntiVPNConfig();

        public const string tag = "[Anti-VPN] ";

        private static readonly HttpClient client = new HttpClient();

        public AntiVPN_TShock(Main game) : base(game)
        {
        }

        public override void Initialize()
        {

            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoinAsync);
        }

        private static void OnInitialize(EventArgs args)
        {
            if (!File.Exists(Path.Combine(TShock.SavePath, pluginFolder)))
            {
                Directory.CreateDirectory(Path.Combine(TShock.SavePath, pluginFolder));
            }

            bool writeConfig = true;
            if (File.Exists(ConfigPath))
            {
                Config.Read(ConfigPath, out writeConfig);
            }

            if (writeConfig)
            {
                var originalForeColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine(tag + "Configs are either missing or incomplete! Generating missing settings...");

                Console.ForegroundColor = originalForeColor;

                Config.Write(ConfigPath);
            }

            ConsoleColor originalForeColour = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.Write(tag + "Successfully loaded AntiVPN by ");

            Console.ForegroundColor = ConsoleColor.DarkRed;

            Console.Write("hdseventh");

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.Write(", and updated by ");

            Console.ForegroundColor = ConsoleColor.Red;

            Console.Write("Maxthegreat99");

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.Write("!\n");

            if ((Config.Settings.Key == "YourKeyHere" || AntiVPN_TShock.Config.Settings.Key == "") && AntiVPN_TShock.Config.Settings.Enabled)
                Console.WriteLine(tag + "Warning: Your key is not configured, the Anti-VPN check will not work! Get your key at https://iphub.info.");

            Console.ForegroundColor = ConsoleColor.Green;

            Console.Write("[Maxthegreat99] Big thanks to the guys at ");

            Console.ForegroundColor = ConsoleColor.Magenta;

            Console.Write("Crystal Lake");

            Console.ForegroundColor = ConsoleColor.Green;

            Console.Write(" for letting me modify and publish their commissioned plugin. They've been absolutely " +
                          "awesome in supporting me along the way." +
                          " Join their Discord and show them some love when you can!\n https://discord.gg/tFWzhWXFYh\n ");

            Console.ForegroundColor = originalForeColour;
        }

        private async void OnJoinAsync(JoinEventArgs args)
        {
            if (TShock.Players[args.Who] == null || args.Handled)
                return;

            var player = TShock.Players[args.Who];

            if (!Config.Settings.Enabled)
                return;

            try
            {
                string getVPN;
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://v2.api.iphub.info/ip/" + player.IP);

                requestMessage.Headers.Remove("X-Key");
                requestMessage.Headers.Add("X-Key", Config.Settings.Key);

                var httpResponse = await client.SendAsync(requestMessage);
                using (var streamReader = new StreamReader(httpResponse.Content.ReadAsStream()))
                {
                    var result = streamReader.ReadToEnd();
                    dynamic data = JObject.Parse(result);
                    getVPN = (string)data.SelectToken("block");
                }

                var statusCode = httpResponse.StatusCode;

                if (statusCode.ToString() == "OK"
                    && getVPN == "1")
                    player.Disconnect(Config.Settings.PositiveKickMessage);
                else
                    Console.WriteLine(tag + player.IP + "'s Connection is not checked, Status : " + statusCode);
                
            }
            catch (Exception e)
            {
                if (Config.Settings.KickWhenError)
                    player.Disconnect(Config.Settings.ErrorKickMessage);

                Console.WriteLine(tag + player.IP + "'s Connection is not checked, Error : " + e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoinAsync);
            }
            base.Dispose(disposing);
        }
    }
}