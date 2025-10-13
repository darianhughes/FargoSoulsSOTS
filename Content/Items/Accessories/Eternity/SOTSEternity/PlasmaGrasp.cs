using FargowiltasSouls.Content.Items;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using System;
using SOTS.Void;
using SOTS.Buffs;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using SecretsOfTheSouls.Core.Players;
using SecretsOfTheSouls.Content.Projectiles.Eternity.SOTSEternity;
using FargowiltasSouls;
using Terraria.Localization;

namespace SecretsOfTheSouls.Content.Items.Accessories.Eternity.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class PlasmaGrasp : SoulsItem
    {
        public override List<AccessoryEffect> ActiveSkillTooltips => [AccessoryEffectLoader.GetEffect<PlasmaHook>()];
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = (int)(74 * 0.70f);
            Item.height = (int)(68 * 0.70f);
            Item.accessory = true;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(0, 7);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Electrified] = true;

            player.AddEffect<PlasmaHook>(Item);
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();

            int damage = (int)((mp.GadgetCoat ? 40 : 20) * player.ActualClassDamage(ModContent.GetInstance<VoidMelee>()));
            Color color = Color.LightGray;
            float lerp = 0.75f;
            Color tooltipColor = Color.Lerp(Color.Purple, new(225, 90, 90), lerp);
            string textValue = Language.GetTextValue("Mods.SOTS.Common.Damage");

            if (IsNotRuminating(Item))
            {
                int firstTooltip = tooltips.FindIndex(line => line.Name == "Tooltip0");
                if (firstTooltip > 0)
                {
                    string text = Language.GetTextValue("Mods.SOTS.Common.VoidM", (object)damage.ToString(), (object)textValue);
                    var damageTooltip = new TooltipLine(Mod, $"{Mod.Name}:DamageTooltip", text);
                    damageTooltip.OverrideColor = tooltipColor;
                    tooltips.Insert(firstTooltip, damageTooltip);
                }
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Rectangle frame = Main.itemAnimations[Type]?.GetFrame(tex) ?? tex.Frame();
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 pos = Item.Center - Main.screenPosition;
            Color drawColor = Item.GetAlpha(Lighting.GetColor((int)(Item.Center.X / 16f), (int)(Item.Center.Y / 16f)));

            const float quarter = 0.70f;
            Main.EntitySpriteDraw(
                tex,
                pos,
                frame,
                drawColor,
                rotation,
                origin,
                Item.scale * quarter,
                SpriteEffects.None,
                0
            );

            return false; // suppress default draw
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class PlasmaHook : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override bool ActiveSkill => true;

        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            if (player.HasBuff(ModContent.BuffType<VoidRecovery>()))
                return;

            VoidPlayer voidPlayer = VoidPlayer.ModPlayer(player);
            int baseCost = 3;
            if (voidPlayer.safetySwitch && voidPlayer.voidMeter < baseCost && !voidPlayer.frozenVoid)
                return;

            int type = ModContent.ProjectileType<PlasmaHookProj>();
            int hooksOut = 0;
            for (int l = 0; l < Main.maxProjectiles; l++)
            {
                if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == type)
                {
                    hooksOut++;
                }
            }
            if (hooksOut > 1) // This hook can have 2 hooks out.
            {
                return;
            }

            SoundEngine.PlaySound(SoundID.Item1, player.Center);
            if (player.whoAmI == Main.myPlayer)
                voidPlayer.voidMeter -= baseCost;

            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();
            int dmg = Math.Max(1, (int)Math.Round(player.GetTotalDamage(ModContent.GetInstance<VoidMelee>()).ApplyTo(mp.GadgetCoat ? 40 : 20)));
            if (!stunned && player.whoAmI == Main.myPlayer)
            {
                const float speed = 15f;
                Vector2 dir = Main.MouseWorld - player.Center;
                if (dir.LengthSquared() > 0f)
                    dir.Normalize();
                Vector2 velocity = dir * speed;

                Projectile.NewProjectile(
                    player.GetSource_FromThis(),
                    player.Center,
                    velocity,
                    type,
                    dmg,
                    0f,
                    player.whoAmI
                );
            }
        }
    }
}
