using FargoSoulsSOTS.Content.Items.Summons.SOTSCopy;
using Fargowiltas.Utilities;
using SOTS.Items;
using SOTS.Items.Celestial;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.Systems.Recipes
{
    public class ConversionRecipeSystem : ModSystem
    {
        public override void AddRecipes()
        {
            AddSummonConversions();
        }

        private static void AddSummonConversions()
        {
            RecipeHelper.CreateSimpleRecipe(ModContent.ItemType<ChaosLure>(), ModContent.ItemType<ElectromagneticLure>(), TileID.WorkBenches, conditions: Condition.Hardmode);
            RecipeHelper.CreateSimpleRecipe(ModContent.ItemType<CatalyzedCrystal>(), ModContent.ItemType<CatalystBomb>(), TileID.WorkBenches);
        }
    }
}
