using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using SOTS.Items.Pyramid;
using SOTS.Items.Nature;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SecretsOfTheSouls.Core.SoulToggles;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Toggler;

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class PatchLeatherEnchant : BaseEnchant
    {
        public override string Texture => "SecretsOfTheSouls/Content/Items/Accessories/Enchantments/SOTSEnchant/PatchLeatherEnchant";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override Color nameColor => new(164, 96, 64);
        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Orange;
            Item.value = 50000;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<PatchLeatherEffect>(Item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PatchLeatherHat>()
                .AddIngredient<PatchLeatherTunic>()
                .AddIngredient<PatchLeatherPants>()
                .AddIngredient<SnakeBow>()
                .AddIngredient<BiomassBlast>()
                .AddIngredient<AcornBag>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class PatchLeatherEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ChaosForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<PatchLeatherEnchant>();
        public override bool ExtraAttackEffect => true;
    }
}
