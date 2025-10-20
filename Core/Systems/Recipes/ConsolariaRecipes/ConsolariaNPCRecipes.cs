using Consolaria.Content.Items.Consumables;
using Fargowiltas.Content.Items.Tiles;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace SecretsOfTheSouls.Core.Systems.Recipes.ConsolariaRecipes
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class ConsolariaNPCRecipes : ModSystem
    {
        private static readonly Mod mutant = ModLoader.GetMod("Fargowiltas");

        public override void AddRecipes()
        {
            int mcMoneyPants = Mod.Find<ModItem>("McMoneyPants").Type;

            Recipe.Create(mcMoneyPants)
                .AddIngredient(mutant.Find<ModItem>("TravellingMerchant").Type)
                .AddIngredient(ItemID.GoldDust, 100)
                .AddIngredient<McMoneypantsInvitation>()
                .AddTile<GoldenDippingVatSheet>()
                .DisableDecraft()
                .Register();
        }
    }
}
