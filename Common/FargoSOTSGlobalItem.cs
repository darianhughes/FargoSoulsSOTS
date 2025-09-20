using System;
using FargoSoulsSOTS.Content.Buffs;
using FargoSoulsSOTS.Content.Items.Accessories.Enchantments;
using FargoSoulsSOTS.Content.Projectiles.Masomode;
using FargoSoulsSOTS.Core.Players;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using SOTS.Projectiles.Earth;
using SOTS.Void;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Common
{
    public class FargoSOTSGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        private float voidBefore;

        public override void OnConsumeItem(Item item, Player player)
        {
            VoidPlayer mp = VoidPlayer.ModPlayer(player);
            if (item.healLife > 0)
            {
                if (player.HasEffect<VesperaEffect>())
                {
                    player.AddBuff(ModContent.BuffType<VoidAttunement>(), 60 * 25);

                    int heal = player.ForceEffect<VesperaEffect>() ? 50 : 25;

                    mp.voidMeter += heal;
                    VoidPlayer.VoidEffect(player, heal);
                }
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            VoidPlayer vp = VoidPlayer.ModPlayer(player);

            if (player.HasEffect<TwilightAssassinEffect>())
                voidBefore = vp.voidMeter;

            return base.CanUseItem(item, player);
        }

        public override bool? UseItem(Item item, Player player)
        {
            VoidPlayer mp = VoidPlayer.ModPlayer(player);
            FargoSOTSPlayer fargoSOTSPlayer = player.GetModPlayer<FargoSOTSPlayer>();

            if (item.ModItem is VoidItem)
            {
                if (player.HasEffect<VibrantEffect>())
                {
                    var source = player.GetSource_ItemUse(item);
                    Vector2 origin = player.Center;

                    // Aim toward cursor; fallback to facing if overlapping.
                    Vector2 dir = player.DirectionTo(Main.MouseWorld);
                    if (dir == Vector2.Zero)
                        dir = new Vector2(player.direction, 0f);
                    dir.Normalize();

                    const int baseDmg = 10;
                    int dmg = Math.Max(1, (int)Math.Round(player.GetTotalDamage(DamageClass.Ranged).ApplyTo(baseDmg)));

                    float kb = 2f;
                    float speed = 24f;
                    Vector2 baseVel = dir * speed;

                    // Two-bolt spread when Forced
                    float spread = MathHelper.ToRadians(7f);
                    Vector2 velA = baseVel.RotatedBy(-spread);
                    Vector2 velB = baseVel.RotatedBy(spread);

                    // Sideways offset so they don't stack
                    Vector2 perp = new Vector2(-baseVel.Y, baseVel.X);
                    if (perp != Vector2.Zero)
                    {
                        perp.Normalize();
                        perp *= 8f;
                    }

                    int type = ModContent.ProjectileType<VibrantBolt>();

                    if (player.ForceEffect<VibrantEffect>())
                    {
                        Projectile.NewProjectile(source, origin - perp, velA, type, dmg, kb, player.whoAmI, ai0: 1);
                        Projectile.NewProjectile(source, origin + perp, velB, type, dmg, kb, player.whoAmI, ai0: 1);
                    }
                    else
                    {
                        // Single bolt toward cursor, scaled by ranged bonuses
                        Projectile.NewProjectile(source, origin, baseVel, type, dmg, kb, player.whoAmI, ai0: 1);
                    }
                }
            }

            if (player.HasEffect<PatchLeatherEffect>() && player.ownedProjectileCounts[ModContent.ProjectileType<SnakeWhipProjectile>()] < 1)
            {
                var source = player.GetSource_ItemUse(item);
                Vector2 origin = player.Center;

                Vector2 dir = player.DirectionTo(Main.MouseWorld);
                if (dir == Vector2.Zero)
                    dir = new Vector2(player.direction, 0f);
                dir.Normalize();

                bool playerHoldingSummonWeapon =
                    (player.HeldItem.CountsAsClass(DamageClass.Summon) ||
                     player.HeldItem.CountsAsClass(DamageClass.SummonMeleeSpeed));

                int baseDamage = playerHoldingSummonWeapon ? 5 : 4;
                int dmg = Math.Max(1, (int)Math.Round(player.GetTotalDamage(DamageClass.Summon).ApplyTo(baseDamage)));

                float kb = 2f;
                float speed = 12f;
                Vector2 baseVel = dir * speed;

                int type = ModContent.ProjectileType<SnakeWhipProjectile>();

                if (player.ForceEffect<PatchLeatherEffect>())
                {
                    float angleSpread = MathHelper.ToRadians(8f);

                    Vector2 perp = new Vector2(-baseVel.Y, baseVel.X);
                    if (perp != Vector2.Zero)
                    {
                        perp.Normalize();
                        perp *= 6f; // small side offset
                    }

                    Vector2[] vels =
                    {
                        baseVel,
                        baseVel.RotatedBy(-angleSpread),
                        baseVel.RotatedBy(angleSpread)
                    };

                    Vector2[] offsets =
                    {
                        Vector2.Zero,
                        -perp,
                        perp
                    };

                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(source, origin + offsets[i], vels[i], type, dmg, kb, player.whoAmI, ai1: 0.2f);
                    }
                }
                else
                {
                    Projectile.NewProjectile(source, origin, baseVel, type, dmg, kb, player.whoAmI, ai1: 0.4f);
                }
            }

            if (fargoSOTSPlayer.BloomTimeLeft > 0 && IsWeapon(item) && !IsSummonWeapon(item))
                fargoSOTSPlayer.BloomReduced = true;

            bool? useItem = base.UseItem(item, player);

            if (useItem == true)
            {
                if (player.HasEffect<TwilightAssassinEffect>())
                {
                    float voidUsed = voidBefore - mp.voidMeter;
                    double burstsToStore = voidUsed / (player.ForceEffect<TwilightAssassinEffect>() ? 3 : 2);
                    fargoSOTSPlayer.storedCodeBurst = (int)Math.Floor(burstsToStore);
                }
            }

            return useItem;
        }

        private static bool IsWeapon(Item item)
            => item.damage > 0
               && item.useStyle != ItemUseStyleID.None
               && !item.accessory
               && item.ammo == AmmoID.None
               && item.pick <= 0 && item.axe <= 0 && item.hammer <= 0;

        private static bool IsSummonWeapon(Item item)
        {
            if (item == null || item.IsAir) return false;
            return (item.DamageType == DamageClass.Summon || item.CountsAsClass(DamageClass.Summon) || item.DamageType == DamageClass.SummonMeleeSpeed || item.CountsAsClass(DamageClass.SummonMeleeSpeed));
        }
    }
}
