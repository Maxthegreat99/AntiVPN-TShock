using System;
using System.Linq;
using TShockAPI.DB;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Org.BouncyCastle.Utilities.Net;
using AntiVPN_TShock.Data;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using NuGet.Protocol;
using NuGet.Packaging;
using Microsoft.Xna.Framework;
using AntiVPN_TShock.Lang;
using Color = Microsoft.Xna.Framework.Color;

namespace AntiVPN_TShock.Commands
{
    [Flags]
    public enum IPBanProperties
    {
        NONE = 1 << 0,
        PLAYER = 1 << 1,
        EXPIRED = 1 << 2,
        HASMULTIPLEMATCHINGACCS = 1 << 3,
        ACCOUNT = 1 << 4,
        NEEDTOADDIPS = 1 << 5,

        ALL = ACCOUNT | PLAYER | NEEDTOADDIPS | EXPIRED | HASMULTIPLEMATCHINGACCS
    }
    public static class AntiVPN_Commands
    {
        public static void IPBanDel(CommandArgs args)
        {
            var player = args.Player;

            if (args.Parameters.Count == 0 || !int.TryParse(args.Parameters[0], out int index))
            {
                player.SendErrorMessage(AntiVPN_TShock.tag + AntiVPN_Lang.invalidCommandUsage);
                player.SendInfoMessage(AntiVPN_TShock.tag + AntiVPN_Lang.ipBanDelHelpText);
                return;
            }

            var IPBan = IPBans.Get(index);

            if (IPBan == null)
            {
                player.SendErrorMessage(AntiVPN_TShock.tag + AntiVPN_Lang.ipBanNoBansFound);

                return;
            }

            IPBans.Remove(IPBan.ID);

            player.SendSuccessMessage(AntiVPN_TShock.tag + AntiVPN_Lang.ipBanDelSuccessfullDeletion, IPBan.ID);
        }
        public static void IPBanInfo(CommandArgs args)
        {
            var player = args.Player;

            if(args.Parameters.Count == 0 || !int.TryParse(args.Parameters[0], out int index))
            {
                player.SendErrorMessage(AntiVPN_TShock.tag + AntiVPN_Lang.invalidCommandUsage);
                player.SendInfoMessage(AntiVPN_TShock.tag + AntiVPN_Lang.ipBanInfoHelpText);
                return;
            }

            var IPBan = IPBans.Get(index);

            if(IPBan == null)
            {
                player.SendErrorMessage(AntiVPN_TShock.tag + AntiVPN_Lang.ipBanNoBansFound);

                return;
            }


            player.SendInfoMessage(AntiVPN_TShock.tag + AntiVPN_Lang.ipBanInfoShowBanFormat,
                                   IPBan.ID, IPBan.TargetName, IPBan.ResponsibleName, IPBan.Reason,
                                   IPBan.Expiration, IPBan.Identifiers);
        }

        public static void IPBanList(CommandArgs args)
        {
            var player = args.Player;

            if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, player, out int pageNumber))
            {
                player.SendErrorMessage(AntiVPN_TShock.tag + AntiVPN_Lang.invalidCommandUsage);
                player.SendInfoMessage(AntiVPN_TShock.tag + AntiVPN_Lang.ipBanListHelpText);
                return;
            }

            var ipBans = IPBans.GetAll();

            List<string> ipBanList = new();

            foreach (var i in ipBans)
                ipBanList.Add(String.Format(AntiVPN_Lang.ipBanListItemFormat, i.ID, i.TargetName, i.Reason, i.Expiration));

