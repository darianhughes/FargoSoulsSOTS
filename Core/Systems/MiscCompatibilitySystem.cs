using System.Linq;
using Fargowiltas;
using Fargowiltas.Common.Configs;
using Fargowiltas.Content.Items.Tiles;
using SecretsOfTheSouls.Common.ItemChanges;
using SecretsOfTheSouls.Content.Items.Accessories.Eternity.SOTSEternity;
using SOTS;
using Terraria;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems
{
    public class MiscCompatibilitySystem : ModSystem
    {
        public override void Load()
        {
            On_Player.ItemCheck_CheckCanUse += AllowUseSummons;
            On_Player.SummonItemCheck += AllowMultipleBosses;
        }

        public override void PostSetupContent()
        {
            Mod mutant = ModLoader.GetMod("Fargowiltas");

            if (SecretsOfTheSoulsCrossmod.SOTS.Loaded)
            {
                SOTSCompatbilityMethods.sotsAddMutantSupport(mutant);

                Mod sots = ModLoader.GetMod("SOTS");

                FargoSets.Items.InfoAccessory.SetValue(true, sots.Find<ModItem>("AnomalyLocator").Type);

                FargoSets.Items.DuplicatableItems.SetValue(FargoSets.Items.DupeType.Dupable, ModContent.ItemType<GadgetCoat>());

                FargoSets.Items.DuplicatableItems.SetValue(FargoSets.Items.DupeType.MaterialsDupable, sots.Find<ModItem>("SubspaceBoosters").Type);
                FargoSets.Items.DuplicatableItems.SetValue(FargoSets.Items.DupeType.MaterialsDupable, sots.Find<ModItem>("ChallengerRing").Type);

                FargoSets.Items.DuplicatableItems.SetValue(FargoSets.Items.DupeType.NotDupableFromDupable, sots.Find<ModItem>("SoulOfPlight").Type);
                FargoSets.Items.DuplicatableItems.SetValue(FargoSets.Items.DupeType.NotDupableFromDupable, sots.Find<ModItem>("SanguiteBar").Type);
                FargoSets.Items.DuplicatableItems.SetValue(FargoSets.Items.DupeType.NotDupableFromDupable, sots.Find<ModItem>("PrecariousCluster").Type);

                FargowiltasSouls.Content.Items.FargoGlobalItem.NoRuminateText.Add(ModContent.ItemType<GadgetCoat>());
            }

            EnchantedTreeTileEntity.SoulsMods.Add(Mod.Name);
            FargowiltasSouls.Content.Items.FargoGlobalItem.SoulsMods.Add(Mod.Name);
            }

        private bool AllowUseSummons(On_Player.orig_ItemCheck_CheckCanUse orig, Player self, Item item)
        {
            if (ModContent.GetInstance<FargoServerConfig>().EasySummons)
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    if (SOTSGlobalItem.ALwyasUsableVanillaSummons.Contains(item.type))
                        return true;
                }
                if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
                {

                }
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

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSCompatbilityMethods
    {
        public static bool DownedGlowmoth => SOTSWorld.downedGlowmoth;
        public static bool DownedPutrid => SOTSWorld.downedPinky;
        public static bool DownedPharoah => SOTSWorld.downedCurse;
        public static bool DownedExcavator => SOTSWorld.downedExcavator;
        public static bool DownedAdvisor => SOTSWorld.downedAdvisor;
        public static bool DownedPolaris => SOTSWorld.downedAmalgamation;
        public static bool DownedLux => SOTSWorld.downedLux;
        public static bool DownedSubspace => SOTSWorld.downedSubspace;

        public static void sotsAddMutantSupport(Mod mutant)
        {
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
        }
    }
}
