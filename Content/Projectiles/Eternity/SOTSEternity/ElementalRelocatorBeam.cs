using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using SOTS.Buffs.Debuffs;
using SOTS.Common.GlobalNPCs;
using SOTS.Dusts;
using SOTS.Helpers;
using SOTS;
using Terraria.GameContent;
using Terraria;

namespace SecretsOfTheSouls.Content.Projectiles.Eternity.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class ElementalRelocatorBeam : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        private readonly List<Vector2> drawPositions = new();
        private readonly List<int> ignoredNpc = new();
        private Vector2 firstDestination = Vector2.Zero;

        private bool spawned = true;
        private float redirectGrowth;
        private Vector2? blinkDestination;

        public const float Speed = 3f;
        public const float SeekOutOthersRange = 96f;

        public int GrowthRange = 20;
        public int DegradeRange = 10;

        public override string Texture => "SOTS/Projectiles/Chaos/RelocatorBeam";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.timeLeft = 60;

            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 0f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            if (!target.friendly && !target.townNPC && !target.dontTakeDamage && !target.boss)
            {
                DebuffNPC.SetTimeFreeze(Main.player[Projectile.owner], target, 120);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => true;

        public override bool? CanHitNPC(NPC target)
        {
            if (target == null || !target.active || target.friendly || target.townNPC)
                return false;

            return !ignoredNpc.Contains(target.whoAmI);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (spawned)
                return false;

            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(tex.Width / 2f, tex.Height / 2f);

            Color baseColor = new(140, 140, 140, 0);

            float lifeFrac = Projectile.timeLeft / Math.Max(1f, Projectile.ai[2]);
            float rot = Projectile.velocity.ToRotation();

            int count = drawPositions.Count;
            int startIndex = (int)((1f - lifeFrac) * count);
            int seg = 0;

            for (int i = startIndex; i < count; i++)
            {
                float widthFactor = 1f;
                if (seg < GrowthRange)
                    widthFactor = seg / (float)GrowthRange;
                else if (i > count - DegradeRange)
                    widthFactor = 1f - (i - (count - DegradeRange)) / (float)DegradeRange;

                Vector2 p = drawPositions[i];

                Color rim = ColorHelper.Pastel(MathHelper.ToRadians(i * 3));
                rim.A = 0;

                Vector2 offset = new Vector2(0f, 12f * widthFactor * (float)Math.Sin(MathHelper.ToRadians((int)Main.GameUpdateCount * 6 + i * 6)))
                                 .RotatedBy(rot);

                Main.spriteBatch.Draw(
                    tex,
                    p - Main.screenPosition,
                    null,
                    baseColor * ((255 - Projectile.alpha) / 255f),
                    rot,
                    origin,
                    new Vector2(1f, widthFactor * 0.75f) * Projectile.scale,
                    SpriteEffects.None,
                    0f);

                Main.spriteBatch.Draw(
                    tex,
                    p + offset - Main.screenPosition,
                    null,
                    rim * ((255 - Projectile.alpha) / 255f),
                    rot,
                    origin,
                    new Vector2(1f, widthFactor * 0.375f) * Projectile.scale,
                    SpriteEffects.None,
                    0f);

                if (i < count - 1)
                    rot = (drawPositions[i + 1] - p).ToRotation();

                seg++;
            }

            return false;
        }

        private void SetupLaser()
        {
            float radians = Projectile.velocity.ToRotation();
            Vector2 endTarget = new(Projectile.ai[0], Projectile.ai[1]);
            int chaseIndex = (int)Projectile.knockBack;

            int maxChainHits = 20;
            float seekRangeMul = 1.5f;

            if (chaseIndex >= 0)
            {
                NPC n = Main.npc[chaseIndex];
                if (n.CanBeChasedBy())
                    firstDestination = n.Center;
            }

            Vector2 p = Projectile.Center;
            Vector2 vel = Vector2.Normalize(Projectile.velocity == Vector2.Zero ? new Vector2(0f, 1f) : Projectile.velocity) * Speed;

            bool reachedEnd = false;
            int steps = 0;

            while (!reachedEnd && steps < 4000)
            {
                p += vel;
                drawPositions.Add(p);

                // Pretty dust
                if (Main.rand.NextBool(3))
                {
                    Dust d = Dust.NewDustPerfect(
                        p,
                        ModContent.DustType<CopyDust4>(),
                        Main.rand.NextVector2Circular(3f, 3f),
                        120,
                        default,
                        1f);
                    d.velocity += Projectile.velocity * 0.1f;
                    d.noGravity = true;
                    d.color = ColorHelper.Pastel(Main.rand.NextFloat(0f, MathHelper.TwoPi));
                    d.fadeIn = 0.2f;
                    d.scale *= 2.2f;
                }

                bool followedTargetThisStep = false;

                if (firstDestination != Vector2.Zero && chaseIndex >= 0)
                {
                    NPC seed = Main.npc[chaseIndex];
                    Rectangle beamCell = new Rectangle((int)p.X - Projectile.width / 2, (int)p.Y - Projectile.height / 2, Projectile.width, Projectile.height);

                    if (beamCell.Contains(seed.Center.ToPoint()))
                    {
                        ignoredNpc.Add(chaseIndex);
                        firstDestination = Vector2.Zero;
                    }
                    else
                    {
                        redirectGrowth = 0f;
                        radians = Redirect(radians, p, firstDestination);
                        followedTargetThisStep = true;
                    }
                }
                else if (maxChainHits > 0 && steps > 10)
                {
                    chaseIndex = SOTSNPCs.FindTarget_Ignore(p, ignoredNpc, SeekOutOthersRange * seekRangeMul);
                    if (chaseIndex >= 0)
                    {
                        NPC next = Main.npc[chaseIndex];
                        Rectangle beamCell = new Rectangle((int)p.X - Projectile.width / 2, (int)p.Y - Projectile.height / 2, Projectile.width, Projectile.height);

                        if (beamCell.Contains(next.Center.ToPoint()))
                        {
                            redirectGrowth = 0f;
                            ignoredNpc.Add(chaseIndex);
                            maxChainHits--;
                        }
                        else
                        {
                            radians = Redirect(radians, p, next.Center);
                            followedTargetThisStep = true;
                        }
                    }
                }

                if (!followedTargetThisStep)
                {
                    Rectangle beamCell = new Rectangle((int)p.X - Projectile.width / 2, (int)p.Y - Projectile.height / 2, Projectile.width, Projectile.height);
                    if (beamCell.Contains(endTarget.ToPoint()))
                        reachedEnd = true;

                    radians = Redirect(radians, p, endTarget);
                }

                vel = new Vector2(1f, 0f).RotatedBy(radians) * Speed;
                steps++;
            }

            if (drawPositions.Count / 3 < DegradeRange)
                DegradeRange = drawPositions.Count / 3;
            if (drawPositions.Count / 6 < GrowthRange)
                GrowthRange = drawPositions.Count / 6;

            Projectile.Center = endTarget;
        }

        private float Redirect(float radians, Vector2 pos, Vector2 target)
        {
            Vector2 to = target - pos;
            float steer = 1f + redirectGrowth;

            float newRot = (new Vector2(3f, 0f).RotatedBy(radians) + Vector2.Normalize(to) * steer).ToRotation();

            redirectGrowth += 0.1f;
            return newRot;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Player p = Main.player[Projectile.owner];

            if (Projectile.ai[2] <= 0f)
                Projectile.ai[2] = 60f;

            if (Projectile.timeLeft > Projectile.ai[2])
                Projectile.timeLeft = (int)Projectile.ai[2];

            if (spawned)
            {
                blinkDestination = SimulateRodOfDiscordTeleport(p);
                if (!blinkDestination.HasValue)
                {
                    spawned = false;
                    Projectile.Kill();
                    return;
                }

                for (int i = 0; i < 20; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CopyDust4>(), 0f, 0f, 120);
                    d.velocity += Projectile.velocity * 0.1f;
                    d.noGravity = true;
                    d.color = ColorHelper.Pastel(MathHelper.ToRadians(i * 18));
                    d.fadeIn = 0.2f;
                    d.scale *= 2.2f;
                }

                SetupLaser();

                spawned = false;

                p.immuneTime = 40;
                p.immune = true;

                p.Teleport(blinkDestination.Value, 1);

                if (Projectile.owner == Main.myPlayer)
                    NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, p.whoAmI, blinkDestination.Value.X, blinkDestination.Value.Y, 1);

                SOTSUtils.PlaySound(SoundID.Item72, (int)p.Center.X, (int)p.Center.Y, 1.2f, 0.1f);

                // Exit burst
                for (int i = 0; i < 20; i++)
                {
                    Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CopyDust4>(), 0f, 0f, 120);
                    d.velocity += Projectile.velocity * 0.1f;
                    d.noGravity = true;
                    d.color = ColorHelper.Pastel(MathHelper.ToRadians(i * 18));
                    d.fadeIn = 0.2f;
                    d.scale *= 2.2f;
                }
            }

            float t = Projectile.timeLeft / Math.Max(1f, Projectile.ai[2]);
            Projectile.alpha = (int)(255f - 255f * t * t);
        }

        private Vector2? SimulateRodOfDiscordTeleport(Player player)
        {
            Vector2 dst = new(Projectile.ai[0], Projectile.ai[1]);

            Vector2 pos = dst;
            pos.Y -= player.height * 0.5f;

            if (pos.X <= 50f || pos.X >= Main.maxTilesX * 16 - 50f || pos.Y <= 50f || pos.Y >= Main.maxTilesY * 16 - 50f)
                return null;

            int tx = (int)(pos.X / 16f);
            int ty = (int)(pos.Y / 16f);

            Tile tile = Main.tile[tx, ty];

            bool badWall = (tile.WallType == 87 && !NPC.downedPlantBoss && (Main.remixWorld || ty > Main.worldSurface));
            bool blocked = Collision.SolidCollision(pos, player.width, player.height);

            if (badWall || blocked)
                return null;

            return pos;
        }
    }
}
