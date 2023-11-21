
# Anti-VPN Plugin

A TShock plugin adding functionalities for IPBan and prevents users to join with VPNs.
- Originally made by [hdseventh](https://github.com/hdseventh)
- First ported as a commission by yours truly for the [Crytal Lake](https://discord.gg/tFWzhWXFYh) server who gave me permission to publish the plugin.

## Important
To use this plugin you need to install PluginCommonLibrary Version `>= 4.2.0.5`, heres where you need to get it: https://github.com/Maxthegreat99/PluginCommonLibrary/releases/tag/4.2.0.5, **Please read the release notes before downloading, thank you**

## How It Works

### AntiVPN:
For any non-ipbanned users it will check their connection through a configurable list of APIs online (Where you get your keys from). So if you get an error from the plugin try to check if the API online is down or not.

### AntiBot:
if a connection is rejected too many times, depending on whats set in the configs the user gets automatically IP-Ban, this means that when they try to join again the plugin wont even check if they have a VPN connection and will directly reject their connection.

### IPBan:
The plugin tries to regroup similar ipbans, if you ban an account for example and they have several known IPs, some of which have already been banned then the plugin will try to update the existing IP Ban instead of creating a new ban. 

## Useable APIs

Here are the APIs the plugin can use, the sites where you are meant to get your key are also mentioned. If the API has no key  the value of `ApiKeys` should be "", if the API requires a contact email to use, the value should be your contact email:

```md
  # the values that you can insert into `IpCheckers`
  - 'proxycheck'
  - 'iptrooper'
  - 'getipintel'
  - 'ipqualityscore'
  - 'iphub'
  - 'iphunter'
  - 'vpnblocker'
  - 'ip2location'
  - 'shodan'

  # https://proxycheck.io
  # Results updated Jan 19, 2020
  # Error rate:                     0.00%
  # NordVPN detection rate:       100.00%
  # Cryptostorm detection rate:   100.00%
  # False-flagged homes:            0.00%

  # https://iptrooper.net/
  # Results updated Jan 19, 2020
  # Error rate:                     0.00%
  # NordVPN detection rate:        96.00%
  # Cryptostorm detection rate:   100.00%
  # False-flagged homes:            0.00%

  # https://www.getipintel.net/
  # Results updated Jan 19, 2020
  # Error rate:                     0.00%
  # NordVPN detection rate:        86.00%
  # Cryptostorm detection rate:   100.00%
  # False-flagged homes:            0.00%

  # https://www.ipqualityscore.com/
  # Results updated Feb 9, 2020
  # Error rate:                    0.00%
  # NordVPN detection rate:       86.00%
  # Cryptostorm detection rate:   96.43%
  # False-flagged homes:           0.00%

  # https://iphub.info/
  # Results updated Jan 19, 2020
  # Error rate:                    0.00%
  # NordVPN detection rate:       84.00%
  # Cryptostorm detection rate:   96.43%
  # False-flagged homes:           0.00%

  # https://www.iphunter.info/
  # Results updated Jan 19, 2020
  # Error rate:                    0.00%
  # NordVPN detection rate:       60.00%
  # Cryptostorm detection rate:   92.86%
  # False-flagged homes:           0.00%

  # https://vpnblocker.net/usage
  # Results updated Jan 19, 2020
  # Error rate:                    0.00%
  # NordVPN detection rate:       64.00%
  # Cryptostorm detection rate:   82.14%
  # False-flagged homes:           0.00%

  # https://www.ip2location.io
  # NOTE: This info is old
  # Results updated Jul 18, 2019
  # Error rate: 0%
  # NordVPN detection rate: 100%
  # Cryptostorm detection rate: 60%
  # False-flagged homes: 0%

  # https://www.shodan.io/
  # Results updated Jan 19, 2020
  # Error rate:                  55.15%
  # NordVPN detection rate:      90.00%
  # Cryptostorm detection rate:   0.00%
  # False-flagged homes:          0.00%

Source: https://www.spigotmc.org/resources/anti-vpn.58291/
```

## Permissions

Here are the plugin's permissions:

- `antivpn.ipban` - This allows you to use the `/ipban` command/sub-commands.
- `antivpn.ignoreipban` - This protects you (For the most part) from being IP banned.

## Configs

- `Enabled` - Whether or not the plugin checks for user's IPs as they join.
- `PositiveKickMessage` - The kick message to send the user when the plugin detects that they are using a VPN.
- `KickWhenError` - Whether or not the plugin should kick users when an error occurs within the actual VPN check. (Might kick users when the API is down)
- `ErrorKickMessage` - The kick message shown to kicked players if the field above is true.
- `ConnectionTriesLimit` - The amount of tries a VPN IP can have at connecting to the server before getting IP banned.
- `AutoIPBanReason` - The ipban reason showed to players when they have reached their connection attempts limit.
- `IpCheckers` - List of APIs the plugin can use, can only contain the values cited above.
- `ApiKeys` - List of keys that the API needs, the order of the key for a specific api should be the same as the order of the API in `IPCheckers`.
- `DaysBeforeDeletingTrustedIps` - Days it takes for the plugin to remove a trusted ip from its Database.

## Commands

- `/ipban <player/ip> (Optional) <reason> (Optional, must be in a '0d0h0m0s' format) <duration> (Optional) <flag>`: 
Bans the IP of the target for the specified amount of time. 

### Flags:
`-ip` - this flag should always be positioned as your last parameter, 
it determines whether or not the first parameter is an IP address, 
this command can ipban unregistered online players as well as offline players. 

`-l <page>` - this flag should be used as the first parameter, 
it lists the current active IP-bans, 
a second parameter can be used to specify the page.

`-d <index>` - this flag too should be used as first parameter,
it deletes the specified ban using the specified ID.

`-i <index>` - this flag should also be used as the first parameter,
gives info about the specified ban.

each of the command flags listed above have their command variant,
`/ipbanlist`, `/ipbandel` and `/ipbaninfo` respectively.

### Notes: 
- if you ipban an offline user they will have all their 'Known IPS' banned that is all the IPS the user have had logined with.

- If you ipban someone and remove the plugin,  the ban wont be valid anymore and the person will be able to join again, so be careful when cleaning your plugins folder! 

- `Enabled` field in configs dont affect anything about IPBan.


## Translating

Most if not all of the plugin's text is located in the `AntiVPN_Laang.cs` file, you should not have too much trouble translating tho if you require assistance im always willing to help. Tho one thing is that you may encounter texts with symbols such as `{0}`, `{1}` etc.. in them, do not delete those from the text as it might cause some commands to simply not work, these are where specific variable texts like player names or indexes should be, hope this helps while translating.

## Thank You <3 !!!
Special thanks to the [Crystal Lake](https://discord.gg/cXt6Urhhan) community for generously allowing me to publicly publish their commission, despite the time it took to complete. Without their support, this plugin wouldn't exist. If you're using this plugin, I highly recommend checking out their Discord server. Even if you don't speak Spanish, sending them kind words for their contribution costs nothing. I would also like to express my gratitude to everyone who encouraged me to continue porting plugins, whether by using them or supporting me on [Buy Me A Coffee](https://www.buymeacoffee.com/maxthegreat). Time is often limited for me, so seeing others benefit from my work is truly motivating. I hope you enjoy the plugin, and if you encounter any bugs, please report them on this [discord server](https://discord.gg/xmHax4BuUR).

## Forked Repository
https://github.com/hdseventh/AntiVPN-TShock
