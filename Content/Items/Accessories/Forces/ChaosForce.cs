using System.Collections.Generic;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Terraria;
using static FargoSoulsSOTS.Content.Items.Accessories.Enchantments.ElementalEnchant;
using Terraria.ModLoader;
using FargoSoulsSOTS.Content.Items.Accessories.Enchantments;

namespace FargoSoulsSOTS.Content.Items.Accessories.Forces
{
    public class ChaosForce : BaseForce
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }
        public override List<AccessoryEffect> ActiveSkillTooltips => [AccessoryEffectLoader.GetEffect<BloomStrike>()];
        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<ElementalEnchant>(),
                ModContent.ItemType<TwilightAssassinEnchant>(),
                ModContent.ItemType<WormwoodEnchant>(),
                ModContent.ItemType<PatchLeatherEnchant>(),
            ];
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TwilightAssassinEffect>(Item);
            player.AddEffect<WormwoodEffect>(Item);
            player.AddEffect<BloomStrike>(Item);
            player.AddEffect<PatchLeatherEffect>(Item);
            player.AddEffect<ChaosTeleport>(Item);
            player.AddEffect<ElementalEffect>(Item);
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
