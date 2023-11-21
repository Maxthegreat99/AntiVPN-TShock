using AntiVPN_TShock.Lang;
using NuGet.Protocol;
using System;
using System.Linq;

namespace AntiVPN_TShock.Intro
{
    public static class AntiVPN_Intro
    {
        public static void PrintIntro()
        {
            ConsoleColor originalForeColour = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.Write(AntiVPN_TShock.tag + AntiVPN_Lang.successfullyLoadedMessage);

            Console.ForegroundColor = ConsoleColor.DarkRed;

            Console.Write("hdseventh");

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.Write(AntiVPN_Lang.successfullyLoadedMessage2);

            Console.ForegroundColor = ConsoleColor.Red;

            Console.Write("Maxthegreat99");

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.Write("!\n");

            Console.ForegroundColor = ConsoleColor.Yellow;

            if (!AntiVPN_TShock.Config.Settings.Enabled)
                Console.WriteLine(AntiVPN_TShock.tag + AntiVPN_Lang.announceVPNcheckIsDisabled);

            if ((AntiVPN_TShock.Config.Settings.ApiKeys.Count() != AntiVPN_TShock.Config.Settings.IpCheckers.Count() || AntiVPN_TShock.Config.Settings.ApiKeys.Any(i => i.Equals("YourKeyHere"))) 
                 && AntiVPN_TShock.Config.Settings.Enabled)
                Console.WriteLine(AntiVPN_TShock.tag + AntiVPN_Lang.warnThatKeyIsNotSet);

            if(AntiVPN_TShock.Config.Settings.ApiKeys.Any(i => i.Equals("your.email@here.com")) && AntiVPN_TShock.Config.Settings.Enabled)
                Console.WriteLine(AntiVPN_TShock.tag + AntiVPN_Lang.warnInvalidContactEmail);

            Console.ForegroundColor = ConsoleColor.Green;

            Console.Write(AntiVPN_Lang.specialThanksMessage);

            Console.ForegroundColor = ConsoleColor.Magenta;

            Console.Write("Crystal Lake");

            Console.ForegroundColor = ConsoleColor.Green;

            Console.Write(AntiVPN_Lang.specialThanksMessage2);

            Console.ForegroundColor = originalForeColour;
        }
    }
}
