using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using Lawgivers.Interface;
using ModLibrary;

namespace CheatMenu
{
    internal static class Cheats
    {
        /// <summary>In the order the panel shows them.</summary>
        public static List<Cheat> All { get; private set; }

        public static void Load(ConfigFile config)
        {
            All = new List<Cheat>
            {
                new(config, "UnlimitedActionPoints", "ActionPointsBefore",
                    "Unlimited action points", "Your action points stay at 9. Switching off gives back what you had.",
                    lockedValue: 9,
                    read: () => GameEvents.Player?.actions?.points,
                    write: value => GameEvents.Player.actions.points = (int)value,
                    redraw: hud => hud.RefreshPartyActionPoints()),

                new(config, "UnlimitedMoney", "MoneyBefore",
                    "Unlimited money", "Your party's money stays at 1T. Switching off gives back what you had.",
                    lockedValue: 1_000_000_000_000,
                    read: () => GameEvents.Player?.Party?.money?.value,
                    write: value => GameEvents.Player.Party.money.value = value,
                    redraw: hud => hud.RefreshPartyMoney()),

                new(config, "UnlimitedBlackMoney", "BlackMoneyBefore",
                    "Unlimited black money", "Your party's black money stays at 1T. Switching off gives back what you had.",
                    lockedValue: 1_000_000_000_000,
                    read: () => GameEvents.Player?.Party?.black?.value,
                    write: value => GameEvents.Player.Party.black.value = value,
                    redraw: hud => hud.RefreshPartyBlack()),
            };
        }
    }

    /// <summary>
    /// Holds one of the player's numbers at a fixed value. Switching on remembers the number,
    /// switching off puts it back. Both the switch and the remembered number are stored in the
    /// config, so a restart in between changes nothing.
    /// </summary>
    internal sealed class Cheat
    {
        public readonly string Label;
        public readonly string Description;

        readonly long lockedValue;
        readonly Func<long?> read; // null while no game is loaded
        readonly Action<long> write;
        readonly Action<UIGame> redraw;
        readonly ConfigEntry<bool> isOn;
        readonly ConfigEntry<long> valueBefore;

        public Cheat(ConfigFile config, string key, string valueBeforeKey, string label, string description,
            long lockedValue, Func<long?> read, Action<long> write, Action<UIGame> redraw)
        {
            Label = label;
            Description = description;
            this.lockedValue = lockedValue;
            this.read = read;
            this.write = write;
            this.redraw = redraw;
            isOn = config.Bind("Cheats", key, false, description);
            valueBefore = config.Bind("Cheats", valueBeforeKey, 0L, $"The value before '{label}' was switched on.");
        }

        public bool IsOn => isOn.Value;

        public void Switch(UIGame hud, bool on)
        {
            if (on == IsOn) return;

            if (read() is { } current)
            {
                if (on) valueBefore.Value = current;
                else Set(hud, valueBefore.Value);
            }
            isOn.Value = on;

            if (on) KeepApplied(hud);
        }

        /// <summary>Runs every frame while the cheat is on, which catches every way the game changes the number.</summary>
        public void KeepApplied(UIGame hud)
        {
            if (read() is { } current && current != lockedValue) Set(hud, lockedValue);
        }

        /// <summary>For a cheat that keeps failing: off, without touching the game.</summary>
        public void TurnOffAfterError() => isOn.Value = false;

        void Set(UIGame hud, long value)
        {
            write(value);
            redraw(hud);
        }
    }
}
