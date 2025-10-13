using System.Collections.Generic;
using FargowiltasSouls;
using FargowiltasSouls.Content.Buffs.Eternity;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs;
using SecretsOfTheSouls.Content.Buffs.Emode;
using SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant;
using SecretsOfTheSouls.Content.Items.Accessories.Eternity.SOTSEternity;
using SecretsOfTheSouls.Content.Items.Accessories.Forces.SOTSForce;
using SecretsOfTheSouls.Core.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Materials;
using Fargowiltas.Content.Items.Tiles;

namespace SecretsOfTheSouls.Content.Items.Accessories.Souls.SOTSSoul
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [AutoloadEquip(EquipType.Face)]
    public class SubspaceVoyagerSoul : BaseSoul
    {
        public override List<AccessoryEffect> ActiveSkillTooltips =>
        [
            AccessoryEffectLoader.GetEffect<BloomStrike>()
        ];

        public static List<int> Forces =
        [
            ModContent.ItemType<ChaosForce>(),
            ModContent.ItemType<SpaceForce>(),
        ];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 5000000;
            //Item.rare = ItemRarityID.Expert;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            if (!SecretsOfTheSoulsCrossmod.CommunitySoulsExpansion.Loaded)
            {
                foreach (int force in Forces)
                    modPlayer.ForceEffects.Add(force);


                ModContent.GetInstance<ChaosForce>().UpdateAccessory(player, hideVisual);
                ModContent.GetInstance<SpaceForce>().UpdateAccessory(player, hideVisual);
            }

            //Jelly Jumpers
            player.buffImmune[ModContent.BuffType<Corrosion>()] = true;

            player.extraFall += 10;
            player.autoJump = true;

            player.AddEffect<JellyJumpersEffect>(Item);

            //Drill Cap
            player.buffImmune[ModContent.BuffType<Grounded>()] = true;
            player.buffImmune[ModContent.BuffType<LowGroundBuff>()] = true;

            player.AddEffect<DrillCapEffect>(Item);

            //Plasma Grasp
            player.buffImmune[BuffID.Electrified] = true;

            player.AddEffect<PlasmaHook>(Item);

            //Cooled Fidget Spiner

            //Gadget Coat
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();
            mp.GadgetCoat = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            if (!SecretsOfTheSoulsCrossmod.CommunitySoulsExpansion.Loaded)
            {
                foreach (int force in Forces)
                    recipe.AddIngredient(force);
            }

            recipe.AddIngredient<GadgetCoat>();
            recipe.AddIngredient<AbomEnergy>(10);
            recipe.AddTile<CrucibleCosmosSheet>();
            recipe.Register();
        }
    }
}