using AntiVPN_TShock.Commands;
using AntiVPN_TShock.Configs;
using AntiVPN_TShock.Data;
using AntiVPN_TShock.Intro;
using AntiVPN_TShock.Lang;
using AntiVPN_TShock.VpnChecker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;


namespace AntiVPN_TShock
{
    [ApiVersion(2, 1)]
    public class AntiVPN_TShock : TerrariaPlugin
    {
        public override string Author => "hdseventh + Maxthegreat99";
        public override string Description => "A simple anti-vpn plugin allowing the use of multiple Ip checking APIs";
        public override string Name => "Anti-VPN Plugin";

        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public const string pluginFolder = "Anti-VPN";
        private static string ConfigPath => Path.Combine(TShock.SavePath, pluginFolder, "AntiVPNConfig.xml");

        public static AntiVPN_Config Config { get; set; } = new AntiVPN_Config(ConfigPath);

        private static Dictionary<string, int> TriesPerUsers = new Dictionary<string, int>();

        public const string tag = "[Anti-VPN] ";

        public static string LastApiResult;

        public static class Permissions
        {
            public const string useipban = "antivpn.ipban";

            public const string ignoreipban = "antivpn.ignoreipban";
        }

        public static readonly HttpClient client = new HttpClient();

        private static Dictionary<string, Func<string, string, Task<bool>>> ipCheckers = new();
        
        public AntiVPN_TShock(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            TShockAPI.Commands.ChatCommands.Add(new Command(Permissions.useipban, AntiVPN_Commands.IPBan, "ipban")
            {
                HelpText = AntiVPN_Lang.ipBanHelpText
            });

            TShockAPI.Commands.ChatCommands.Add(new Command(Permissions.useipban, AntiVPN_Commands.IPBanDel, "ipbandel")
            {
                HelpText = AntiVPN_Lang.ipBanDelHelpText
            });

            TShockAPI.Commands.ChatCommands.Add(new Command(Permissions.useipban, AntiVPN_Commands.IPBanInfo, "ipbaninfo")
            {
                HelpText = AntiVPN_Lang.ipBanInfoHelpText
            });

            TShockAPI.Commands.ChatCommands.Add(new Command(Permissions.useipban, AntiVPN_Commands.IPBanList, "ipbanlist")
            {
                HelpText = AntiVPN_Lang.ipBanListHelpText
            });

            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoinAsync);
        }

        private static void OnInitialize(EventArgs args)
        {
            if (!File.Exists(Path.Combine(TShock.SavePath, pluginFolder)))
            {
                Directory.CreateDirectory(Path.Combine(TShock.SavePath, pluginFolder));
            }

            IPBans.Initialize();

            bool writeConfig = true;
            if (File.Exists(ConfigPath))
            {
                writeConfig = Config.Read();
            }

            if (writeConfig)
            {
                var originalForeColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine(tag + AntiVPN_Lang.invalidOrNoConfigs);

                Console.ForegroundColor = originalForeColor;

                Config.Write();
            }

            if (IPBans.GetAll().Any())
            {
                ConsoleColor originalForeColour = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Cyan;

                Console.Write(tag + String.Format(AntiVPN_Lang.showIPBanCountMessage
                              , IPBans.GetAll().Count(), (TShock.Config.Settings.StorageType.ToLower() == "mysql") ? "MySQL" : "SQLite"));
                Console.ForegroundColor = originalForeColour;
            }

            cleanTrustedIps();

            setupApiList();

            AntiVPN_Intro.PrintIntro();
        }

