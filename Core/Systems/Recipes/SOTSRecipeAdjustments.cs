using System.Linq;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using SOTS.Items;
using SOTS.Items.Permafrost;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.Systems.Recipes
{
    public class SOTSRecipeAdjustments : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];

                if (recipe.HasResult<FlashsparkBoots>() && ItemConfig.Instance.FlashsparkBootsRework)
                {
                    recipe.RemoveIngredient(ItemID.TerrasparkBoots);
                    int barCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<AbsoluteBar>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<AbsoluteBar>());

                    recipe.AddIngredient(ItemID.HellfireTreads);
                    recipe.AddIngredient<AbsoluteBar>(barCount);
                }
            }
        }
    }
}
