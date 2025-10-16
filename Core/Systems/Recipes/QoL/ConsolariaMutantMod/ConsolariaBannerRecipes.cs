using Consolaria.Content.Items.Placeable.Banners;
using Consolaria.Content.Items.Weapons.Melee;
using Fargowiltas.Common.Configs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems.Recipes.QoL.ConsolariaMutantMod
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class ConsolariaBannerRecipes : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargoServerConfig.Instance.BannerRecipes;
        }

        public override void AddRecipes()
        {
            Recipe.Create(ModContent.ItemType<AlbinoMandible>())
                .AddIngredient<AlbinoChargerBanner>()
                .AddTile(TileID.Solidifier)
                .DisableDecraft()
                .Register();
        }
    }
}
