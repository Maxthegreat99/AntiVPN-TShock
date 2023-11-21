
using System.Reflection.Metadata;

namespace AntiVPN_TShock.Lang
{
    public static class AntiVPN_Lang
    {
        public const string ipBanHelpText = "[Anti-VPN] Usage: '/ipban <player/ip> (Optional) <reason> (Optional, must be in a '0d0h0m0s' format) <duration> (Optional) <flag>'" +
                                     "\nFlags:" +
                                     "\n'-d <index>' - flag to use the command as /ipbandel, should be used as first parameter." +
                                     "\n'-i <index>' - flag to use the command as /ipbaninfo, should be used as first parameter." +
                                     "\n'-l <page>' - flag to use the command as /ipbanlist, should be used as first parameter." +
                                     "\n'-ip' - flag determining whether or not the first argument is an ip, use as your last parameter." +
                                     "\nCommand explanation: Bans the target by not allowing their ip to join your server.";

        public const string ipBanListHelpText = "[Anti-VPN] Usage: '/<ipban -l/ipbanlist> <page, defaults to 1>'" +
                                         "\nCommand explanation: Lists and shows basic info on every currently registered IP ban.";

        public const string ipBanDelHelpText = "[Anti-VPN] Usage: '/<ipban -d/ipbandel> <index>'" +
                                        "\nCommand explanation: Deletes the specified IP ban from the database.";

        public const string ipBanInfoHelpText = "[Anti-VPN] Usage: '/<ipban -i/ipbaninfo> <index>'" +
                                         "\nCommand explanation: Shows detailed info on the specified IP ban.";

        public const string invalidOrNoConfigs = "Configs are either missing or incomplete! Generating missing settings...";

        public const string announceAutoIPBan = " has reached the connection tries limit, their IP has been added to the ipban database.";

        public const string autoIPBanEntity = "Automatic IP Ban Protocol";

        public const string announceConnectionCheck = "'s connection was successfully checked as non-proxy.";

        public const string displayConnectionError = "'s Connection is not checked, Error : ";

        public const string successfullyLoadedMessage = "Successfully loaded AntiVPN by ";

        public const string successfullyLoadedMessage2 = ", and updated by ";

        public const string announceVPNcheckIsDisabled = "AntiVPN check is disabled! Anyone using a VPN can join your server!";

        public const string warnThatKeyIsNotSet = "Warning: one of your keys is not configured yet, the Anti-VPN check will not work! Be sure to get your key on its site.";

        public const string warnInvalidContactEmail = "Warning: your contact email for getipintel is invalid, please change it to your real email address or else the site might ban you from using its API.";

        public const string invalidIpChecker = "Error while setting up APIs to use, '{0}' is not a valid API please refer to the documentation for a list of values of valid APIs to use: https://github.com/Maxthegreat99/AntiVPN-TShock";
        
        public const string specialThanksMessage = "[Maxthegreat99] Thank you so much to the";

        public const string specialThanksMessage2 = " Server for supporting my work and allowing this plugin to go public ! Please check out their Discord when you have some spare time: https://discord.gg/cXt6Urhhan\n";

        public const string invalidCommandUsage = "Invalid command usage!";

        public const string showedResponseWhenError = "Api response: ";

        public const string defaultIPBanTargetName = "Unknown";

        public const string defaultIPBanReason = "Misbehavior";

        public const string unableToParseTime = "Was unable to parse the <duration> parameter, please use the <0d0h0m0s> format";

        public const string playerNotFound = "No player or account of the name '{0}' was found!";

        public const string predictBanViaIP = "\nUse the -ip flag to ban a specific IP!";

        public const string targetAlreadyBanned = "The target already has IP their banned!";

        public const string banSuccess = "Successfully IPbanned '{0}' until '{1}' due to '{2}'.";

        public const string showIpIDMessage = "New IPBan ID: ";

        public const string successModifying = "Successfully modified IPBan{0}to contain {1}'s other matching IPs.";

        public const string disconectMessage = "You have been banned for: ";

        public const string ipNotValidMessage = " is not a valid IP Address!";

        public const string targetIsProtected = " cannot be IPbanned due to their permissions!";

        public const string ipIsAlreadyBanned = "This IP is already banned!";

        public const string showIPBanCountMessage = "Successfully regcognized {0} IPBans in the {1} database!\n";

        public const string ipBanListItemFormat = "[[c/00FFFF:{0}]] - IP's associated names: '{1}', IP Ban reason: '{2}', IP Ban expires on: '{3}'";

        public const string ipBanListHeaderFormat = "IPBans ({0}/{1}):";

        public const string ipBanListFooterFormat = "Type {0}ipban -l {{0}} for more.";

        public const string ipBanListNoBansFormat = "There are currently IP Bans.";

        public const string ipBanNoBansFound = "No corresponding IP ban was found!";

        public const string ipBanInfoShowBanFormat = "[c/90EE90:Showing details for IP ban {0}:]" +
                                                     "\nBan ID: {0}" +
                                                     "\nBan's associated names: {1}" +
                                                     "\nResponsible moderator's name: {2}" +
                                                     "\nReason: {3}" +
                                                     "\nExpiration: {4}" +
                                                     "\nIdentifiers: {5}";

        public const string ipBanDelSuccessfullDeletion = "Successfully deleted IP ban [[c/00FFFF:{0}]]";
    }
}
