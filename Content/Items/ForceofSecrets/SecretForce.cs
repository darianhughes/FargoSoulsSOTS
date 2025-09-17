using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core.Toggler;
using static FargoSoulsSOTS.Content.Items.ForceofSecrets.ElementalEnchant;
using static FargoSoulsSOTS.Content.Items.ForceofSecrets.WormwoodEnchant;
using static FargoSoulsSOTS.Content.Items.ForceofSecrets.FrigidEnchant;
using static FargoSoulsSOTS.Content.Items.ForceofSecrets.FrostArtifactEnchant;
using static FargoSoulsSOTS.Content.Items.ForceofSecrets.TwilightAssassinEnchant;

namespace FargoSoulsSOTS.Content.Items.ForceofSecrets
{
    public class SecretForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Enchants[Type] =
            [
                ModContent.ItemType<ElementalEnchant>(),
                ModContent.ItemType<FrostArtifactEnchant>(),
                ModContent.ItemType<TwilightAssassinEnchant>(),
                ModContent.ItemType<FrigidEnchant>(),
                ModContent.ItemType<WormwoodEnchant>(),
            ];
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);
            player.AddEffect<ElementalEffect>(Item);
            player.AddEffect<FrostArtifactEffect>(Item);
            player.AddEffect<TwilightAssassinEffect>(Item);
            player.AddEffect<FrigidEffect>(Item);
            player.AddEffect<WormwoodEffect>(Item);
            player.AddEffect<BloomStrike>(Item);
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
        public class SecretEffect : AccessoryEffect
        {
            public override Header ToggleHeader => null;
            public override int ToggleItemType => ModContent.ItemType<SecretForce>();
        }
    }
}