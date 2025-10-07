using System.Linq;
using SOTS.Items.Permafrost;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.Systems.Recipes.SOTSRecipes
{
    public class SOTSMiscRecipeAdjustments : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];

                if (FargoSOTSCrossmod.MagicStorage.Loaded)
                {
                    Mod magicStorage = FargoSOTSCrossmod.MagicStorage.Mod;

                    if (recipe.HasResult(magicStorage.Find<ModItem>("UpgradeHallowed")))
                    {
                        int frightSoulCount = recipe.requiredItem.Where(i => i.type == ItemID.SoulofFright).Sum(i => i.stack);
                        int plightSoulCount = frightSoulCount;
                        int gemCount = recipe.requiredItem.Where(i => i.type == ItemID.Sapphire).Sum(i => i.stack);
                        recipe.RemoveIngredient(ItemID.Sapphire);

                        recipe.AddIngredient<SoulOfPlight>(plightSoulCount);
                        recipe.AddIngredient(ItemID.Sapphire, gemCount);
                    }
                }
            }
        }
    }
}
