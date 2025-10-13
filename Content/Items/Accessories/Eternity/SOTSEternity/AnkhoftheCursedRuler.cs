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
using SecretsOfTheSouls.Core.SoulToggles.SOTSToggles;
using FargowiltasSouls;
using SecretsOfTheSouls.Core.Players;
using System.Collections.Generic;
using Terraria.Localization;

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

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            int damage = 60;
            Color color = Color.LightGray;
            float lerp = 0.75f;
            Color tooltipColor = Color.Lerp(Color.Purple, Color.LightGray, lerp);
            string textValue = Language.GetTextValue("Mods.SOTS.Common.Damage");

            if (IsNotRuminating(Item))
            {
                int firstTooltip = tooltips.FindIndex(line => line.Name == "Tooltip0");
                if (firstTooltip > 0)
                {
                    string text = Language.GetTextValue("Mods.SOTS.Common.Void2", (object)damage.ToString(), (object)textValue);
                    var damageTooltip = new TooltipLine(Mod, $"{Mod.Name}:DamageTooltip", text);
                    damageTooltip.OverrideColor = tooltipColor;
                    tooltips.Insert(firstTooltip, damageTooltip);
                }
            }
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
