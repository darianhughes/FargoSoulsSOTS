using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core.Toggler;
using static FargoSoulsSOTS.Content.Items.Accessories.Enchantments.FrostArtifactEnchant;
using FargoSoulsSOTS.Content.Items.Accessories.Enchantments;

namespace FargoSoulsSOTS.Content.Items.Accessories.Forces
{
    public class SpaceForce : BaseForce
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Enchants[Type] =
            [
                ModContent.ItemType<FrostArtifactEnchant>(),
                ModContent.ItemType<VibrantEnchant>(),
            ];
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);
            player.AddEffect<FrostArtifactEffect>(Item);
            player.AddEffect<VibrantEffect>(Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int enchant in Enchants[Type])
            {
                recipe.AddIngredient(enchant);
            }
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }
    public class SpaceEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<SpaceForce>();
    }
}