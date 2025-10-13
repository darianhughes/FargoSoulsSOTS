using SOTS.Items.Evil;
using SOTS.Items.Inferno;
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
        public static string AnyItem(int id) => $"{Lang.misc[37]} {Lang.GetItemName(id)}";

        public static string AnyItem(string fargoSoulsLocalizationKey) => $"{Lang.misc[37]} {Language.GetTextValue($"Mods.FargowiltasSouls.RecipeGroups.{fargoSoulsLocalizationKey}")}";
        public override void AddRecipeGroups()
        {
            RecipeGroup FrigidChests = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Frigid Chests", ModContent.ItemType<FrigidRobe>(), ModContent.ItemType<ShatterShardChestplate>());
            RecipeGroup.RegisterGroup("SecretsOfTheSouls:FrigidChests", FrigidChests);

            RecipeGroup preHMWings = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Pre-Hardmode Wings", ItemID.CreativeWings, ModContent.ItemType<GelWings>());
            RecipeGroup.RegisterGroup("SecretsOfTheSouls:PreHMWings", preHMWings);

            RecipeGroup group;

            group = new RecipeGroup(() => AnyItem(ItemID.SharkToothNecklace), ItemID.SharkToothNecklace, ItemID.StingerNecklace, ModContent.ItemType<MidnightPrism>());
            RecipeGroup.RegisterGroup("SecretsOfTheSouls:AnySharktoothNecklace", group);

            group = new RecipeGroup(() => AnyItem(ItemID.MagicQuiver), ItemID.MagicQuiver, ItemID.MoltenQuiver, ItemID.StalkersQuiver, ModContent.ItemType<BlazingQuiver>());
            RecipeGroup.RegisterGroup("SecretsOfTheSouls:AnyQuiver", group);
        }
    }
}