using SecretsOfTheSouls.Core.SoulToggles;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using SOTS.Buffs;
using SOTS.Void;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SecretsOfTheSouls.Content.Projectiles.Eternity.SOTSEternity;

namespace SecretsOfTheSouls.Content.Items.Accessories.Eternity.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class AnkhoftheCursedRuler : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<PharaohsCurse>()] = true;
            player.buffImmune[BuffID.Cursed] = true;

            VoidPlayer mp = player.GetModPlayer<VoidPlayer>();
            mp.voidMeterMax2 += 50;
            mp.voidRegenSpeed += 0.05f;

            player.AddEffect<CursedExplosionEffect>(Item);
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class CursedExplosionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<AncientMcGuffinHeader>();
        public override int ToggleItemType => ModContent.ItemType<AnkhoftheCursedRuler>();

        public override void OnHurt(Player player, Player.HurtInfo info)
        {
            int dmg = 60 + (int)(info.Damage * 0.25f);

            if (player.whoAmI == Main.myPlayer)
            {
                var src = player.GetSource_FromThis();

                int p = Projectile.NewProjectile(src, player.Center, Vector2.Zero, ModContent.ProjectileType<CursedExplosion>(), dmg, 2f, player.whoAmI);

                if (p >= 0 && p < Main.maxProjectiles)
                {
                    Main.projectile[p].DamageType = ModContent.GetInstance<VoidGeneric>();
                }
            }
        }
    }
}
