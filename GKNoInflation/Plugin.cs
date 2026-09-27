using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace GKNoInflation
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        public static ConfigEntry<bool> DisableBuyPriceInflation;
        public static ConfigEntry<bool> DisableSellPriceInflation;

        private void Awake()
        {
            Logger = base.Logger;

            DisableBuyPriceInflation = Config.Bind(
                "Patches",
                "Disable Buy Price Inflation",
                true,
                "Keep the buy price at base value instead of the inflated trader price when trading from the trader's inventory.");

            DisableSellPriceInflation = Config.Bind(
                "Patches",
                "Disable Sell Price Inflation",
                true,
                "Keep the sell price at base value instead of the inflated trader price when selling from the player's inventory.");

            Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, MyPluginInfo.PLUGIN_GUID);
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }

    [HarmonyPatch(typeof(Trading), nameof(Trading.GetSingleItemCostInTraderInventory), new Type[] { typeof(Item), typeof(int) })]
    static class TraderInventoryPatch
    {
        static void Postfix(ref float __result, Item item)
        {
            if (!Plugin.DisableBuyPriceInflation.Value)
                return;

            if (__result != 0f)
            {
                ItemDefinition definition = item.definition;
                __result = definition.base_price;
            }
        }
    }

    [HarmonyPatch(typeof(Trading), nameof(Trading.GetSingleItemCostInPlayerInventory), new Type[] { typeof(Item), typeof(int) })]
    static class PlayerInventoryPatch
    {
        static void Postfix(ref float __result, Item item)
        {
            if (!Plugin.DisableSellPriceInflation.Value)
                return;

            if (__result != 0f)
            {
                ItemDefinition definition = item.definition;
                __result = definition.base_price;
            }
        }
    }
}