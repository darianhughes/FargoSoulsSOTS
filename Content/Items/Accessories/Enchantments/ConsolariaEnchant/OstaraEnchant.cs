using Consolaria.Content.Items.Accessories;
using Consolaria.Content.Items.Armor.Misc;
using Consolaria.Content.Items.Consumables;
using Consolaria.Content.Items.Weapons.Ranged;
using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using SecretsOfTheSouls.Core.SoulToggles.ConsolariaToggles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.ConsolariaEnchant
{
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class OstaraEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return SecretsOfTheSoulsConfig.Instance.UnfinishedContent;
        }

        public override Color nameColor => new Color(148, 214, 107);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<OstaraEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OstaraHat>()
                .AddIngredient<OstaraJacket>()
                .AddIngredient<OstaraBoots>()
                .AddIngredient<OstarasGift>()
                .AddIngredient<EggCannon>()
                .AddIngredient<CandiedFruit>()
                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }

    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class OstaraEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<MightForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<OstaraEnchant>();
    }
}
