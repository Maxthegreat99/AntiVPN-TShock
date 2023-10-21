
# ANTI-VPN PLUGIN

A simple anti-vpn plugin for TShock powered by iphub.info 

- Originally made by [hdseventh](https://github.com/hdseventh)
- First ported as a commission by yours truly for the [Crytal Lake](https://discord.gg/tFWzhWXFYh) server who gave me permission to publish the plugin.

## How It Works

it will check the connections of joining users to see if they are using a VPN/proxy through an API online (Where you get your key from). 
So if you get an error from the plugin try to check if the API is down or not.

## How To Get A Key

if you didnt already you can get a free key from https://iphub.info by simply going to:
[Pricing] > ["Looking for the free plan (1000 req/day)? Here!"] > [register/login] > [New API key].

## Configs

- 'Enabled' - Whether or not the plugin checks for user's IPs as they join.
- 'Key' - Your key from https://iphub.info.
- 'PositiveKickMessage' - The kick message to send the user when the plugin detects that they are using a VPN.
- 'KickWhenError' - Whether or not the plugin should kick users when an error occurs within the actual VPN check. (Might kick users when the API is down)
- 'ErrorKickMessage' - The kick message shown the kicked players if the field above is true.

## Forked Repository
https://github.com/hdseventh/AntiVPN-TShock
