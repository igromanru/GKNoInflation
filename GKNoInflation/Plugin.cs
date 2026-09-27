using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace GKNoInflation
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Harmony.CreateAndPatchAll(typeof(TradingPatches));
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }

    [HarmonyPatch(typeof(Trading), nameof(Trading.GetSingleItemCostInTraderInventory))]
    [HarmonyPatch(typeof(Trading), nameof(Trading.GetSingleItemCostInPlayerInventory))]
    static class TradingPatches
    {
        static void Postfix(ref float __result, Item item)
        {
            if (__result != 0f)
            {
                ItemDefinition definition = item.definition;
                __result = definition.base_price;
            }
        }
    }
}
