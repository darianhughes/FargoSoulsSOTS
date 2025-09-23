using SOTS.Items.AbandonedVillage;
using SOTS.Items.Banners;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using SOTS.Items.Tide;
using SOTS.Items.Chaos;
using SOTS.Items.Planetarium;
using SOTS.Items.Inferno;
using SOTS.Items.Nature;
using SOTS.Items.Conduit;
using SOTS.Items.Earth;
using SOTS.Items.Pyramid;

namespace FargoSoulsSOTS.Core.Systems.Recipes.QoL
{
    public class BannerRecipes : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe.Create(ModContent.ItemType<PintOPunch>())
                .AddIngredient<BallOGutsBanner>(2)
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<PintOPunch>())
                .AddIngredient<BallOWormsBanner>(2)
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<HydrokineticAntennae>())
                .AddIngredient<BigPhantarayBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<HydrokineticAntennae>())
                .AddIngredient<SmallPhantarayBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<BundleOfSnakes>())
                .AddIngredient<ChimeraBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<CapturedHeart>())
                .AddIngredient<CapturedHeart>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<EarthDrive>())
                .AddIngredient<EarthenGizmoBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<RotHeart>())
                .AddIngredient<FamishedBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<PintOPunch>())
                .AddIngredient<FistfullBanner>(2)
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<BladeGenerator>())
                .AddIngredient<HoloSwordBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<BookOfVirtues>())
                .AddIngredient<LesserWispBanner>(5)
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<BotanicalSymbiote>())
                .AddIngredient<NatureSlimeBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<TwilightBeads>())
                .AddIngredient<PhaseSpeederBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<TinyPlanetoid>())
                .AddIngredient<PlanetoidBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<PintOPunch>())
                .AddIngredient<PupaBanner>(2)
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<PintOPunch>())
                .AddIngredient<RotWalkerBanner>(2)
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<SporeSprayer>())
                .AddIngredient<SittingMushroomBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<LittleWoes>())
                .AddIngredient<ThroeBanner>(2)
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<GravityAnchor>())
                .AddIngredient<TwilightDevilBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<TwilightBeads>())
                .AddIngredient<TwilightDevilBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<GravityAnchor>())
                .AddIngredient<TwilightScouterBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<ThundershockShortbow>())
                .AddIngredient<TwilightScouterBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<GravityAnchor>())
                .AddIngredient<UltracapBanner>()
                .AddTile(TileID.Solidifier)
                .Register();

            Recipe.Create(ModContent.ItemType<TheDarkEye>())
                .AddIngredient<WallMimicBanner>(5)
                .AddTile(TileID.Solidifier)
                .Register();
        }
    }
}
