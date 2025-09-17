using SOTS.Items.Permafrost;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Common
{
    public class FargoSOTSGlobalRecipe : ModSystem
    {
        public override void AddRecipeGroups()
        {
            RecipeGroup FrigidChests = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Frigid Chests", ModContent.ItemType<FrigidRobe>(), ModContent.ItemType<ShatterShardChestplate>());
            RecipeGroup.RegisterGroup("FargoSoulsSOTS:FrigidChests", FrigidChests);
        }

    }
}