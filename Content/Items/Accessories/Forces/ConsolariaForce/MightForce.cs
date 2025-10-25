using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using SecretsOfTheSouls.Content.Items.Accessories.Enchantments.ConsolariaEnchant;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Accessories.Forces.ConsolariaForce
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class MightForce : BaseForce
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
                ModContent.ItemType<OstaraEnchant>(),
                ModContent.ItemType<DragonEnchant>()
            ];

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 13));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<OstaraEffect>(Item);
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

    public class MightEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<MightForce>();
    }
}
