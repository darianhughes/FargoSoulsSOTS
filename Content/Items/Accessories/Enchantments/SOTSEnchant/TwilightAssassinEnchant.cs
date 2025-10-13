using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.Toggler;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.Items.Planetarium.FromChests;
using SOTS.Items.SpiritStaves;
using SOTS.Items;
using Fargowiltas.Content.Items.Tiles;
using SecretsOfTheSouls.Core.SoulToggles.SOTSToggles;

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class TwilightAssassinEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Color nameColor => new(0, 127, 135);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(25, 8, 30);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TwilightAssassinEffect>(Item);
            //player.AddEffect<HoloEyeMinionEffect>(Item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<TwilightAssassinsCirclet>()
                .AddIngredient<TwilightAssassinsChestplate>()
                .AddIngredient<TwilightAssassinsLeggings>()
                .AddIngredient<ChainedPlasma>()
                .AddIngredient<DigitalDaito>()
                .AddIngredient<OtherworldlySpiritStaff>()
                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class TwilightAssassinEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaosForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<TwilightAssassinEnchant>();
        public override bool ExtraAttackEffect => true;
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class HoloEyeMinionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaosForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<TwilightAssassinEnchant>();
        public override bool MinionEffect => true;
    }
}