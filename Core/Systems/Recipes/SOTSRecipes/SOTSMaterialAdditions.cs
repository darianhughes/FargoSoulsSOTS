using System.Linq;
using SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Weapons.Misc;
using SOTS.Items;
using SOTS.Items.DoorItems;
using SOTS.Items.Permafrost;
using SOTS.Items.Planetarium.FromChests;
using SOTS.Items.Pyramid;
using SOTS.Items.Wings;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Armor.Styx;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using FargowiltasSouls.Content.Items.Accessories;
using SOTS.Items.Fragments;
using SOTS.Items.Chaos;
using SOTS.Items.Crushers;
using SOTS.Items.ChestItems;
using SOTS.Items.Conduit;
using SOTS.Items.AbandonedVillage;
using SOTS.Items.CritBonus;
using SOTS.Items.Celestial;
using Fargowiltas.Content.Items.Tiles;
using SOTS.Items.Tide;
using SOTS.Items.Slime;

namespace SecretsOfTheSouls.Core.Systems.Recipes.SOTSRecipes
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSMaterialAdditions : ModSystem
    {
        public override void AddRecipes()
        {
           Recipe.Create(ModContent.ItemType<SnipersSoul>())
                .AddRecipeGroup("SecretsOfTheSouls:AnyQuiver")
                .AddRecipeGroup("FargowiltasSouls:AnySniperScope")
                .AddRecipeGroup("SecretsOfTheSouls:AnySharktoothNecklace")
                .AddIngredient<BackupBow>()
                .AddIngredient<InfinityPouch>()
                .AddIngredient<Calculator>()
                .AddIngredient<FocusReticle>()
                .AddIngredient(ItemID.Blowpipe)
                .AddIngredient(ItemID.Boomstick)
                .AddIngredient(ItemID.BeesKnees)
                .AddIngredient(ItemID.Megashark)
                .AddIngredient<RebarRifle>()
                .AddIngredient(ItemID.Tsunami)
                .AddIngredient<ChaosChamber>()
                .AddIngredient<RoseBow>()
                .AddIngredient<StellarSerpentLauncher>()
                .AddIngredient<DimensionShredder>()
                .AddTile<CrucibleCosmosSheet>()
                .Register();

            Recipe.Create(ModContent.ItemType<SnipersSoul>())
                .AddRecipeGroup("SecretsOfTheSouls:AnyQuiver")
                .AddRecipeGroup("FargowiltasSouls:AnySniperScope")
                .AddRecipeGroup("SecretsOfTheSouls:AnySharktoothNecklace")
                .AddIngredient<BackupBow>()
                .AddIngredient<InfinityPouch>()
                .AddIngredient<Calculator>()
                .AddIngredient<FocusReticle>()
                .AddIngredient<RebarRifle>()
                .AddIngredient(ItemID.Tsunami)
                 .AddIngredient<StellarSerpentLauncher>()
                .AddIngredient<DimensionShredder>()
                .AddIngredient<AbomEnergy>(10)
                .AddTile<CrucibleCosmosSheet>()
                .Register();

            Recipe.Create(ModContent.ItemType<ConjuristsSoul>())
                .AddIngredient(ItemID.PapyrusScarab)
                .AddIngredient<FortressGenerator>()
                .AddIngredient(ItemID.BabyBirdStaff)
                .AddIngredient(ItemID.VampireFrogStaff)
                .AddIngredient(ItemID.BlandWhip)
                .AddIngredient(ItemID.HoundiusShootius)
                .AddIngredient(ItemID.ImpStaff)
                .AddIngredient(ItemID.CoolWhip)
                .AddIngredient(ItemID.OpticStaff)
                .AddIngredient(ItemID.StormTigerStaff)
                .AddIngredient<StarcallerStaff>()
                .AddIngredient(ItemID.EmpressBlade)
                .AddIngredient<Tesseract>()
                .AddIngredient<VoidspaceAuraStaff>()
                .AddIngredient<Lemegeton>()
                .AddTile<CrucibleCosmosSheet>()
                .Register();

            Recipe.Create(ModContent.ItemType<ConjuristsSoul>())
                .AddIngredient(ItemID.PapyrusScarab)
                .AddIngredient<FortressGenerator>()
                .AddIngredient(ItemID.StormTigerStaff)
                .AddIngredient<StarcallerStaff>()
                .AddIngredient(ItemID.EmpressBlade)
                .AddIngredient<Tesseract>()
                .AddIngredient<AbomEnergy>(10)
                .AddTile<CrucibleCosmosSheet>()
                .Register();

        }

        public override void PostAddRecipes()
        {
            for (int index = 0; index < Recipe.numRecipes; ++index)
            {
                Recipe recipe = Main.recipe[index];

                if (recipe.HasResult<TophatSquirrelWeapon>())
                {
                    int frightSoulCount = recipe.requiredItem.Where(i => i.type == ItemID.SoulofFright).Sum(i => i.stack);
                    int plightSoulCount = frightSoulCount;

                    recipe.AddIngredient<SoulOfPlight>(plightSoulCount);
                }

                if (recipe.HasResult<StyxCrown>() || recipe.HasResult<StyxChestplate>() || recipe.HasResult<StyxLeggings>())
                {
                    int lunarBarCount = recipe.requiredItem.Where(i => i.type == ItemID.LunarBar).Sum(i => i.stack);
                    int energyCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<AbomEnergy>()).Sum(i => i.stack);

                    recipe.RemoveIngredient(ItemID.LunarBar);
                    recipe.RemoveIngredient(ModContent.ItemType<AbomEnergy>());

                    recipe.AddIngredient<SoulOfPlight>(5);
                    recipe.AddIngredient(ItemID.LunarBar, lunarBarCount);
                    recipe.AddIngredient<AbomEnergy>(energyCount);
                }

                if (recipe.HasResult<BerserkerSoul>() && !recipe.HasIngredient<AbomEnergy>())
                {
                    recipe.AddIngredient<SupernovaEmblem>();
                    recipe.AddIngredient<Hyperdrive>();
                    recipe.AddIngredient<AquaticEclipse>();
                    recipe.AddIngredient<Sawflake>();
                    recipe.AddIngredient<SkipScythe>();
                    recipe.AddIngredient<KingBlade>();
                    recipe.AddIngredient<SubspaceScissors>();
                }

                if (recipe.HasResult<BerserkerSoul>() && recipe.HasIngredient<AbomEnergy>())
                {
                    int energyCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<AbomEnergy>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<AbomEnergy>());

                    recipe.AddIngredient<SupernovaEmblem>();
                    recipe.AddIngredient<Hyperdrive>();
                    recipe.AddIngredient<Sawflake>();
                    recipe.AddIngredient<SubspaceScissors>();

                    recipe.AddIngredient<AbomEnergy>(energyCount);
                }

                if (recipe.HasResult<SnipersSoul>() && !recipe.HasIngredient<BackupBow>())
                {
                    recipe.DisableRecipe();
                }

                if (recipe.HasResult<ArchWizardsSoul>() && !recipe.HasIngredient<AbomEnergy>())
                {
                    recipe.RemoveIngredient(ItemID.DemonScythe);

                    recipe.AddIngredient<PlasmaShrimp>();
                    recipe.AddIngredient<WishingStar>();
                    recipe.AddIngredient<PutridEye>();
                    recipe.AddIngredient<BrachialLance>();
                    recipe.AddIngredient<TangleStaff>();
                    recipe.AddIngredient<StellarShot>();
                    recipe.AddIngredient<DanceOfDeath>();
                    recipe.AddIngredient<Apocalypse>();
                }

                if (recipe.HasResult<ArchWizardsSoul>() && recipe.HasIngredient<AbomEnergy>())
                {
                    int energyCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<AbomEnergy>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<AbomEnergy>());

                    recipe.AddIngredient<PlasmaShrimp>();
                    recipe.AddIngredient<WishingStar>();
                    recipe.AddIngredient<TangleStaff>();
                    recipe.AddIngredient<Apocalypse>();

                    recipe.AddIngredient<AbomEnergy>(energyCount);
                }

                if (recipe.HasResult<ConjuristsSoul>() && !recipe.HasIngredient<FortressGenerator>())
                { 
                    recipe.DisableRecipe();
                }

                if (recipe.HasResult<AeolusBoots>())
                {
                    int frightSoulCount = recipe.requiredItem.Where(i => i.type == ItemID.SoulofFright).Sum(i => i.stack);
                    int mightSoulCount = recipe.requiredItem.Where(i => i.type == ItemID.SoulofMight).Sum(i => i.stack);
                    int sightSoulCount = recipe.requiredItem.Where(i => i.type == ItemID.SoulofSight).Sum(i => i.stack);

                    int plightSoulCount = frightSoulCount;

                    int energyCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<DeviatingEnergy>()).Sum(i => i.stack);

                    recipe.RemoveIngredient(ItemID.SoulofFright);
                    recipe.RemoveIngredient(ItemID.SoulofMight);
                    recipe.RemoveIngredient(ItemID.SoulofSight);
                    recipe.RemoveIngredient(ModContent.ItemType<DeviatingEnergy>());

                    //recipe.AddIngredient<FlashsparkBoots>();
                    if (frightSoulCount > 0)
                        recipe.AddIngredient(ItemID.SoulofFright, frightSoulCount);
                    if (mightSoulCount > 0)
                    recipe.AddIngredient(ItemID.SoulofMight, mightSoulCount);
                    if (sightSoulCount > 0)
                    recipe.AddIngredient(ItemID.SoulofSight, sightSoulCount);
                    if (plightSoulCount > 0 & !recipe.HasIngredient<SoulOfPlight>())
                        recipe.AddIngredient<SoulOfPlight>(plightSoulCount);
                    recipe.AddIngredient<DeviatingEnergy>(energyCount);
                }

                if (recipe.HasResult<SupersonicSoul>())
                {
                    recipe.RemoveIngredient(ModContent.ItemType<AeolusBoots>());
                    recipe.RemoveIngredient(ItemID.FlyingCarpet);

                    recipe.AddIngredient<SubspaceBoosters>();
                    //recipe.AddIngredient<ShoeIce>();
                    recipe.AddIngredient<BandOfDoor>();
                    //recipe.AddIngredient<TheDarkEye>();
                    recipe.AddIngredient<SpiritSurfer>();
                }

                if (recipe.HasResult<WorldShaperSoul>())
                {
                    recipe.AddIngredient<RockCandy>();
                    recipe.AddIngredient<ArchaeologistToolbelt>();
                    recipe.AddIngredient<DrillHand>();
                    recipe.AddIngredient<GreedierRing>();
                    recipe.AddIngredient<Lockpick>();
                    recipe.AddIngredient<EarthenEnchant>();
                }

                if (recipe.HasResult<TrawlerSoul>())
                {
                    recipe.AddIngredient<ZombieHand>();
                    recipe.AddIngredient<TwilightFishingPole>();
                    //recipe.AddIngredient<ZephyrousZeppelin>();
                    //recipe.AddIngredient<LuckyPurpleBalloon>();
                }

                if (recipe.HasResult<FlightMasterySoul>())
                {
                    recipe.RemoveIngredient(ItemID.EmpressFlightBooster);
                    recipe.RemoveIngredient(ItemID.CreativeWings);

                    recipe.AddRecipeGroup("SecretsOfTheSouls:PreHMWings");
                    recipe.AddIngredient<GildedBladeWings>();
                }

                if (recipe.HasResult<Devilshield>())
                {
                    if (recipe.HasIngredient(ItemID.FrozenTurtleShell))
                    {
                        recipe.RemoveIngredient(ItemID.FrozenTurtleShell);
                        recipe.AddIngredient<ShardGuard>();
                    }
                    if (recipe.HasIngredient(ItemID.FrozenShield))
                    {
                        recipe.AddIngredient<PermafrostMedallion>();
                        recipe.AddIngredient<ShatterHeartShield>();
                        recipe.AddIngredient<DissolvingAurora>();
                    }
                }

                if (recipe.HasResult<ColossusSoul>())
                {
                    recipe.RemoveIngredient(ItemID.AnkhShield);
                    recipe.RemoveIngredient(ItemID.CharmofMyths);

                    recipe.AddIngredient<BulwarkOfTheAncients>();
                    recipe.AddIngredient<AlchemistsCharm>();
                    recipe.AddIngredient<Sandwich>();
                }

                if (recipe.HasResult<TerrariaSoul>())
                {
                    /*
                    int energyCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<AbomEnergy>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<AbomEnergy>());

                    if (SecretsOfTheSoulsConfig.Instance.UnfinishedContent)
                    {
                        if (!SecretsOfTheSoulsCrossmod.CommunitySoulsExpansion.Loaded)
                        {
                            recipe.AddIngredient<ChaosForce>();
                            recipe.AddIngredient<SpaceForce>();
                        }
                    }
                    else
                    {
                        recipe.AddIngredient<VoidForce>();
                    }

                    recipe.AddIngredient<AbomEnergy>(energyCount);
                    */
                }

                if (recipe.HasResult<DubiousCircuitry>())
                {
                    int frightSoulCount = recipe.requiredItem.Where(i => i.type == ItemID.SoulofFright).Sum(i => i.stack);
                    int plightSoulCount = frightSoulCount;
                    int energyCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<DeviatingEnergy>()).Sum(i => i.stack);

                    recipe.RemoveIngredient(ModContent.ItemType<DeviatingEnergy>());

                    recipe.AddIngredient<SoulOfPlight>(plightSoulCount);
                    recipe.AddIngredient<DeviatingEnergy>(energyCount);
                }
            }
        }
    }
}
