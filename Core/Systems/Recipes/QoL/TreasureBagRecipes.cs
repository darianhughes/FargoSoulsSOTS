using SOTS.Items;
using SOTS.Items.AbandonedVillage;
using SOTS.Items.Banners;
using SOTS.Items.Crushers;
using SOTS.Items.Earth.Glowmoth;
using SOTS.Items.Planetarium;
using SOTS.Items.Slime;
using SOTS.Items.Tools;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.Systems.Recipes.QoL
{
    public class TreasureBagRecipes : ModSystem
    {
        public override void AddRecipes()
        {
            int[] glowmothItems =
            {
                ModContent.ItemType<IlluminantAxe>(),
                ModContent.ItemType<GuideToIllumination>(),
                ModContent.ItemType<IlluminantBow>(),
                ModContent.ItemType<IlluminantStaff>(),
                ModContent.ItemType<NightIlluminator>(),
            };

            foreach (var item in glowmothItems)
            {
                Recipe.Create(item)
                    .AddIngredient<GlowmothBag>()
                    .AddTile(TileID.Solidifier)
                    .DisableDecraft()
                    .Register();
            }

            Recipe.Create(ModContent.ItemType<GlowSpores>())
                .AddIngredient<GlowmothTrophy>()
                .AddTile(TileID.Solidifier)
                .DisableDecraft()
                .Register();

            int[] putridItems =
            {
                ModContent.ItemType<GelWings>(),
                ModContent.ItemType<WormWoodParasite>(),
                ModContent.ItemType<WormWoodHelix>(),
                ModContent.ItemType<WormWoodHook>(),
                ModContent.ItemType<WormWoodCollapse>(),
                ModContent.ItemType<WormWoodScepter>(),
                ModContent.ItemType<WormWoodStaff>(),
            };

            foreach (var item in putridItems)
            {
                Recipe.Create(item)
                    .AddIngredient<PinkyBag>()
                    .AddTile(TileID.Solidifier)
                    .DisableDecraft()
                    .Register();
            }

            Recipe.Create(ModContent.ItemType<PeanutButter>())
                .AddIngredient<PutridPinkyTrophy>()
                .AddTile(TileID.Solidifier)
                .DisableDecraft()
                .Register();

            int[] excavatorItems =
            {
                ModContent.ItemType<EarthBreaker>(),
                ModContent.ItemType<EarthGrinder>(),
                ModContent.ItemType<GuardianGreatsword>(),
                ModContent.ItemType<FortressCrasher>(),
                ModContent.ItemType<MagmaBeam>()
            };

            foreach (var item in excavatorItems)
            {
                Recipe.Create(item)
                    .AddIngredient<ExcavatorBossBag>()
                    .AddTile(TileID.Solidifier)
                    .DisableDecraft()
                    .Register();
            }

            Mod sots = ModLoader.GetMod("SOTS");
            int[] advisorItems =
            {
                sots.Find<ModItem>("MeteoriteKey").Type, //why are these internal????
                sots.Find<ModItem>("SkywareKey").Type,
                sots.Find<ModItem>("StrangeKey").Type
            };

            foreach (var item in advisorItems)
            {
                Recipe.Create(item)
                    .AddIngredient<TheAdvisorBossBag>()
                    .AddTile(TileID.Solidifier)
                    .DisableDecraft()
                    .Register();
            }
        }
    }
}
