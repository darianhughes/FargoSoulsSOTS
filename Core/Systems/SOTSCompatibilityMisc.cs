using SOTS;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.Systems
{
    public class SOTSCompatibilityMisc : ModSystem
    {
        public static bool DownedLux => SOTSWorld.downedLux;
        public static bool DownedSubspace => SOTSWorld.downedSubspace;

        public override void PostSetupContent()
        {
            Mod mutant = ModLoader.GetMod("Fargowiltas");
            mutant.Call("AddSummon", 16.5f, "FargoSoulsSOTS", "ChaosLure",
                () => DownedLux, Item.buyPrice(gold: 60));
            mutant.Call("AddSummon", 17.9f, "FargoSoulsSOTS", "CatalyzedCrystal",
                () => DownedSubspace, Item.buyPrice(gold: 85));
        }
    }
}
