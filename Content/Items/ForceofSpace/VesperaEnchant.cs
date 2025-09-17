using FargoSoulsSOTS.Core.SoulToggles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using SOTS.Items.Crushers;
using SOTS.Items.Earth.Glowmoth;
using SOTS.Items.Invidia;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.ForceofSpace
{
    public class VesperaEnchant : BaseEnchant
    {
        public override Color nameColor => new(79, 98, 113);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<VesperaEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<VesperaMask>()
                .AddIngredient<VesperaBreastplate>()
                .AddIngredient<VesperaLeggings>()
                .AddIngredient<VesperaNanDao>()
                .AddIngredient<IlluminantStaff>()
                .AddIngredient<MantisGrip>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    public class VesperaEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<VesperaEnchant>();
    }
}
