using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.ForceofVoid
{
    public class VoidForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                 ModContent.ItemType<VesperaEnchant>(),
                 ModContent.ItemType<VibrantEnchant>(),
            ];
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<VesperaEffect>(Item);
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

    public class VoidEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<VoidForce>();
    }
}
