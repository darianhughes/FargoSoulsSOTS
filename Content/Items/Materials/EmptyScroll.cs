using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Materials
{
    public class EmptyScroll : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood)
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}
