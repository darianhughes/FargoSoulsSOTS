using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using SOTS.Items.AbandonedVillage;
using SOTS.Items;
using SOTS.Items.Tools;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using Steamworks;
using SecretsOfTheSouls.Core.Players;
using FargowiltasSouls;

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class EarthenEnchant : BaseEnchant
    {
        public override string Texture => "SecretsOfTheSouls/Content/Items/Accessories/Enchantments/SOTSEnchant/EarthenEnchant";
        public override Color nameColor => new(185, 173, 149);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Blue;
            Item.value = 10000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<EarthenEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EarthenHelmet>()
                .AddIngredient<EarthenChestplate>()
                .AddIngredient<EarthenLeggings>()
                .AddIngredient<Earthshaker>()
                .AddIngredient<ManicMiner>()
                .AddIngredient<MinersSword>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class EarthenEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<WorldShaperHeader>();
        public override int ToggleItemType => ModContent.ItemType<EarthenEnchant>();

        public override void OnHitNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();

            if (item.axe > 0 || item.pick > 0 || item.hammer > 0)
            {
                if (!(mp.MinersCurse >= 100))
                    mp.MinersCurse += player.ForceEffect<EarthenEffect>() ? 10 : 5;

                if (Main.myPlayer == player.whoAmI && Main.netMode == NetmodeID.MultiplayerClient)
                    mp.SendClientChanges(mp);
                mp.MinersCurseDuration = 0;
            }
        }
        public override void ModifyHitByNPC(Player player, NPC npc, ref Player.HurtModifiers modifiers)
        {
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();
            modifiers.FinalDamage.Flat += mp.MinersCurse;
            mp.MinersCurse = 60 * 5;
        }

        public override void ModifyHitByProjectile(Player player, Projectile projectile, ref Player.HurtModifiers modifiers)
        {
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();
            modifiers.FinalDamage.Flat += mp.MinersCurse;
            mp.MinersCurse = 60 * 5;
        }
    }
}
