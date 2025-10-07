using System.Linq;
using SOTS.Items.Permafrost;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems.Recipes.SOTSRecipes
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSMiscRecipeAdjustments : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];

                if (SecretsOfTheSoulsCrossmod.MagicStorage.Loaded)
                {
                    Mod magicStorage = SecretsOfTheSoulsCrossmod.MagicStorage.Mod;

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
