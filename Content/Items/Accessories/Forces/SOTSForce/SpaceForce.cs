using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core.Toggler;
using SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant;

namespace SecretsOfTheSouls.Content.Items.Accessories.Forces.SOTSForce
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SpaceForce : BaseForce
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return SecretsOfTheSoulsConfig.Instance.UnfinishedContent;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Enchants[Type] =
            [
                ModContent.ItemType<VibrantEnchant>(),
                ModContent.ItemType<CursedEnchant>(),
                ModContent.ItemType<FrostArtifactEnchant>(),
                ModContent.ItemType<VoidspaceRangerEnchant>()
            ];
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);
            player.AddEffect<VibrantEffect>(Item);
            player.AddEffect<CursedEffect>(Item);
            //player.AddEffect<TinyPlanetoidEffect>(Item);
            //player.AddEffect<CursedAppleEffect>(Item);
            //player.AddEffect<GhostPepperMinionEffect>(Item);
            player.AddEffect<FrostArtifactEffect>(Item);
            player.AddEffect<VoidspaceEffect>(Item);
            player.AddEffect<AbyssalInfernoEffect>(Item);
            player.AddEffect<SpaceEffect>(Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int enchant in Enchants[Type])
            {
                recipe.AddIngredient(enchant);
            }
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SpaceEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<SpaceForce>();
    }
}