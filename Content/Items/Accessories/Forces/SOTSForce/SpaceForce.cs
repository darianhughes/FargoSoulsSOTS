using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core.Toggler;
using FargoSoulsSOTS.Content.Items.Accessories.Enchantments.SOTSEnchant;

namespace FargoSoulsSOTS.Content.Items.Accessories.Forces.SOTSForce
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class SpaceForce : BaseForce
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargoSOTSConfig.Instance.UnfinishedContent;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Enchants[Type] =
            [
                ModContent.ItemType<VibrantEnchant>(),
                ModContent.ItemType<CursedEnchant>(),
                ModContent.ItemType<FrostArtifactEnchant>(),
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
            player.AddEffect<TinyPlanetoidEffect>(Item);
            player.AddEffect<CursedAppleEffect>(Item);
            player.AddEffect<GhostPepperMinionEffect>(Item);
            player.AddEffect<FrostArtifactEffect>(Item);
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

    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class SpaceEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<SpaceForce>();
    }
}