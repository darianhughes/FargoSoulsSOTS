using SOTS.Items.Permafrost;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems.Recipes.SOTSRecipes
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class FargoSOTSRecipeGroups : ModSystem
    {
        public override void AddRecipeGroups()
        {
            RecipeGroup FrigidChests = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Frigid Chests", ModContent.ItemType<FrigidRobe>(), ModContent.ItemType<ShatterShardChestplate>());
            RecipeGroup.RegisterGroup("SecretsOfTheSouls:FrigidChests", FrigidChests);
        }

    }
}