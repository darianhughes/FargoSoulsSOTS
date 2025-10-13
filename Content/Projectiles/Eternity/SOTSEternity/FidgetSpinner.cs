using System;
using SOTS.Void;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace SecretsOfTheSouls.Content.Projectiles.Eternity.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class FidgetSpinner : ModProjectile
    {
        private int LatchDuration = 60 * 3;
        private float SeekSpeed = 14f;
        private float TurnResponsiveness = 0.18f;
        private float ReturnSpeed = 18f;
        private float ReturnTurn = 0.25f;
        private float ReturnKillDist = 24f;
        private int ReturnMaxTime = 90;

        public override string Texture => "SecretsOfTheSouls/Content/Items/Accessories/Eternity/SOTSEternity/CooledFidgetSpinner";

        /*
         * ai[0] = npcIndex
         * ai[1] = state (0 seek, 1 latched0
         * localAI[0] = latchTimer
         * localAI[1] = orbitAngle
        */

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 70 / 2;
            Projectile.height = 68 / 2;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60 * 10;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = ModContent.GetInstance<VoidRanged>();

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[1] == 0f)
            {
                if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X * 0.6f;
                if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y * 0.6f;
            }
            return false;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.rotation += 0.45f * Math.Sign(Projectile.velocity.X == 0 ? 1 : Projectile.velocity.X);

                int idx = (int)Projectile.ai[0];
                if (idx >= 0 && idx < Main.maxNPCs)
                {
                    NPC npc = Main.npc[idx];
                    if (npc.active && !npc.friendly && npc.CanBeChasedBy(Projectile))
                    {
                        Vector2 desired = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * SeekSpeed;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, TurnResponsiveness);
                    }
                    else AcquireNewTarget();
                }
                else AcquireNewTarget();
            }
            else if (Projectile.ai[1] == 1f)
            {
                int idx = (int)Projectile.ai[0];
                if (idx >= 0 && idx < Main.maxNPCs)
                {
                    NPC npc = Main.npc[idx];
                    if (npc.active)
                    {
                        float radius = Projectile.localAI[1];
                        float angle = Projectile.rotation;
                        Projectile.Center = npc.Center + new Vector2(radius, 0f).RotatedBy(angle);
                        Projectile.velocity = Vector2.Zero;

                        if (Projectile.localAI[0] <= 0f)
                            Projectile.localAI[0] = LatchDuration;

                        Projectile.localAI[0]--;
                        if (Projectile.localAI[0] <= 0f)
                            StartReturn();
                    }
                    else
                    {
                        StartReturn();
                    }
                }
                else
                {
                    StartReturn();
                }
            }
            else
            {
                Player owner = Main.player[Projectile.owner];
                if (!owner.active || owner.dead)
                {
                    Projectile.Kill();
                    return;
                }

                Projectile.rotation += 0.45f;

                Vector2 toOwner = owner.Center - Projectile.Center;
                float dist = toOwner.Length();
                if (dist <= ReturnKillDist)
                {
                    SoundEngine.PlaySound(SoundID.Grab, owner.Center);
                    Projectile.Kill();
                    return;
                }

                Vector2 desired = toOwner.SafeNormalize(Vector2.Zero) * ReturnSpeed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, ReturnTurn);

                if (Projectile.localAI[0] > 0f)
                {
                    Projectile.localAI[0]--;
                    if (Projectile.localAI[0] <= 0f)
                        Projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[0] = target.whoAmI;
                Vector2 offset = Projectile.Center - target.Center;
                Projectile.localAI[1] = offset.Length();
                Projectile.rotation = offset.ToRotation();
                Projectile.ai[1] = 1f;
                Projectile.velocity = Vector2.Zero;
                Projectile.localAI[0] = LatchDuration;
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;

                SoundEngine.PlaySound(SoundID.Item37 with { Pitch = 0.1f }, Projectile.Center);
            }
        }

        private void AcquireNewTarget()
        {
            float maxDist = 650f;
            int found = -1;
            float best = maxDist;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n.active && !n.friendly && n.CanBeChasedBy(Projectile))
                {
                    float d = Vector2.Distance(Projectile.Center, n.Center);
                    if (d < best && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1))
                    {
                        best = d;
                        found = i;
                    }
                }
            }
            Projectile.ai[0] = found;
        }

        private void StartReturn()
        {
            Projectile.ai[1] = 2f;
            Projectile.tileCollide = false;
            Projectile.localAI[0] = ReturnMaxTime; // reuse as return timer
            Projectile.netUpdate = true;

            Player owner = Main.player[Projectile.owner];
            if (owner.active)
            {
                Vector2 toOwner = (owner.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
                Projectile.velocity = toOwner * ReturnSpeed;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(tex.Width * 0.5f, tex.Height * 0.5f);

            const float scale = 0.50f;
            Vector2 visualNudge = Vector2.Zero;

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition + visualNudge,
                null,
                lightColor,
                Projectile.rotation,
                origin,
                scale,
                SpriteEffects.None,
                0
            );
            return false;
            return base.PreDraw(ref lightColor);
        }
    }
}
