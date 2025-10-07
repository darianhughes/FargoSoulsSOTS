using SecretsOfTheSouls.Core.SoulToggles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using SOTS.Items.Earth;
using SOTS.Items.Flails;
using SOTS.Void;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class VibrantEnchant : BaseEnchant
    {
        public override string Texture => "SecretsOfTheSouls/Content/Items/Accessories/Enchantments/SOTSEnchant/VibrantEnchant";
        public override Color nameColor => new(181, 220, 97);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<VibrantEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<VibrantHelmet>()
                .AddIngredient<VibrantChestplate>()
                .AddIngredient<VibrantLeggings>()
                .AddIngredient<VibrantPistol>()
                .AddIngredient<Shattershine>()
                .AddIngredient<VibrantBlade>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class VibrantEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<SpaceForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<VibrantEnchant>();
        public override bool ExtraAttackEffect => true;

        public override void PostUpdateEquips(Player player)
        {
            VoidPlayer.ModPlayer(player).voidCost -= 0.1f;
        }
    }
}
