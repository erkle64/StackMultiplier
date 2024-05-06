using C3.ModKit;
using HarmonyLib;
using Unfoundry;
using UnityEngine;

namespace StackMultiplier
{
    [UnfoundryMod(GUID)]
    public class Plugin : UnfoundryPlugin
    {
        public const string
            MODNAME = "StackMultiplier",
            AUTHOR = "erkle64",
            GUID = AUTHOR + "." + MODNAME,
            VERSION = "0.1.1";

        public static LogSource log;

        public static TypedConfigEntry<float> stackMultiplier;

        public Plugin()
        {
            log = new LogSource(MODNAME);

            new Config(GUID)
                .Group("Multipliers")
                    .Entry(out stackMultiplier, "stackMultiplier", 2.0f, true, "Multiplier for stack size")
                .EndGroup()
                .Load()
                .Save();
        }

        public override void Load(Mod mod)
        {
            log.Log($"Loading {MODNAME}");
        }

        public const ItemTemplate.ItemTemplateFlags exclusionFlags =
            ItemTemplate.ItemTemplateFlags.MINING_TOOL |
            ItemTemplate.ItemTemplateFlags.TRAIN_VEHICLE |
            ItemTemplate.ItemTemplateFlags.RAIL_MINER |
            ItemTemplate.ItemTemplateFlags.AL_STARTER |
            ItemTemplate.ItemTemplateFlags.EMOTE |
            ItemTemplate.ItemTemplateFlags.CONSTRUCTION_MATERIAL |
            ItemTemplate.ItemTemplateFlags.CONSTRUCTION_RUBBLE |
            ItemTemplate.ItemTemplateFlags.SALES_CURRENCY |
            ItemTemplate.ItemTemplateFlags.SALES_ITEM |
            ItemTemplate.ItemTemplateFlags.SALES_ITEM_ASSEMBLY_LINE;


        [HarmonyPatch]
        public static class Patch
        {
            [HarmonyPatch(typeof(ItemTemplate), nameof(ItemTemplate.onLoad))]
            [HarmonyPostfix]
            public static void ItemTemplateOnLoad(ItemTemplate __instance)
            {
                var stackMultiplier = Plugin.stackMultiplier.Get();
                if (stackMultiplier > 0.0f && stackMultiplier != 1.0f
                    && (__instance.flags & exclusionFlags) == 0
                    && __instance.stackSize > 1)
                {
                    __instance.stackSize = (uint)Mathf.CeilToInt(__instance.stackSize * stackMultiplier);
                }
            }
        }
    }
}
