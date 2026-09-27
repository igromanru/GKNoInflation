using System;
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
            Logger = base.Logger;
            Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, MyPluginInfo.PLUGIN_GUID);
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }

    [HarmonyPatch(typeof(Trading), nameof(Trading.GetSingleItemCostInTraderInventory), new Type[] { typeof(Item), typeof(int) })]
    [HarmonyPatch(typeof(Trading), nameof(Trading.GetSingleItemCostInPlayerInventory), new Type[] { typeof(Item), typeof(int) })]
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