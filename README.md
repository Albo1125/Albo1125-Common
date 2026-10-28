# Belgs Common
Belgs Common is a helper library for GTA V and LSPDFR plugins, updated for current versions of LSPDFR and RAGE Plugin Hook.

It is a fork of [Albo1125.Common](https://github.com/Albo1125/Albo1125-Common) by Albo1125.

## Credits
* **[Albo1125](https://github.com/Albo1125)**: original author of Albo1125.Common, which this project is built on.
  * [Original download page](https://www.lcpdfr.com/files/file/10294-albo1125common/)
  * [YouTube channel](https://www.youtube.com/channel/UCSDMQS6b2roa-dZ0vAyaVWg)
  * [Why Albo1125 open sourced their LSPDFR plugins](https://www.lcpdfr.com/forums/topic/87615-open-sourcing-albo1125s-mods-retirement/)
  * [Donate to Albo1125](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=T9T5RTSWX8PEY)
* **[PeterU](https://github.com/LSPDFR-PeterU)**: the `FriendlyKeys` class (friendly names for keybinds), licence plate randomisation and ELS vehicle detection.

## Changes from Albo1125.Common
* Targets .NET Framework 4.8 and the current LSPDFR, and builds against RAGE Plugin Hook 1.124.
* RAGE Plugin Hook version check fixed for RPH 1.100 and newer.
* SDK-style project that builds with Visual Studio 2022 or `dotnet build`, without the mscoree COM reference.
* Update checks use HTTPS.
* Bug fixes: stack overflow in `CommonVariables.CarsToSelectFrom` / `MotorbikesToSelectFrom`, and empty key names in `FriendlyKeys` for keys missing from the lookup.

## Code & building
1. Copy `LSPD First Response.dll` from your [LSPDFR](https://www.lcpdfr.com/files/file/7792-lspd-first-response/) download into a new **Albo1125.Common/dependencies** folder.
2. Build the solution. The RAGE Plugin Hook SDK comes from NuGet; to use the one from your [RPH download](https://ragepluginhook.net/Downloads.aspx) instead, place `RagePluginHookSDK.dll` in the same `dependencies` folder.

The output is `Albo1125.Common/bin/Release/Albo1125.Common.dll`, which goes in the root of your GTA V folder.

## Licence
Licensed under the GNU General Public License v3, like the original. See [LICENSE.txt](LICENSE.txt).
