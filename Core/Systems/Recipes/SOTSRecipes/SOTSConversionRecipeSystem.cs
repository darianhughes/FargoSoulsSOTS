using SecretsOfTheSouls.Content.Items.Summons.SOTSCopy;
using Fargowiltas.Utilities;
using SOTS.Items;
using SOTS.Items.Celestial;
using SOTS.Items.Earth.Glowmoth;
using SOTS.Items.Permafrost;
using SOTS.Items.Slime;
using SOTS.Items.Tools;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems.Recipes.SOTSRecipes
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSConversionRecipeSystem : ModSystem
    {
        public override void AddRecipes()
        {
            AddSummonConversions();
        }

        private static void AddSummonConversions()
        {
            RecipeHelper.CreateSimpleRecipe(ModContent.ItemType<OffbrandPeanuts>(), ModContent.ItemType<JarOfPeanuts>(), TileID.WorkBenches);
            RecipeHelper.CreateSimpleRecipe(ModContent.ItemType<OldCRTTV>(), ModContent.ItemType<WorldgenScanner>(), TileID.WorkBenches);
            RecipeHelper.CreateSimpleRecipe(ModContent.ItemType<PolarKey>(), ModContent.ItemType<FrostedKey>(), TileID.WorkBenches);
            RecipeHelper.CreateSimpleRecipe(ModContent.ItemType<ChaosLure>(), ModContent.ItemType<ElectromagneticLure>(), TileID.WorkBenches, conditions: Condition.Hardmode);
            RecipeHelper.CreateSimpleRecipe(ModContent.ItemType<CatalystDynamite>(), ModContent.ItemType<CatalystBomb>(), TileID.WorkBenches);
        }
    }
}
