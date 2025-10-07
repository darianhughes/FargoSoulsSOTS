using SecretsOfTheSouls.Core.SoulToggles;
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

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class VesperaEnchant : BaseEnchant
    {
        public override string Texture => "SecretsOfTheSouls/Content/Items/Accessories/Enchantments/SOTSEnchant/VesperaEnchant";
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

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class VesperaEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<VesperaEnchant>();
    }
}
