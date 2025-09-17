using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.ForceofChaos
{
    public class ChaosForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<ElementalEnchant>(),
                ModContent.ItemType<TwilightAssassinEnchant>(),
                ModContent.ItemType<WormwoodEnchant>()
            ];
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<ElementalEffect>(Item);
            player.AddEffect<TwilightAssassinEffect>(Item);
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
    }

    public class ChoasEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<ChaosForce>();
    }
}
