using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.Toggler;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.Items.Permafrost;
using Fargowiltas.Content.Items.Tiles;
using SecretsOfTheSouls.Core.SoulToggles.SOTSToggles;

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class FrostArtifactEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return SecretsOfTheSoulsConfig.Instance.UnfinishedContent;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Color nameColor => new(134, 114, 223);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(45, 42, 10);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<FrostArtifactEffect>(Item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FrostArtifactHelmet>()
                .AddIngredient<FrostArtifactChestplate>()
                .AddIngredient<FrostArtifactTrousers>()
                .AddIngredient<HypericeClusterCannon>()
                .AddIngredient<FrigidEnchant>()
                .AddIngredient<PBow>()
                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class FrostArtifactEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<FrostArtifactEnchant>();
    }
}