using System.Linq;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using SOTS.Items;
using SOTS.Items.Celestial;
using SOTS.Items.Chaos;
using SOTS.Items.Permafrost;
using SOTS.Items.Planetarium.FromChests;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems.Recipes.SOTSRecipes
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSRecipeAdjustments : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];

                if (recipe.HasResult<FlashsparkBoots>() && SOTSItemConfig.Instance.FlashsparkBootsRework)
                {
                    recipe.RemoveIngredient(ItemID.TerrasparkBoots);
                    int barCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<AbsoluteBar>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<AbsoluteBar>());

                    recipe.AddIngredient(ItemID.HellfireTreads);
                    recipe.AddIngredient<AbsoluteBar>(barCount);
                }

                if (recipe.HasResult<SubspaceBoosters>())
                {
                    recipe.RemoveIngredient(ItemID.Tabi);

                    int scaleCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<SanguiteBar>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<SanguiteBar>());

                    recipe.AddIngredient<AeolusBoots>();
                    recipe.AddIngredient<SanguiteBar>(scaleCount);
                }

                if (recipe.HasResult<FortressGenerator>() && SOTSItemConfig.Instance.FortressGeneratorRework)
                {
                    recipe.RemoveIngredient(ItemID.PaladinsShield);
                }

                if (recipe.HasResult<FrostArtifactHelmet>())
                {
                    recipe.RemoveIngredient(ItemID.FrostHelmet);
                    recipe.AddIngredient<FrigidCrown>();
                }

                if (recipe.HasResult<FrostArtifactChestplate>())
                {
                    recipe.RemoveIngredient(ItemID.FrostBreastplate);
                    recipe.AddRecipeGroup("SecretsOfTheSouls:FrigidChests");
                }

                if (recipe.HasResult<FrostArtifactTrousers>())
                {
                    recipe.RemoveIngredient(ItemID.FrostLeggings);
                    recipe.AddIngredient<FrigidGreaves>();
                }

                if (recipe.HasResult<ElementalHelmet>())
                {
                    int barCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<PhaseBar>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<PhaseBar>());
                    recipe.RemoveIngredient(ModContent.ItemType<TwilightAssassinsCirclet>());

                    recipe.AddIngredient<PhaseBar>(barCount + 10);
                }

                if (recipe.HasResult<ElementalBreastplate>())
                {
                    int barCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<PhaseBar>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<PhaseBar>());
                    recipe.RemoveIngredient(ModContent.ItemType<TwilightAssassinsChestplate>());

                    recipe.AddIngredient<PhaseBar>(barCount + 15);
                }

                if (recipe.HasResult<ElementalLeggings>())
                {
                    int barCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<PhaseBar>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<PhaseBar>());
                    recipe.RemoveIngredient(ModContent.ItemType<TwilightAssassinsLeggings>());

                    recipe.AddIngredient<PhaseBar>(barCount + 10);
                }
            }
        }
    }
}
