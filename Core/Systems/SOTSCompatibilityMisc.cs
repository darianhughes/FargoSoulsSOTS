using SOTS;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.Systems
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class SOTSCompatibilityMisc : ModSystem
    {
        public static bool DownedGlowmoth => SOTSWorld.downedGlowmoth;
        public static bool DownedPutrid => SOTSWorld.downedPinky;
        public static bool DownedPharoah => SOTSWorld.downedCurse;
        public static bool DownedExcavator => SOTSWorld.downedExcavator;
        public static bool DownedAdvisor => SOTSWorld.downedAdvisor;
        public static bool DownedPolaris => SOTSWorld.downedAmalgamation;
        public static bool DownedLux => SOTSWorld.downedLux;
        public static bool DownedSubspace => SOTSWorld.downedSubspace;

        public override void PostSetupContent()
        {
            Mod mutant = ModLoader.GetMod("Fargowiltas");
            mutant.Call("AddSummon", 2.1f, "FargoSoulsSOTS", "GlowingNylonCandle",
                () => DownedGlowmoth, Item.buyPrice(gold: 9));
            mutant.Call("AddSummon", 4.25f, "FargoSoulsSOTS", "OffbrandPeanuts",
                () => DownedPutrid, Item.buyPrice(gold: 13));
            mutant.Call("AddSummon", 4.5f, "FargoSoulsSOTS", "CursedSarcophagus",
                () => DownedPharoah, Item.buyPrice(gold: 13));
            mutant.Call("AddSummon", 6.8f, "FargoSoulsSOTS", "ExcavationRemote",
                () => DownedExcavator, Item.buyPrice(gold: 17));
            mutant.Call("AddSummon", 6.9f, "FargoSoulsSOTS", "OldCRTTV",
                () => DownedAdvisor, Item.buyPrice(gold: 17));
            mutant.Call("AddSummon", 11.01f, "FargoSoulsSOTS", "PolarKey",
                () => DownedPolaris, Item.buyPrice(gold: 43));
            mutant.Call("AddSummon", 16.5f, "FargoSoulsSOTS", "ChaosLure",
                () => DownedLux, Item.buyPrice(gold: 60));
            mutant.Call("AddSummon", 17.9f, "FargoSoulsSOTS", "CatalyzedCrystal",
                () => DownedSubspace, Item.buyPrice(gold: 85));
        }
    }
}
