using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fargowiltas;
using Fargowiltas.Common.Configs;
using Fargowiltas.Content.Items.CaughtNPCs;
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
            if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
                Add("McMoneyPants", SecretsOfTheSoulsCrossmod.Consolaria.Mod.Find<ModNPC>("McMoneypants").Type);
            if (SecretsOfTheSoulsCrossmod.Heartbeataria.Loaded)
                Add("StarMerchant", SecretsOfTheSoulsCrossmod.Heartbeataria.Mod.Find<ModNPC>("StarMerchantNPC").Type);

            On_Player.ItemCheck_CheckCanUse += AllowUseSummons;
            On_Player.ItemCheck_UseBossSpawners += AllowUseSummons2EvilEdition;
            //On_Player.SummonItemCheck += AllowMultipleBosses;
        }

        public static void Add(string internalName, int id)
        {
            if (SecretsOfTheSouls.Instance == null)
            {
                SecretsOfTheSouls.Instance = ModContent.GetInstance<SecretsOfTheSouls>();
            }
            CaughtNPCItem item = new(internalName, id);
            SecretsOfTheSouls.Instance.AddContent(item);
            FieldInfo info = typeof(CaughtNPCItem).GetField("CaughtTownies", LumUtils.UniversalBindingFlags);
            Dictionary<int, int> list = (Dictionary<int, int>)info.GetValue(info);
            list.Add(id, item.Type);
            info.SetValue(info, list);
        }

        public override void Unload()
        {
            On_Player.ItemCheck_CheckCanUse -= AllowUseSummons;
            On_Player.ItemCheck_UseBossSpawners -= AllowUseSummons2EvilEdition;
            //On_Player.SummonItemCheck -= AllowMultipleBosses;
        }

        private static readonly Mod mutant = ModLoader.GetMod("Fargowiltas");

        public override void PostSetupContent()
        {
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

            Fargowiltas.Fargowiltas.SoulsMods.Add(Mod.Name);
        }

        private bool AllowUseSummons(On_Player.orig_ItemCheck_CheckCanUse orig, Player self, Item item)
        {
            if (ModContent.GetInstance<FargoServerConfig>().EasySummons)
            {
                if (ModLoader.HasMod("SOTS"))
                {
                    if (SOTSGlobalItem.AlwyasUsableSOTSSummons.Contains(item.type))
                        return true;
                }
                if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
                {
                    if (ConsolariaGlobalItem.AlwyasUsableConsolariaSummons.Contains(item.type))
                        return true;
                }
            }
            return orig(self, item);
        }

        /*
        private bool AllowMultipleBosses(On_Player.orig_SummonItemCheck orig, Player self, Item item)
        {
            if (ModContent.GetInstance<FargoServerConfig>().EasySummons && self.itemAnimation == self.itemAnimationMax)
            {
                return true;
            }
            return orig(self, item);
        }
        */

        private void AllowUseSummons2EvilEdition(On_Player.orig_ItemCheck_UseBossSpawners orig, Player self, int onWhichPlayer, Item item)
        {
            if (!ModContent.GetInstance<FargoServerConfig>().EasySummons)
            {
                orig(self, onWhichPlayer, item);
                return;
            }
            bool day = Main.dayTime;
            if (self.ItemTimeIsZero && self.itemAnimation > 0)
            {
                if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
                {
                    if (ConsolariaGlobalItem.NightSettingConsolariaSummons.Contains(item.type))
                        Main.dayTime = false;
                }
            }
            orig(self, onWhichPlayer, item);
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
