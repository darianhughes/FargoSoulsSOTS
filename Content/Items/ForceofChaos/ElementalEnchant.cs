using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.Toggler;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.Items.Chaos;
using FargoSoulsSOTS.Core.SoulToggles;

namespace FargoSoulsSOTS.Content.Items.ForceofChaos
{
    public class ElementalEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Color nameColor => new(116, 122, 159);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(77, 3, 26);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<ElementalEffect>(Item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ElementalHelmet>()
                .AddIngredient<ElementalBreastplate>()
                .AddIngredient<ElementalLeggings>()
                .AddIngredient<HyperlightGeyser>()
                .AddIngredient<TwilightAssassinEnchant>()
                .AddIngredient<RoseBow>()
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
    public class ElementalEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaosForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<ElementalEnchant>();
    }
}