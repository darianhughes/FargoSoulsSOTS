using System.Linq;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using SOTS.Items;
using SOTS.Items.Celestial;
using SOTS.Items.Permafrost;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.Systems.Recipes.SOTSRecipes
{
    public class SOTSRecipeAdjustments : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];

                if (recipe.HasResult<FlashsparkBoots>() && SOTSItemConfig.Instance.FlashsparkBootsRework)
                {
                    recipe.RemoveIngredient(ItemID.TerrasparkBoots);
                    int barCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<AbsoluteBar>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<AbsoluteBar>());

                    recipe.AddIngredient(ItemID.HellfireTreads);
                    recipe.AddIngredient<AbsoluteBar>(barCount);
                }

                if (recipe.HasResult<SubspaceBoosters>())
                {
                    recipe.RemoveIngredient(ItemID.Tabi);

                    int scaleCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<SanguiteBar>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<SanguiteBar>());

                    recipe.AddIngredient<AeolusBoots>();
                    recipe.AddIngredient<SanguiteBar>(scaleCount);
                }
            }
        }
    }
}
