using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Consolaria.Content.Items.Armor.Melee;
using Consolaria.Content.Items.Weapons.Melee;
using Consolaria.Content.Items.Pets;
using Consolaria.Content.Items.Consumables;
using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SecretsOfTheSouls.Core.SoulToggles.ConsolariaToggles;
using FargowiltasSouls.Core.Toggler;

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.ConsolariaEnchant
{
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class AncientDragonEnchant : BaseEnchant
    {
        public override Color nameColor => new(132, 122, 224);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Lime;
            Item.value = 250000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<AncientDragonEffect>(Item);
        }

        public override int DamageTooltip(out DamageClass damageClass, out Color? tooltipColor, out int? scaling)
        {
            damageClass = DamageClass.Generic;
            tooltipColor = null;
            scaling = null;
            return (int)(90 * Main.LocalPlayer.ActualClassDamage(DamageClass.Generic));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AncientDragonMask>()
                .AddIngredient<AncientDragonBreastplate>()
                .AddIngredient<AncientDragonGreaves>()
                .AddIngredient<GreatDrumstick>()
                .AddIngredient<GoldenLantern>()
                .AddIngredient<Wiesnbrau>(30)
                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }

    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class AncientDragonEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<MightForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<AncientDragonEnchant>();

        private int TrailSpawnEveryTicks = 2;
        private int trailTicker;

        public override void PostUpdate(Player player)
        {
            base.PostUpdate(player);
        }
    }
}
