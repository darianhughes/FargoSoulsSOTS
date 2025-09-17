using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.Toggler;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargoSoulsSOTS.Core.SoulToggles;
using SOTS.Items.Permafrost;

namespace FargoSoulsSOTS.Content.Items.ForceofSpace
{
    public class FrigidEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Color nameColor => new(187, 199, 255);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(6, 19, 64);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<FrigidEffect>(Item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()

                .AddIngredient<FrigidCrown>()
                .AddRecipeGroup("FargoSoulsSOTS:FrigidChests")
                .AddIngredient<FrigidGreaves>()
                .AddIngredient<ShardStaff>()
                .AddIngredient<ShatterBlade>()
                .AddIngredient<FrigidJavelin>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
        public class FrigidEffect : AccessoryEffect
        {
            public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
            public override int ToggleItemType => ModContent.ItemType<FrigidEnchant>();
        }
    }
}