        private async void OnJoinAsync(JoinEventArgs args)
        {
            if (TShock.Players[args.Who] == null || args.Handled)
                return;

            var player = TShock.Players[args.Who];

            if (IPBans.GetAll().Any(i => i.Identifiers.Contains(player.IP)))
            {
                var ban = IPBans.GetAll().First(i => i.Identifiers.Contains(player.IP));

                if (ban.Expiration < DateTime.UtcNow) IPBans.Remove(ban.ID);
                else player.Disconnect($"#{ban.ID} - " + ban.Reason);
                return;
            }

            if (!Config.Settings.Enabled)
                return;

            if (IPBans.GetAllTrustedIps().Any(i => i == player.IP))
                return;

            try
            {
                List<bool> isVPN = new();

                foreach (var checker in ipCheckers)
                    isVPN.Add(await checker.Value(player.IP, checker.Key));


                if (isVPN.Any(i => i))
                {
                    if (!player.HasPermission(Permissions.ignoreipban) && !TriesPerUsers.TryAdd(player.IP, 1))
                        TriesPerUsers[player.IP]++;

                    if (TriesPerUsers[player.IP] >= Config.Settings.ConnectionTriesLimit)
                    {
                        Console.WriteLine(tag + player.IP + AntiVPN_Lang.announceAutoIPBan);

                        IPBans.Insert(player.Name, AntiVPN_Lang.autoIPBanEntity, player.IP,
                                      DateTime.MaxValue, Config.Settings.AutoIPBanReason);

                        TriesPerUsers.Remove(player.IP);
                    }

                    player.Disconnect(Config.Settings.PositiveKickMessage);

                }
                else
                {
                    Console.WriteLine(tag + player.IP + AntiVPN_Lang.announceConnectionCheck);

                    IPBans.InsertTrustedIps(player.IP, DateTime.UtcNow);
                }
            }
            catch (Exception e)
            {
                if (Config.Settings.KickWhenError)
                    player.Disconnect(Config.Settings.ErrorKickMessage);

                ConsoleColor originalForeColour = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(tag + player.IP + AntiVPN_Lang.displayConnectionError + e);

                Console.WriteLine(tag + AntiVPN_Lang.showedResponseWhenError + LastApiResult);

                Console.ForegroundColor = originalForeColour;
            }



        }

        private static void cleanTrustedIps()
        {
            foreach(string ip in IPBans.GetAllTrustedIps())
            {
                var timeAdded = IPBans.GetTimeIpWasAdded(ip);

                if (timeAdded < DateTime.UtcNow.AddDays(Config.Settings.DaysBeforeDeletingTrustedIps))
                    IPBans.RemoveTrustedIp(ip);
            }
        }
        private static void setupApiList()
        {
            int i = 0;

            foreach(string str in Config.Settings.IpCheckers)
            {
                switch (str)
                {
                    case "iphub":
                        ipCheckers.Add(Config.Settings.ApiKeys[i], AntiVPN_IPChecker.IpHubChecker);
                        i++;
                        break;

                    case "proxycheck":
                        ipCheckers.Add(Config.Settings.ApiKeys[i], AntiVPN_IPChecker.ProxyCheckChecker);
                        i++;
                        break;

                    case "getipintel":
                        ipCheckers.Add(Config.Settings.ApiKeys[i], AntiVPN_IPChecker.GetIpIntelChecker);
                        i++;
                        break;

                    case "iptrooper":
                        ipCheckers.Add(Config.Settings.ApiKeys[i], AntiVPN_IPChecker.IpTrooperChecker);
                        i++;
                        break;

                    case "ipqualityscore":
                        ipCheckers.Add(Config.Settings.ApiKeys[i], AntiVPN_IPChecker.IpQualityScoreChecker);
                        i++;
                        break;

                    case "iphunter":
                        ipCheckers.Add(Config.Settings.ApiKeys[i], AntiVPN_IPChecker.IpHunterChecker);
                        i++;
                        break;

                    case "vpnblocker":
                        ipCheckers.Add(Config.Settings.ApiKeys[i], AntiVPN_IPChecker.VpnBlockerChecker);
                        i++;
                        break;

                    case "ip2location":
                        ipCheckers.Add(Config.Settings.ApiKeys[i], AntiVPN_IPChecker.Ip2LocationChecker);
                        i++;
                        break;

                    case "shodan":
                        ipCheckers.Add(Config.Settings.ApiKeys[i], AntiVPN_IPChecker.ShodanChecker);
                        i++;
                        break;

                    default:
                        ConsoleColor originalForeColour = Console.ForegroundColor;

                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.WriteLine(tag + String.Format(AntiVPN_Lang.invalidIpChecker, str));

                        Console.ForegroundColor = originalForeColour;

                        break;
                }
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoinAsync);
                client.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}