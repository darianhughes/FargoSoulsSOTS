using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core.Toggler;
using System.Collections.Generic;
using FargoSoulsSOTS.Content.Items.Accessories.Enchantments;

namespace FargoSoulsSOTS.Content.Items.Accessories.Forces
{
    //TEMPORARY FORCE UNTIL WE RELEASE THE HARDMODE UPDATE
    public class VoidForce : BaseForce
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return true;
        }
        public override List<AccessoryEffect> ActiveSkillTooltips => [AccessoryEffectLoader.GetEffect<BloomStrike>()];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Enchants[Type] =
            [
                ModContent.ItemType<VesperaEnchant>(),
                ModContent.ItemType<VibrantEnchant>(),
                ModContent.ItemType<WormwoodEnchant>(),
                ModContent.ItemType<FrigidEnchant>(),
                ModContent.ItemType<PatchLeatherEnchant>(),
                ModContent.ItemType<TwilightAssassinEnchant>(),
            ];
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);
            player.AddEffect<VesperaEffect>(Item);
            player.AddEffect<VibrantEffect>(Item);
            player.AddEffect<WormwoodEffect>(Item);
            player.AddEffect<BloomStrike>(Item);
            player.AddEffect<FrigidEffect>(Item);
            player.AddEffect<PatchLeatherEffect>(Item);
            player.AddEffect<TwilightAssassinEffect>(Item);
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
    public class VoidEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<VoidForce>();
    }
}
