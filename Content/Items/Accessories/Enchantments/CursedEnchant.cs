using SOTS.Items.AbandonedVillage;
using SOTS.Items;
using Terraria.ID;
using Terraria;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using FargoSoulsSOTS.Core.SoulToggles;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Toggler;

namespace FargoSoulsSOTS.Content.Items.Accessories.Enchantments
{
    public class CursedEnchant : BaseEnchant
    {
        public override Color nameColor => new(185, 173, 149);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<CursedEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EarthenHelmet>()
                .AddIngredient<EarthenChestplate>()
                .AddIngredient<EarthenLeggings>()
                .AddIngredient<Earthshaker>()
                .AddIngredient<ManicMiner>()
                .AddIngredient<MinersSword>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    public class CursedEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<CursedEnchant>();
    }
}
