using System;
using System.Collections.Generic;
using System.Linq;
using FargoSoulsSOTS.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Materials;
using ssm.Content.Items.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.Systems.Recipes
{
    [ExtendsFromMod(FargoSOTSCrossmod.CommunitySoulsExpansion.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.CommunitySoulsExpansion.Name)]
    public class CSERecipeAdjustments : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargoSOTSConfig.Instance.UnfinishedContent;
        }

        public override void PostAddRecipes()
        {
            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];

                if (recipe.HasResult<MicroverseSoul>())
                {
                    int energyCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<AbomEnergy>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<AbomEnergy>());

                    if (FargoSOTSConfig.Instance.UnfinishedContent)
                    {
                        recipe.AddIngredient<ChaosForce>();
                        recipe.AddIngredient<SpaceForce>();
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
