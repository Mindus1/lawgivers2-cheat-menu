<div align="center">

# Cheat Menu

**Unlimited action points and money for Lawgivers II, one click away.**

![Lawgivers II 0.17.1](https://img.shields.io/badge/Lawgivers_II-0.17.1-F28019?style=flat-square)
![BepInEx 6 IL2CPP](https://img.shields.io/badge/BepInEx-6_IL2CPP-4B5563?style=flat-square)
![Needs Mod Manager](https://img.shields.io/badge/needs-Mod_Manager-6B7280?style=flat-square)
![Needs Mod Library](https://img.shields.io/badge/needs-Mod_Library-5A8DB8?style=flat-square)

<img src="docs/cheats-panel.png" alt="The Cheats panel" width="620">

</div>

## Cheats

| Cheat | What it does |
|---|---|
| **Unlimited action points** | Your action points stay at 9, however many you spend. Switching it off gives back what you had. |
| **Unlimited money** | Your party's money stays at 1T. Switching it off gives back the amount you had before. |
| **Unlimited black money** | The same for your party's black money. |

The padlock button sits next to Filters, Help and Settings in the top-right corner. It's open while any cheat is on, so you can see at a glance whether you're cheating.

## Install

1. Install [BepInEx 6 for IL2CPP games](https://builds.bepinex.dev/projects/bepinex_be) (x64, build 790 or newer). Start the game once so BepInEx can set itself up.
2. Install the [Mod Manager](https://github.com/Mindus1/lawgivers2-mod-manager) and the [Mod Library](https://github.com/Mindus1/lawgivers2-mod-library). The Cheat Menu needs both.
3. Download `CheatMenu.dll` from the [Releases](../../releases) page.
4. Put it in `Lawgivers II\BepInEx\plugins`, next to `ModManager.dll` and `ModLibrary.dll`.
5. Load a game and click the padlock in the top-right corner.

## Good to know

- **Your switches are remembered.** They're saved in `BepInEx\config\mindus.lawgivers2.cheatmenu.cfg`, so cheats that were on when you quit are on again next time.
- **What you had comes back.** When you switch on a cheat, your action points or money are saved in that config file. Switching the cheat off gives them back, even after a restart.
- **Switch the whole mod off** in **MODS**, from the main menu or the pause menu. That switches every cheat off, gives everything back, and hides the padlock. No restart needed.
- **Saving while a cheat is on** saves the cheated values (9 action points, 1T money) into your save.

## Build from source

You need the .NET SDK (6 or newer) and the game with BepInEx installed. Build and install the Mod Library and the Mod Manager first: this project references their installed DLLs.

```bash
dotnet build -c Release -p:GameDir="D:\Games\Steam\steamapps\common\Lawgivers II"
```

That copies `CheatMenu.dll` into `BepInEx\plugins`. Close the game first, or the copy fails because the DLL is in use.

### Add a cheat

Every cheat holds one number at a fixed value, remembers the number from before, and gives it back when switched off. To add one, add an entry to the list in `Cheats.Load`:

```csharp
new(config, "UnlimitedSomething", "SomethingBefore",
    "Unlimited something", "Your something stays at 100. Switching off gives back what you had.",
    lockedValue: 100,
    read: () => Instance.Player?.something,        // null while no game is loaded
    write: value => Instance.Player.something = (int)value,
    redraw: hud => hud.RefreshSomething()),
```

The panel gets a row for it automatically. The button, the panel and the per-frame update all come from the [Mod Library](https://github.com/Mindus1/lawgivers2-mod-library) (`GameHud`, `GameUi`, `GameEvents`), so `CheatMenu.cs` is only about the cheats.
