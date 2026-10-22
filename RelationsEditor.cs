using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using HarmonyLib;
using Lawgivers;
using Lawgivers.Interface;
using ModLibrary;
using UnityEngine;

namespace CheatMenu
{
    /// <summary>
    /// An "Edit relations" switch under the list in the party window's Relations tab.
    /// While it's on, the relation bars can be dragged to set the relation (-50 to 50, whole numbers).
    /// Switching it off locks the bars again and keeps the new relations.
    /// </summary>
    internal static class RelationsEditor
    {
        const string RowName = "Edit relations";
        const float Lowest = -50, Highest = 50; // PartyRelation's own limits

        static ConfigEntry<bool> editing;
        static bool modEnabled = true;
        static readonly Dictionary<IntPtr, Party> otherPartyOf = new(); // relation row -> the party it compares with

        public static bool IsOn => editing.Value;

        public static void Load(ConfigFile config)
        {
            editing = config.Bind("Cheats", "EditPartyRelations", false, "Lets you drag the bars in the party window's Relations tab.");
            GameEvents.AfterRefresh<UIPartyRelations>(Refresh);
        }

        public static void SetModEnabled(bool enabled)
        {
            modEnabled = enabled;
            if (!enabled) editing.Value = false;
        }

        [HarmonyPatch(typeof(UIPartyRelationsElement), nameof(UIPartyRelationsElement.Set))]
        static class RememberOtherParty
        {
            static void Postfix(UIPartyRelationsElement __instance, Party partyCompetition) =>
                otherPartyOf[__instance.Pointer] = partyCompetition;
        }

        static void Refresh(UIPartyRelations tab)
        {
            var row = GameUi.SwitchRowLike(tab.Directory, tab.Element, RowName, editing.Value, on =>
            {
                editing.Value = on;
                Refresh(tab);
                CheatMenu.UpdateIcon();
            }, textToMatch: tab.Element.Text);
            row.Root.gameObject.SetActive(modEnabled);

            var party = tab.Party;
            for (var i = 0; i < tab.Boxes.Count; i++)
            {
                var element = tab.Boxes[i];
                if (!otherPartyOf.TryGetValue(element.Pointer, out var other)) continue;
                GameUi.SetDraggable(element.Lever.Slider, editing.Value && modEnabled,
                    current: () => ToPosition(party.GetRelation(other).value),
                    changed: position =>
                    {
                        party.GetRelation(other).Set(Mathf.Round(Mathf.Lerp(Lowest, Highest, position)));
                        element.Set(party, other);
                    });
            }
        }

        static float ToPosition(float relation) => Mathf.InverseLerp(Lowest, Highest, relation);
    }
}
