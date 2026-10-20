using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using ModLibrary;
using ModManager;

namespace CheatMenu
{
    [BepInPlugin(Guid, "Cheat Menu", "1.3.0")]
    [BepInDependency(ModLibrary.Plugin.Guid)]
    [BepInDependency(ModManager.Plugin.Guid)]
    [ModDescription("Adds a Cheats button to the top-right corner of the game.")]
    public class Plugin : BasePlugin
    {
        public const string Guid = "mindus.lawgivers2.cheatmenu";

        internal static new ManualLogSource Log;

        public override void Load()
        {
            Log = base.Log;
            Cheats.Load(Config);
            CheatMenu.Install();
            Mods.Register(this, CheatMenu.SetModEnabled);
        }
    }
}
