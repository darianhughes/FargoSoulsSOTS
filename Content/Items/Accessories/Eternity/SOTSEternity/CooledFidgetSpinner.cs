using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs;
using SecretsOfTheSouls.Content.Projectiles.Eternity.SOTSEternity;
using SecretsOfTheSouls.Core.Players;
using SecretsOfTheSouls.Core.SoulToggles.SOTSToggles;
using SOTS.Void;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Accessories.Eternity.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class CooledFidgetSpinner : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 70 / 2;
            Item.height = 68 / 2;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<CryomagneticDisruption>()] = true;
            player.AddEffect<FidgetSpinnerEffect>(Item);
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();

            int damage = (int)(60 * player.ActualClassDamage(ModContent.GetInstance<VoidRanged>()));
            Color color = Color.LightGray;
            float lerp = 0.75f;
            Color tooltipColor = Color.Lerp(Color.Purple, new(38, 168, 35), lerp);
            string textValue = Language.GetTextValue("Mods.SOTS.Common.Damage");

            if (IsNotRuminating(Item))
            {
                int firstTooltip = tooltips.FindIndex(line => line.Name == "Tooltip0");
                if (firstTooltip > 0)
                {
                    string text = Language.GetTextValue("Mods.SOTS.Common.VoidR", (object)damage.ToString(), (object)textValue);
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

            const float drawScale = 0.50f;
            Vector2 visualNudge = Vector2.Zero;

            Color drawColor = Item.GetAlpha(Lighting.GetColor((int)(Item.Center.X / 16f), (int)(Item.Center.Y / 16f)));
            Main.EntitySpriteDraw(
                tex,
                Item.Center - Main.screenPosition + visualNudge,
                frame,
                drawColor,
                rotation,
                origin,
                drawScale,
                SpriteEffects.None,
                0
            );
            return false;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class FidgetSpinnerEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<GadgetCoatHeader>();
        public override int ToggleItemType => ModContent.ItemType<CooledFidgetSpinner>();
        public override bool ExtraAttackEffect => true;

        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();

            if (projectile.DamageType.CountsAsClass<VoidGeneric>() || item.DamageType.CountsAsClass<VoidGeneric>())
            {
                if (projectile.type != ModContent.ProjectileType<FidgetSpinner>() && player.ownedProjectileCounts[ModContent.ProjectileType<FidgetSpinner>()] < (mp.GadgetCoat ? 5 : 3))
                    TrySpawnSpinner(player, target, hitInfo);
            }
        }

        private void TrySpawnSpinner(Player player, NPC target, NPC.HitInfo hit)
        {
            if (!hit.Crit || target == null || !target.active || target.friendly)
                return;

            if (player.whoAmI != Main.myPlayer)
                return;

            int dmg = (int)player.GetTotalDamage(ModContent.GetInstance<VoidRanged>()).ApplyTo(60);
            float kb = 0f;

            Vector2 spawnPos = player.Center + new Vector2(player.direction * 14f, -6f);
            Vector2 toTarget = target.Center - spawnPos;
            Vector2 vel = toTarget.SafeNormalize(Vector2.UnitX * player.direction) * 12f;

            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, player.Center);
            var source = player.GetSource_OnHit(target);
            int p = Projectile.NewProjectile(source, spawnPos, vel, ModContent.ProjectileType<FidgetSpinner>(), dmg, kb, player.whoAmI, target.whoAmI, 0f);

            if (p >= 0 && Main.projectile.IndexInRange(p))
            {
                var pr = Main.projectile[p];
                pr.velocity = pr.velocity.RotatedByRandom(0.15f);
            }
        }
    }
}
