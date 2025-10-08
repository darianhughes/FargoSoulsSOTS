using System.Linq;
using Fargowiltas.Common.Configs;
using Fargowiltas.Content.Items;
using Fargowiltas.Content.Items.Tiles;
using SecretsOfTheSouls.Common.ItemChanges;
using SOTS;
using SOTS.Items.Earth.Glowmoth;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
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

        public override void Load()
        {
            On_Player.ItemCheck_CheckCanUse += AllowUseSummons;
            On_Player.SummonItemCheck += AllowMultipleBosses;
        }

        public override void PostSetupContent()
        {
            Mod mutant = ModLoader.GetMod("Fargowiltas");
            mutant.Call("AddSummon", 2.1f, "FargoSoulsSOTS", "SuspiciousLookingCandle",
                () => DownedGlowmoth, Item.buyPrice(gold: 9));
            mutant.Call("AddSummon", 4.25f, "FargoSoulsSOTS", "OffbrandPeanuts",
                () => DownedPutrid, Item.buyPrice(gold: 13));
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

            EnchantedTreeTileEntity.DupableMaterialsModded.Add(("SOTS", "SubspaceBoosters"));
            EnchantedTreeTileEntity.DupableMaterialsModded.Add(("SOTS", "ChallengerRing"));
            EnchantedTreeTileEntity.DontDupeModded.Add(("SOTS", "SanguiteBar"));
            EnchantedTreeTileEntity.DontDupeModded.Add(("SOTS", "PrecariousCluster"));
            EnchantedTreeTileEntity.SoulsMods.Add(Mod.Name);
        }

        private bool AllowUseSummons(On_Player.orig_ItemCheck_CheckCanUse orig, Player self, Item item)
        {
            if (SOTSGlobalItem.ALwyasUsableVanillaSummons.Contains(item.type) && ModContent.GetInstance<FargoServerConfig>().EasySummons)
            {
                    return true;
            }
            return orig(self, item);
        }

        private bool AllowMultipleBosses(On_Player.orig_SummonItemCheck orig, Player self, Item item)
        {
            if (ModContent.GetInstance<FargoServerConfig>().EasySummons)
            {
                return true;
            }
            return orig(self, item);
        }
    }
}
