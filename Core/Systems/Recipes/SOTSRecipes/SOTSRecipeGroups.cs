using SOTS.Items.Permafrost;
using SOTS.Items.Slime;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems.Recipes.SOTSRecipes
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSRecipeGroups : ModSystem
    {
        public override void AddRecipeGroups()
        {
            RecipeGroup FrigidChests = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Frigid Chests", ModContent.ItemType<FrigidRobe>(), ModContent.ItemType<ShatterShardChestplate>());
            RecipeGroup.RegisterGroup("SecretsOfTheSouls:FrigidChests", FrigidChests);

            RecipeGroup preHMWings = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Pre-Hardmode Wings", ItemID.CreativeWings, ModContent.ItemType<GelWings>());
            RecipeGroup.RegisterGroup("SecretsOfTheSouls:PreHMWings", preHMWings);
        }
    }
}