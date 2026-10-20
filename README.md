<div align="center">

# Cheat Menu

Unlimited action points and money for Lawgivers II.

![Lawgivers II 0.17.1](https://img.shields.io/badge/Lawgivers_II-0.17.1-F28019?style=flat-square)
![BepInEx 6 IL2CPP](https://img.shields.io/badge/BepInEx-6_IL2CPP-4B5563?style=flat-square)
![License: MIT](https://img.shields.io/badge/license-MIT-6B7280?style=flat-square)

<img src="docs/cheats-panel.png" alt="The Cheats panel" width="620">

</div>

The Cheat Menu adds a padlock button to the top-right corner of the HUD, next to Filters, Help and Settings. Clicking it opens a small panel with three cheats:

| Cheat | What it does |
|---|---|
| Unlimited action points | Keeps your action points at 9. |
| Unlimited money | Keeps your party's money at 1T. |
| Unlimited black money | Keeps your party's black money at 1T. |

When you turn a cheat off, you get back whatever you had before you turned it on. That value is stored in `BepInEx\config\mindus.lawgivers2.cheatmenu.cfg`, so it survives a restart too. The cheats you leave on stay on the next time you play. The padlock is open while any cheat is active.

If you save while a cheat is on, the cheated values end up in your save.

## Installation

The Cheat Menu needs the [Mod Library](https://github.com/Mindus1/lawgivers2-mod-library) and the [Mod Manager](https://github.com/Mindus1/lawgivers2-mod-manager).

1. Install [BepInEx 6 for IL2CPP](https://builds.bepinex.dev/projects/bepinex_be) (x64, build 790 or newer) and start the game once so it can set itself up.
2. Install the Mod Library and the Mod Manager.
3. Download `CheatMenu.dll` from [Releases](../../releases) and put it into `Lawgivers II\BepInEx\plugins`.

You can switch the whole mod off in the MODS menu without restarting. That turns every cheat off, gives you your old values back and hides the padlock.

## Building

You need the .NET SDK (6 or newer) and the game with BepInEx installed. The project compiles against the installed `ModLibrary.dll` and `ModManager.dll`, so build and install those two first.

```bash
dotnet build -c Release
```

The project expects the game in `D:\Games\Steam\steamapps\common\Lawgivers II`. Pass `-p:GameDir="..."` if yours is somewhere else. The build copies the DLL into `BepInEx\plugins`, so close the game first.

To publish a release, bump the version in `Plugin.cs`, commit, and run `release.ps1`. It builds the DLL, tags the version, pushes the tag and creates a GitHub release with the DLL attached.

### Adding a cheat

Each cheat holds one number at a fixed value. To add one, put another entry into the list in `Cheats.Load`:

```csharp
new(config, "UnlimitedSomething", "SomethingBefore",
    "Unlimited something", "Your something stays at 100. Switching off gives back what you had.",
    lockedValue: 100,
    read: () => Instance.Player?.something,   // null while no game is loaded
    write: value => Instance.Player.something = (int)value,
    redraw: hud => hud.RefreshSomething()),
```

The panel picks it up on its own. The first two strings are the config keys for the switch and for the value from before, so don't rename them once people use the cheat.

## License

[MIT](LICENSE)
