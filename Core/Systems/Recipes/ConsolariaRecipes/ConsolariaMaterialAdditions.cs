using Consolaria.Content.Items.Accessories;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using Terraria;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems.Recipes.ConsolariaRecipes
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class ConsolariaMaterialAdditions : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];

                if (recipe.HasResult<SupersonicSoul>())
                {
                    recipe.AddIngredient<ValentineRing>();
                    recipe.AddIngredient<ShadowboundExoskeleton>();
                }
            }
        }
    }
}
