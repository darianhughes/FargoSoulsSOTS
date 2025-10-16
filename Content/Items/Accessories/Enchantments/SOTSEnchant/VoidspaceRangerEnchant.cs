using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SecretsOfTheSouls.Core.SoulToggles.SOTSToggles;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.Toggler;
using SOTS.Items.Celestial;
using SOTS.Buffs;
using SOTS.Void;
using SecretsOfTheSouls.Content.Items.Accessories.Forces.SOTSForce;

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class VoidspaceRangerEnchant : BaseEnchant
    {
        public override Color nameColor => new(107, 191, 113);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<VoidspaceEffect>(Item);
            player.AddEffect<AbyssalInfernoEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<VoidspaceMask>()
                .AddIngredient<VoidspaceBreastplate>()
                .AddIngredient<VoidspaceLeggings>()
                .AddIngredient<CataclysmSpheres>()
                .AddIngredient<VesperaEnchant>()
                .AddIngredient<DimensionShredder>()
                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class VoidspaceEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<VoidspaceRangerEnchant>();
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class AbyssalInfernoEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<VoidspaceRangerEnchant>();
        public override bool ExtraAttackEffect => true;

        public override void OnHitNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.HasBuff<VoidShock>())
            {
                target.AddBuff(ModContent.BuffType<AbyssalInferno>(), 60 * 3);
            }
            else if (item.CountsAsClass<VoidGeneric>())
            {
                target.AddBuff(ModContent.BuffType<AbyssalInferno>(), 60 * 3);
            }
        }

        public override void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (player.HasBuff<VoidShock>() || player.HasEffect<SpaceEffect>())
            {
                target.AddBuff(ModContent.BuffType<AbyssalInferno>(), 60 * 3);
            }
            else if (proj.CountsAsClass<VoidGeneric>())
            {
                target.AddBuff(ModContent.BuffType<AbyssalInferno>(), 60 * 3);
            }
        }
    }
}
