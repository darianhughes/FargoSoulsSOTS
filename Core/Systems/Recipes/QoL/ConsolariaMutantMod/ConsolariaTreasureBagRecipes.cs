using Consolaria.Content.Items.Consumables;
using Consolaria.Content.Items.Weapons.Magic;
using Consolaria.Content.Items.Weapons.Melee;
using Consolaria.Content.Items.Weapons.Ranged;
using Consolaria.Content.Items.Weapons.Summon;
using Fargowiltas.Common.Configs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems.Recipes.QoL.ConsolariaMutantMod
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class ConsolariaTreasureBagRecipes : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargoServerConfig.Instance.ContainerRecipes;
        }

        public override void AddRecipes()
        {
            int[] turkorItems =
            {
                ModContent.ItemType<FeatherStorm>(),
                ModContent.ItemType<GreatDrumstick>(),
                ModContent.ItemType<TurkeyStuff>()
            };

            foreach (var item in turkorItems)
            {
                Recipe.Create(item)
                    .AddIngredient<TurkorBag>()
                    .AddTile(TileID.Solidifier)
                    .DisableDecraft()
                    .Register();
            }

            int[] ocramItems =
            {
                ModContent.ItemType<EternityStaff>(),
                ModContent.ItemType<DragonBreath>(),
                ModContent.ItemType<OcramsEye>(),
                ModContent.ItemType<Tizona>()
            };

            foreach (var item in ocramItems)
            {
                Recipe.Create(item)
                    .AddIngredient<OcramBag>()
                    .AddTile(TileID.Solidifier)
                    .DisableDecraft()
                    .Register();
            }

            Recipe.Create(ModContent.ItemType<McMoneypantsInvitation>())
                .AddIngredient<RedEnvelope>(10)
                .AddTile(TileID.WorkBenches)
                .DisableDecraft()
                .Register();
        }
    }
}
