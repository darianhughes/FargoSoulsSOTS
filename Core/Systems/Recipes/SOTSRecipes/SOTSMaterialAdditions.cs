using System.Linq;
using SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Weapons.Misc;
using SOTS.Items;
using SOTS.Items.DoorItems;
using SOTS.Items.Fishing;
using SOTS.Items.Permafrost;
using SOTS.Items.Planetarium.FromChests;
using SOTS.Items.Pyramid;
using SOTS.Items.Wings;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Armor.Styx;
using FargowiltasSouls.Content.Items.Accessories.Eternity;
using SecretsOfTheSouls.Content.Items.Accessories.Souls.SOTSSoul;
using FargowiltasSouls.Content.Items.Accessories;
using SOTS.Items.Fragments;
using SOTS.Items.Chaos;
using SOTS.Items.Crushers;
using SOTS.Items.ChestItems;
using SOTS.Items.Conduit;
using SOTS.Items.AbandonedVillage;
using SOTS.Items.CritBonus;
using SOTS.Items.Inferno;
using SOTS.Items.Celestial;
using Fargowiltas.Content.Items.Tiles;

namespace SecretsOfTheSouls.Core.Systems.Recipes.SOTSRecipes
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSMaterialAdditions : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe sniperSoul = Recipe.Create(ModContent.ItemType<SnipersSoul>());
            sniperSoul.AddIngredient(ItemID.RangerEmblem)
                .AddRecipeGroup("SecretsOfTheSouls:AnyQuiver")
                .AddRecipeGroup("FargowiltasSouls:AnySniperScope")
                .AddRecipeGroup("SecretsOfTheSouls:AnySharktoothNecklace")
                .AddIngredient<BackupBow>()
                .AddIngredient<InfinityPouch>()
                .AddIngredient<Calculator>()
                .AddIngredient<FocusReticle>()
                .AddIngredient(ItemID.FlintlockPistol)
                .AddIngredient(ItemID.Blowgun)
                .AddIngredient(ItemID.PewMaticHorn)
                .AddIngredient(ItemID.Megashark)
                .AddIngredient<PlasmaAccelerator>()
                .AddIngredient<RebarRifle>()
                .AddIngredient(ItemID.ElfMelter)
                .AddIngredient(ItemID.Tsunami)
                .AddIngredient(ItemID.Xenopopper)
                .AddIngredient<ChaosChamber>()
                .AddIngredient<RoseBow>()
                .AddIngredient<StellarSerpentLauncher>()
                .AddIngredient<CataclysmSpheres>()
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

                if (recipe.HasResult<BerserkerSoul>())
                {
                    recipe.RemoveIngredient(ItemID.WarriorEmblem);

                    recipe.AddIngredient<SupernovaEmblem>();
                    recipe.AddIngredient<Hyperdrive>();
                    recipe.AddIngredient<KingBlade>();
                    recipe.AddIngredient<Sawflake>();
                    recipe.AddIngredient<AquaticEclipse>();
                    recipe.AddIngredient<SkipScythe>();
                    recipe.AddIngredient<IrradiatedChainReactor>();
                }

                if (recipe.HasResult<SnipersSoul>() && recipe.HasIngredient(ItemID.PiranhaGun))
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
                    recipe.AddIngredient<ZephyrousZeppelin>();
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

                if (recipe.HasResult<EternitySoul>())
                {
                    int energyCount = recipe.requiredItem.Where(i => i.type == ModContent.ItemType<EternalEnergy>()).Sum(i => i.stack);
                    recipe.RemoveIngredient(ModContent.ItemType<EternalEnergy>());

                    recipe.AddIngredient<SubspaceVoyagerSoul>();
                    recipe.AddIngredient<EternalEnergy>(energyCount);
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
