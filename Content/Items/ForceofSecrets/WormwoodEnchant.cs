using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.Toggler;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.Items.Nature;
using SOTS.Items.Slime;

namespace FargoSoulsSOTS.Content.Items.ForceofSecrets
{
    public class WormwoodEnchant : BaseEnchant
    {
        public override Color nameColor => new(248, 177, 191);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 3, 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<WormwoodEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<NatureWreath>()
                .AddIngredient<NatureShirt>()
                .AddIngredient<NatureLeggings>()
                .AddIngredient<WormWoodScepter>()
                .AddIngredient<WormWoodHook>()
                .AddIngredient<GelAxe>(300)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    public class WormwoodEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<WormwoodEnchant>();
        public override bool ActiveSkill => Main.LocalPlayer.HasEffectEnchant<WormwoodEffect>();

        public static void ActivateBloomStrike(Player player)
        {
            
        }
    }
}
