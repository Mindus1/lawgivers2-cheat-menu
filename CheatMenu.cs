using System;
using System.Collections.Generic;
using System.Linq;
using Lawgivers.Interface;
using ModLibrary;
using UnityEngine;

namespace CheatMenu
{
    /// <summary>The padlock button in the top-right corner and the panel it opens, one switch per cheat.</summary>
    public static class CheatMenu
    {
        const float PanelWidth = 760;

        static UIGame hud;
        static GameHud.Button button;
        static GameUi.Panel panel;
        static Sprite lockedIcon, unlockedIcon;
        static readonly Dictionary<Cheat, GameUi.SwitchRow> rows = new();

        public static void Install()
        {
            GameEvents.WhenHudReady(Build);
            GameEvents.HudUpdate += KeepCheatsApplied;
        }

        /// <summary>The Mod Manager's switch for this mod. Off means: as if the mod was never there.</summary>
        internal static void SetModEnabled(bool enabled)
        {
            if (!enabled)
            {
                foreach (var cheat in Cheats.All) cheat.Switch(hud, false);
                panel?.Root.gameObject.SetActive(false);
            }
            RelationsEditor.SetModEnabled(enabled);
            button?.SetVisible(enabled);
            ShowCheatStates();
        }

        static void Build(UIGame gameHud)
        {
            hud = gameHud;
            lockedIcon = GameUi.FindSprite("Lock");
            unlockedIcon = GameUi.FindSprite("Unlock");

            button = GameHud.AddButton("Cheats", lockedIcon, () => "Cheats", TogglePanel);
            panel = GameHud.AddPanel("Cheats", PanelWidth);
            foreach (var cheat in Cheats.All)
            {
                rows[cheat] = GameUi.CreateSwitchRow(panel.Body, cheat.Label, cheat.Description, cheat.IsOn, on =>
                {
                    cheat.Switch(hud, on);
                    UpdateIcon();
                });
            }
            UpdateIcon();
        }

        static void TogglePanel() => panel.Root.gameObject.SetActive(!panel.Root.gameObject.activeSelf);

        static void KeepCheatsApplied(UIGame gameHud)
        {
            foreach (var cheat in Cheats.All.Where(cheat => cheat.IsOn))
            {
                try { cheat.KeepApplied(gameHud); }
                catch (Exception e)
                {
                    // Failing every frame would flood the log.
                    Plugin.Log.LogError($"{cheat.Label} stopped working and was switched off: {e}");
                    cheat.TurnOffAfterError();
                    ShowCheatStates();
                }
            }
        }

        static void ShowCheatStates()
        {
            foreach (var (cheat, row) in rows) row.Show(cheat.IsOn);
            UpdateIcon();
        }

        // The padlock is open while any cheat is on.
        internal static void UpdateIcon()
        {
            if (button == null || lockedIcon == null || unlockedIcon == null) return;
            button.Icon.sprite = Cheats.All.Any(cheat => cheat.IsOn) || RelationsEditor.IsOn ? unlockedIcon : lockedIcon;
        }
    }
}
