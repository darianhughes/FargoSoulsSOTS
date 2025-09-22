using System.Linq;
using FargoSoulsSOTS.Content.Items.Accessories.Enchantments;
using FargoSoulsSOTS.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Materials;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.Systems.Recipes
{
    public class FargoRecipeAdjustments : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];

                if (recipe.HasResult<WorldShaperSoul>())
                {
                    recipe.AddIngredient<EarthenEnchant>();
                }

                if (recipe.HasResult<TerrariaSoul>())
                {
                    int energyCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<AbomEnergy>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<AbomEnergy>());

                    if (FargoSOTSConfig.Instance.UnfinishedContent)
                    {
                        if (!FargoSOTSCrossmod.CommunitySoulsExpansion.Loaded)
                        {
                            recipe.AddIngredient<ChaosForce>();
                            recipe.AddIngredient<SpaceForce>();
                        }
                    }
                    else
                    {
                        recipe.AddIngredient<VoidForce>();
                    }

                    recipe.AddIngredient<AbomEnergy>(energyCount);
                }
            }
        }
    }
}
