using System.Linq;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Materials;
using SecretsOfTheSouls.Content.Items.Accessories.Souls.ConsolariaSoul;
using SecretsOfTheSouls.Content.Items.Accessories.Souls.SOTSSoul;
using Terraria;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems.Recipes
{
    public class SoulsRecipeAdjustments : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];

                if (recipe.HasResult<MasochistSoul>() && !(SecretsOfTheSoulsCrossmod.Consolaria.Loaded))
                {
                    int deviEnergyCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<DeviatingEnergy>()).Sum(i => i.stack);
                    int abomEnergyCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<AbomEnergy>()).Sum(i => i.stack);

                    recipe.RemoveIngredient(ModContent.ItemType<DeviatingEnergy>());
                    recipe.RemoveIngredient(ModContent.ItemType<AbomEnergy>());

                    recipe.AddIngredient(SecretsOfTheSoulsCrossmod.Heartbeataria.Mod.Find<ModItem>("OtherworldCore").Type);
                    recipe.AddIngredient<DeviatingEnergy>(deviEnergyCount);
                    recipe.AddIngredient<AbomEnergy>(abomEnergyCount);
                }

                if (recipe.HasResult<EternitySoul>())
                {
                    int energyCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<EternalEnergy>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<EternalEnergy>());

                    if (SecretsOfTheSoulsConfig.Instance.UnfinishedContent)
                    {
                        if (SecretsOfTheSoulsCrossmod.SOTS.Loaded)
                            recipe.AddIngredient<SubspaceVoyagerSoul>();
                        if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
                            recipe.AddIngredient<ForgottenSoul>();
                    }

                    recipe.AddIngredient<EternalEnergy>(energyCount);
                }
            }
        }
    }
}
