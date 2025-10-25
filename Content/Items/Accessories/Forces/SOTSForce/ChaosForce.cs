using System.Collections.Generic;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Terraria;
using Terraria.ModLoader;
using SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant;

namespace SecretsOfTheSouls.Content.Items.Accessories.Forces.SOTSForce
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class ChaosForce : BaseForce
    {
        public override List<AccessoryEffect> ActiveSkillTooltips => [AccessoryEffectLoader.GetEffect<BloomStrike>()];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

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
            //player.AddEffect<HoloEyeMinionEffect>(Item);
            player.AddEffect<WormwoodEffect>(Item);
            player.AddEffect<BloomStrike>(Item);
            player.AddEffect<PatchLeatherEffect>(Item);
            //player.AddEffect<ChaosTeleport>(Item);
            player.AddEffect<ElementalEffect>(Item);
            player.AddEffect<PolarizerEffect>(Item);

            player.AddEffect<ChaosEffect>(Item);
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

    public class ChaosEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
    }
}
