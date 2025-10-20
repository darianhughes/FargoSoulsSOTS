using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using XDContentMod.Content.Items.Weapons.Melee;
using XDContentMod.Content.Items.Pets;
using XDContentMod.Content.Items.Mounts;
using XDContentMod.Content.Items.Placeable;
using XDContentMod.Content.Items.Materials;

namespace SecretsOfTheSouls.Core.Systems.Recipes.QoL
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Heartbeataria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Heartbeataria.Name)]
    public class HeartbeatariaNPCRecipes : ModSystem
    {
        private static readonly Mod mutant = ModLoader.GetMod("Fargowiltas");

        public override void AddRecipes()
        {
            int starMerchant = Mod.Find<ModItem>("StarMerchant").Type;

            Recipe.Create(starMerchant)
                .AddIngredient(mutant.Find<ModItem>("TravellingMerchant").Type)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.SkyMill)
                .DisableDecraft()
                .Register();

            //Melee Weapons
            Recipe.Create(ModContent.ItemType<HeartbeatBroadsword>())
                .AddIngredient(starMerchant)
                .AddIngredient(ItemID.GoldCoin, 10)
                .AddIngredient(ItemID.LifeCrystal)
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();

            Recipe.Create(ModContent.ItemType<TapTapBroadsword>())
                .AddIngredient(starMerchant)
                .AddIngredient(ItemID.GoldCoin, 10)
                .AddIngredient(ItemID.SunplateBlock, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();

            Recipe.Create(ModContent.ItemType<KFCChickenDrumstick>())
                .AddIngredient(starMerchant)
                .AddIngredient(ItemID.GoldCoin, 10)
                .AddIngredient(ItemID.ChickenNugget, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();

            //Disc Weapons
            int[] discWeapons =
            {
                ModContent.ItemType<BaiduTiebaHuajiDisc>(),
                ModContent.ItemType<HupuDisc>(),
                ModContent.ItemType<iFlytekDisc>(),
                ModContent.ItemType<LOOKDisc>(),
                ModContent.ItemType<PururuDisc>(),
            };

            foreach (int disc in discWeapons) 
            {
                Recipe.Create(disc)
                    .AddIngredient(starMerchant)
                    .AddIngredient(ItemID.GoldCoin, 20)
                    .AddTile(TileID.TinkerersWorkbench)
                    .DisableDecraft()
                    .Register();
            }

            //Pets
            Recipe.Create(ModContent.ItemType<Basketball>())
                .AddIngredient(starMerchant)
                .AddIngredient(ItemID.GoldCoin, 50)
                .AddIngredient(ItemID.Leather, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();

            Recipe.Create(ModContent.ItemType<FriedChickenNugget>())
                .AddIngredient(starMerchant)
                .AddIngredient(ItemID.GoldCoin, 50)
                .AddIngredient(ItemID.ChickenNugget)
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();

            Recipe.Create(ModContent.ItemType<PururuCharger>())
                .AddIngredient(starMerchant)
                .AddIngredient(ItemID.GoldCoin, 50)
                .AddIngredient<PururuDisc>()
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();

            Recipe.Create(ModContent.ItemType<Xiaokuai>())
                .AddIngredient(starMerchant)
                .AddIngredient(ItemID.GoldCoin, 50)
                .AddIngredient(ItemID.OrangeDye, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();

            Recipe.Create(ModContent.ItemType<Xiaoliu>())
                .AddIngredient(starMerchant)
                .AddIngredient(ItemID.GoldCoin, 50)
                .AddIngredient(ItemID.OrangeDye, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();

            //Leaf Pet
            int[] leafPets =
            {
                ModContent.ItemType<Leaf>(),
                ModContent.ItemType<ToadQuack2R>(),
                ModContent.ItemType<ToadQuack2W>()
            };

            foreach (int leaf in leafPets)
            {
                Recipe.Create(leaf)
                    .AddIngredient(starMerchant)
                    .AddIngredient(ItemID.GoldCoin, 30)
                    .AddIngredient(ItemID.Waterleaf, 5)
                    .AddTile(TileID.TinkerersWorkbench)
                    .DisableDecraft()
                    .Register();
            }

            //Vehicle
            int[] vehicles =
            {
                ModContent.ItemType<ConvertibleKeys>(),
                ModContent.ItemType<DiDiBikeKeys>(),
                ModContent.ItemType<DiDiCarKeys>(),
                ModContent.ItemType<KFCDeliveryScooterKeys>(),
                ModContent.ItemType<TapTapMinivanKeys>()
            };

            foreach (int vehicle in vehicles)
            {
                Recipe.Create(vehicle)
                    .AddIngredient(starMerchant)
                    .AddIngredient(ItemID.GoldCoin, 60)
                    .AddIngredient(ItemID.GolfCart)
                    .AddTile(TileID.TinkerersWorkbench)
                    .DisableDecraft()
                    .Register();
            }

            Recipe.Create(ModContent.ItemType<KFCChair>())
                .AddIngredient(starMerchant)
                .AddIngredient(ItemID.WoodenChair)
                .AddIngredient(ItemID.RedDye)
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();

            Recipe.Create(ModContent.ItemType<KFCWorkBench>())
                .AddIngredient(starMerchant)
                .AddIngredient(ItemID.WorkBench)
                .AddIngredient(ItemID.RedDye)
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();

            Recipe.Create(ModContent.ItemType<KFCBar>())
                .AddIngredient(starMerchant)
                .AddIngredient(ItemID.Bar)
                .AddIngredient(ItemID.RedDye)
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();

            Recipe.Create(ModContent.ItemType<FusionModule>())
                .AddIngredient(starMerchant)
                .AddIngredient(ItemID.GoldCoin, 10)
                .AddTile(TileID.TinkerersWorkbench)
                .DisableDecraft()
                .Register();
        }
    }
}