            PaginationTools.SendPage(player, pageNumber, ipBanList,
                new PaginationTools.Settings
                {
                    HeaderFormat = AntiVPN_Lang.ipBanListHeaderFormat,
                    FooterFormat = AntiVPN_Lang.ipBanListFooterFormat.SFormat(TShockAPI.Commands.Specifier),
                    NothingToDisplayString = AntiVPN_Lang.ipBanListNoBansFormat
                });
        }
        public static void IPBan(CommandArgs args)
        {
            var player = args.Player;

            if (args.Parameters.Count < 1 || args.Parameters.Count > 4)
            {
                player.SendErrorMessage(AntiVPN_TShock.tag + AntiVPN_Lang.invalidCommandUsage);
                player.SendInfoMessage(AntiVPN_Lang.ipBanHelpText);

                return;
            }

            if ((args.Parameters[0] == "list" || args.Parameters[0] == "-l")
                && TShock.Players != null && !TShock.Players.Any(i => i != null
                                             && (i.Name.StartsWith("list") || i.Name.StartsWith("-l")))
                )
            {
                int page = 0;
                TShockAPI.Commands.HandleCommand(player, String.Format("{0}ipbanlist{1}",
                                                 TShock.Config.Settings.CommandSpecifier,
                                                 (args.Parameters.Count >= 2 && int.TryParse(args.Parameters[1], out page)) ? " " + page : ""));
                return;
            }

            if ((args.Parameters[0] == "del" || args.Parameters[0] == "-d")
                && TShock.Players != null && !TShock.Players.Any(i => i != null
                                             && (i.Name.StartsWith("del") || i.Name.StartsWith("-d")))
                )
            {
                int index = 0;
                TShockAPI.Commands.HandleCommand(player, String.Format("{0}ipbandel{1}",
                                                 TShock.Config.Settings.CommandSpecifier,
                                                 (args.Parameters.Count >= 2 && int.TryParse(args.Parameters[1], out index)) ? " " + index : ""));
                return;
            }

            if ((args.Parameters[0] == "info" || args.Parameters[0] == "-i")
                && TShock.Players != null && !TShock.Players.Any(i => i != null
                                             && (i.Name.StartsWith("info") || i.Name.StartsWith("-i")))
                )
            {
                int index = 0;
                TShockAPI.Commands.HandleCommand(player, String.Format("{0}ipbaninfo{1}",
                                                 TShock.Config.Settings.CommandSpecifier,
                                                 (args.Parameters.Count >= 2 && int.TryParse(args.Parameters[1], out index)) ? " " + index : ""));
                return;
            }

            bool isIp = (args.Parameters[args.Parameters.Count - 1] == "-ip");
            TSPlayer target = null;
            string targetName = AntiVPN_Lang.defaultIPBanTargetName;
            string reason = (args.Parameters.Count >= 2) ? args.Parameters[1] : AntiVPN_Lang.defaultIPBanReason;
            int duration = (args.Parameters.Count >= 3 && TShock.Utils.TryParseTime(args.Parameters[2], out duration)) ? duration : DateTime.MaxValue.Second;
            DateTime expiration = (duration == DateTime.MaxValue.Second) ? DateTime.MaxValue : DateTime.UtcNow.AddSeconds(duration);
            if (args.Parameters.Count >= 3 && duration == DateTime.MaxValue.Second)
            {
                player.SendErrorMessage(AntiVPN_TShock.tag + AntiVPN_Lang.unableToParseTime);
                return;
            }

            if (!isIp)
            {
                target = TSPlayer.FindByNameOrID(args.Parameters[0]).FirstOrDefault();
                bool failure = false;
                UserAccount acc = null;

                if (target == null)
                {
                    acc = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);

                    if (acc == null) failure = true;
                    else targetName = acc.Name;
                }
                else targetName = target.Name;

                if (failure)
                {
                    player.SendErrorMessage(AntiVPN_TShock.tag + AntiVPN_Lang.playerNotFound.SFormat(args.Parameters[0]) +
                                           (IPAddress.IsValid(args.Parameters[0]) ? AntiVPN_Lang.predictBanViaIP : ""));
                    return;
                }

                if((target == null) ? TShock.Groups.GetGroupByName(acc.Group).HasPermission(AntiVPN_TShock.Permissions.ignoreipban) :
                    target.HasPermission(AntiVPN_TShock.Permissions.ignoreipban))
                {
                    player.SendErrorMessage(AntiVPN_TShock.tag + targetName + AntiVPN_Lang.targetIsProtected);
                    return;
                }

                var getPropertiesResults = getIPBanProperties(target, acc);

                bool areOperationsRequired = (getPropertiesResults & IPBanProperties.NEEDTOADDIPS) == IPBanProperties.NEEDTOADDIPS
                                             || (getPropertiesResults & IPBanProperties.HASMULTIPLEMATCHINGACCS) == IPBanProperties.HASMULTIPLEMATCHINGACCS;

                if ((getPropertiesResults & IPBanProperties.HASMULTIPLEMATCHINGACCS) == IPBanProperties.HASMULTIPLEMATCHINGACCS)
                    getPropertiesResults = handleMultipleMatchingAccs(acc);

                if ((getPropertiesResults & IPBanProperties.NEEDTOADDIPS) == IPBanProperties.NEEDTOADDIPS)
                    getPropertiesResults = addIPsToBan(acc);

                if (!areOperationsRequired 
                    && (getPropertiesResults & IPBanProperties.NONE) != IPBanProperties.NONE 
                    && (getPropertiesResults & IPBanProperties.EXPIRED) != IPBanProperties.EXPIRED)
                {
                    player.SendErrorMessage(AntiVPN_TShock.tag + AntiVPN_Lang.targetAlreadyBanned);
                    return;
                }

                if ((getPropertiesResults & IPBanProperties.EXPIRED) == IPBanProperties.EXPIRED)
                {
                    JArray identifiers = (target == null) ? JArray.Parse(acc.KnownIps) : new JArray(target.IP);

                    IPBan ipban = IPBans.GetAll().First(i => identifiers.Any(j => i.Identifiers.Contains(j.ToString())));

                    if (isJSONstring(ipban.Identifiers))
                        identifiers.AddRange(JArray.Parse(ipban.Identifiers));

                    identifiers = new JArray(identifiers.DistinctBy(i => i.ToString()));

                    List<string> associatedNames = new List<string> { ipban.TargetName };

                    if (!ipban.TargetName.Contains(targetName))
                        associatedNames.Add(targetName);

                    IPBans.Update(ipban.ID, String.Join("; ", associatedNames), player.Name, identifiers.ToJson(),
                                  expiration, (reason == AntiVPN_Lang.defaultIPBanReason) ? ipban.Reason : reason);

                }
                else                
                    IPBans.Insert(targetName, player.Name, (target == null) ? acc.KnownIps : target.IP, 
                                  expiration, reason);
                
                if (!areOperationsRequired)
                {
                    player.SendSuccessMessage(AntiVPN_TShock.tag + AntiVPN_Lang.banSuccess.SFormat(targetName, 
                                             expiration, reason));

                    JArray identifiers = (target == null) ? JArray.Parse(acc.KnownIps) : new JArray(target.IP);
                    IPBan ipban = IPBans.GetAll().First(i => identifiers.Any(j => i.Identifiers.Contains(j.ToString())));

                    player.SendSuccessMessage(AntiVPN_TShock.tag + AntiVPN_Lang.showIpIDMessage + "[c/" + Color.Cyan.Hex3() + ":" + ipban.ID.ToString() + "]");
                }
                else
                {
                    IPBan ipbanModified = IPBans.GetAll().First(i => JArray.Parse(acc.KnownIps)
                                          .Any(j => i.Identifiers.Contains(j.ToString())));

                    player.SendSuccessMessage(AntiVPN_TShock.tag + AntiVPN_Lang.successModifying.SFormat(
                                              " [[c/" + Color.Cyan.Hex3() + ":" + ipbanModified.ID.ToString() + "]] ", 
                                              targetName ));
                }

                if (target != null)
                    target.Disconnect(AntiVPN_Lang.disconectMessage + reason);

                return;

            }
            else
            {
                if (!IPAddress.IsValid(args.Parameters[0]))
                {
                    player.SendErrorMessage(AntiVPN_TShock.tag + args.Parameters[0] + AntiVPN_Lang.ipNotValidMessage);
                    return;
                }

                if (TShock.Utils.GetActivePlayerCount() > 0 && TShock.Players.Any(i => i != null && i.IP != null 
                    && i.IP == args.Parameters[0]))
                {
                    target = TShock.Players.First(i => i.IP == args.Parameters[0]);
                    
                    if (target.HasPermission(AntiVPN_TShock.Permissions.ignoreipban))
                    {
                        player.SendErrorMessage(AntiVPN_TShock.tag + target.Name + AntiVPN_Lang.targetIsProtected);
                        return;
                    }
                    
                    targetName = target.Name;
                    target.Disconnect(AntiVPN_Lang.disconectMessage + reason);
                }
                
                else if (TShock.UserAccounts.GetUserAccounts().Any(i => i.KnownIps.Contains(args.Parameters[0])))
                {
                    var acc = TShock.UserAccounts.GetUserAccounts().First(i => i.KnownIps.Contains(args.Parameters[0]));
                    if (TShock.Groups.GetGroupByName(acc.Group).HasPermission(AntiVPN_TShock.Permissions.ignoreipban))
                    {
                        player.SendErrorMessage(AntiVPN_TShock.tag + acc.Name + AntiVPN_Lang.targetIsProtected);
                        return;
                    }
                    targetName = acc.Name;
                }

                bool BanExpired = false;

                var IPBan = IPBans.GetAll().FirstOrDefault(i => i.Identifiers.Contains(args.Parameters[0]));

                if (IPBan != null && IPBan.Expiration > DateTime.UtcNow)
                {
                    player.SendErrorMessage(AntiVPN_TShock.tag + AntiVPN_Lang.ipIsAlreadyBanned);
                    return;
                }

                if (IPBan != null && IPBan.Expiration < DateTime.UtcNow)
                    BanExpired = true;

                if (BanExpired)
                {
                    JArray identifiers = new JArray(args.Parameters[0]);

                    if (isJSONstring(IPBan.Identifiers))
                        identifiers.AddRange(new JArray(IPBan.Identifiers));

                    identifiers = new JArray(identifiers.DistinctBy(i => i.ToString()));

                    IPBans.Update(IPBan.ID, targetName, player.Name, identifiers.ToJson(),
                                  expiration, reason);

                }
                else
                    IPBans.Insert(targetName, player.Name, args.Parameters[0],
                                  expiration, reason);

                player.SendSuccessMessage(AntiVPN_TShock.tag + AntiVPN_Lang.banSuccess.SFormat( 
                                          (targetName == AntiVPN_Lang.defaultIPBanTargetName ? args.Parameters[0] : targetName),
                                          expiration, reason));

                IPBan ipban = IPBans.GetAll().First(i => i.Identifiers.Contains(args.Parameters[0]));

                player.SendSuccessMessage(AntiVPN_TShock.tag + AntiVPN_Lang.showIpIDMessage + "[c/" + Color.Cyan.Hex3() + ":" + ipban.ID.ToString() + "]");

                return;
            }

        }
        private static IPBanProperties addIPsToBan(UserAccount acc)
        {
            IPBanProperties properties = IPBanProperties.ACCOUNT;

            JArray accKnownIPs = JArray.Parse(acc.KnownIps);

            var ipBan = IPBans.GetAll().First(i => accKnownIPs.
                               Any(j => i.Identifiers.Contains(j.ToString())));

            JArray allIdentifiers = new JArray(accKnownIPs);

            if (isJSONstring(ipBan.Identifiers))
                allIdentifiers.AddRange(JArray.Parse(ipBan.Identifiers));
            else
                allIdentifiers.Add(ipBan.Identifiers);

            allIdentifiers = new JArray(allIdentifiers.DistinctBy(i => i.ToString()));

            List<string> associatedNames = new List<string> { acc.Name, ipBan.TargetName };

            associatedNames = associatedNames.DistinctBy(i => i).ToList();

            IPBans.Update(ipBan.ID, String.Join("; ", associatedNames), ipBan.ResponsibleName,
              allIdentifiers.ToJson(), ipBan.Expiration, ipBan.Reason);

            var updatedBan = IPBans.Get(ipBan.ID);

            if (updatedBan.Expiration < DateTime.UtcNow)
                properties |= IPBanProperties.EXPIRED;

            return properties;
        }
        private static IPBanProperties handleMultipleMatchingAccs(UserAccount acc)
        {
            IPBanProperties properties = IPBanProperties.ACCOUNT;
            JArray accKnownIPs = JArray.Parse(acc.KnownIps);

            var allBans = IPBans.GetAll();

            List <IPBan> matchingIPBans = allBans.
                                          Where(i => accKnownIPs.
                              Any(j => i.Identifiers.Contains(j.ToString()))).ToList();

            JArray allIdentifiers = new JArray(accKnownIPs);

            matchingIPBans.ForEach(i => allIdentifiers.AddRange(JArray.Parse(i.Identifiers).ToArray()));

            allIdentifiers = new JArray(allIdentifiers.DistinctBy(i => i.ToString()));

            var BanToUpdate = matchingIPBans.First();

            List<string> AssociatedNames = new List<string>{acc.Name};

            foreach(var IPBan in matchingIPBans)
            {
                AssociatedNames.Add(IPBan.TargetName);

                if (IPBan.ID == BanToUpdate.ID)
                    continue;

                IPBans.Remove(IPBan.ID);
            }
            AssociatedNames = AssociatedNames.DistinctBy(i => i).ToList();

            IPBans.Update(BanToUpdate.ID, String.Join("; ", AssociatedNames), BanToUpdate.ResponsibleName,
                          allIdentifiers.ToJson(), BanToUpdate.Expiration, BanToUpdate.Reason);

            var updatedBan = IPBans.Get(BanToUpdate.ID);

            if (updatedBan.Expiration < DateTime.UtcNow)
                properties |= IPBanProperties.EXPIRED;

            return properties;
        }
        private static IPBanProperties getIPBanProperties(TSPlayer potentialPlayer, UserAccount potentialAcc)
        {
            IPBanProperties properties = IPBanProperties.NONE;

            var allIPBans = IPBans.GetAll();

            if(potentialAcc != null && 
               JArray.Parse(potentialAcc.KnownIps).
               Any(i => allIPBans.Any(j => j.Identifiers.Contains(i.ToString()))))
            {
                JArray accKnownIPS = JArray.Parse(potentialAcc.KnownIps);

                List < IPBan > matchingIPBans = allIPBans.
                              Where(i => accKnownIPS.
                              Any(j => i.Identifiers.Contains(j.ToString()))).ToList();

                if(matchingIPBans.Count > 1)                
                    properties |= IPBanProperties.HASMULTIPLEMATCHINGACCS;

                else if(isJSONstring(matchingIPBans.FirstOrDefault().Identifiers))
                    properties |= IPBanProperties.ACCOUNT;

                else
                    properties |= IPBanProperties.PLAYER;

                if (accKnownIPS.Any(i => !matchingIPBans.FirstOrDefault().
                    Identifiers.Contains(i.ToString())) 
                    && matchingIPBans.Count == 1)
                    properties |= IPBanProperties.NEEDTOADDIPS;

                if (matchingIPBans.Count == 1
                   && matchingIPBans.FirstOrDefault().Expiration < DateTime.UtcNow)
                    properties |= IPBanProperties.EXPIRED;
            }

            if(potentialPlayer != null 
               && allIPBans.Any(i => i.Identifiers.Contains(potentialPlayer.IP)))
            {
                IPBan ipban = allIPBans.First(i => i.Identifiers.Contains(potentialPlayer.IP));

                if(isJSONstring(ipban.Identifiers))
                    properties |= IPBanProperties.ACCOUNT;

                else
                    properties |= IPBanProperties.PLAYER;

                if(ipban.Expiration < DateTime.UtcNow)
                    properties |= IPBanProperties.EXPIRED;
            }

            if ((properties & IPBanProperties.ALL) != 0)
                properties &= ~IPBanProperties.NONE;

            return properties;

        }

        private static bool isJSONstring(string str)
        {
            var token = JToken.Parse(str);

            if (token is JArray)
                return true;

            return false;
        }
    }
}
