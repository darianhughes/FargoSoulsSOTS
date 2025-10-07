using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SOTS.Dusts;
using SOTS;
using SOTS.Void;
using Terraria.Audio;
using SecretsOfTheSouls.Core.Players;

namespace SecretsOfTheSouls.Content.Projectiles.Eternity.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class PlasmaHookProj : ModProjectile
    {
        private int initialDirection;
        private Vector2 Target = Vector2.Zero;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 14;
            SoundEngine.PlaySound(SoundID.Item94, new Vector2?(Projectile.Center), null);
            return true;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SingleGrappleHook[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(235);
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1000000;

            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<VoidMelee>();
        }

        public override float GrappleRange() => 480f;

        public override void GrappleRetreatSpeed(Player player, ref float speed) => speed += 4f;

        public override void GrapplePullSpeed(Player player, ref float speed) => speed += 4f;

        public override void NumGrappleHooks(Player player, ref int numHooks)
        {
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();
            numHooks = mp.GadgetCoat ? 2 : 1;
        }

        public override bool PreDrawExtras() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Color tetherColor = new Color(180, 100, 255, 0) * 0.8f;

            DrawTether(Main.spriteBatch, tetherColor);
            DrawOldPosTrail(Main.spriteBatch, tetherColor);
            DrawBodyLayers(Main.spriteBatch);

            return false;
        }

        public override void AI()
        {
            Color c = new Color(180, 100, 255, 0);

            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;

            if (Target == Vector2.Zero)
            {
                if (Projectile.owner == Main.myPlayer)
                    Target = Main.MouseWorld;
                Projectile.netUpdate = true;
            }

            Projectile.spriteDirection = 1;
            if (initialDirection == 0)
                initialDirection = Math.Sign(Projectile.velocity.X);

            Projectile.localAI[1] = 1f;
            Projectile.localAI[2]++;

            SpawnChargeDust(c);
            TryDeathBurst(c);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item94, new Vector2?(Projectile.Center), null);
            target.AddBuff(BuffID.Electrified, 60 * 3);

            if (!target.boss && target.knockBackResist > 0f && !target.dontTakeDamage)
            {
                Player owner = Main.player[Projectile.owner];

                Vector2 toPlayer = owner.Center - target.Center;
                float distance = toPlayer.Length();

                if (distance > 8f)
                {
                    toPlayer /= distance;

                    float basePull = MathHelper.Clamp(10f + distance * 0.02f, 10f, 18f);
                    float pullForce = basePull * target.knockBackResist;

                    Vector2 deltaVel = toPlayer * pullForce;

                    const float maxSpeed = 16f;
                    Vector2 newVel = target.velocity + deltaVel;
                    if (newVel.Length() > maxSpeed)
                        newVel = newVel.SafeNormalize(Vector2.Zero) * maxSpeed;

                    target.velocity = Vector2.Lerp(target.velocity, newVel, 0.75f);

                    target.netUpdate = true;
                }
            }
        }

        private void DrawTether(SpriteBatch sb, Color color)
        {
            Texture2D whitePixel = SOTSUtils.WhitePixel;

            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            float dist = Vector2.Distance(Projectile.Center, mountedCenter);

            Vector2 center = Projectile.Center;
            Vector2 headOffset = new Vector2(0f, MathF.Min(9f, dist)).RotatedBy(Projectile.rotation);
            Vector2 headPos = center + headOffset;

            Vector2 texOrigin = new Vector2(0f, 1f);

            Vector2 last = mountedCenter;
            float segs = Vector2.Distance(mountedCenter, headPos) / 10f;

            for (int i = 1; i <= segs + 1f; i++)
            {
                float t = MathF.Min(1f, i / segs);
                float wobble = (float)(0.5 + 0.5 * MathF.Cos((float)(Math.PI * i * 0.4 + SOTSWorld.GlobalCounter * 0.15f)) * MathF.Sin(t * MathF.PI));

                Vector2 side = new Vector2(12f * wobble * MathF.Sin(MathHelper.ToRadians(i * 60 + SOTSWorld.GlobalCounter * 5)), 0f).RotatedBy(Projectile.rotation);
                Vector2 p = Vector2.Lerp(mountedCenter, headPos, t);

                for (int s = -1; s <= 1; s += 2)
                {
                    Vector2 v = p + side * s - last;
                    float len = v.Length();
                    sb.Draw(whitePixel, last - Main.screenPosition, null, color, v.ToRotation(), texOrigin, new Vector2(len / 2f, 1f + wobble), SpriteEffects.None, 0f);
                    last = p + side * s;
                }
            }
        }

        private void DrawOldPosTrail(SpriteBatch sb, Color baseColor)
        {
            float swirlSpeed = 12f;
            float lifeFade = MathF.Min(Projectile.timeLeft / 50f, 1f);
            int startIndex = Math.Max(0, 50 - Projectile.timeLeft);
            float swell = Math.Min(1f, Projectile.localAI[2] / 20f);

            for (int side = -1; side <= 1; side += 2)
            {
                Vector2 last = Projectile.Center;
                for (int i = startIndex; i < Projectile.oldPos.Length && Projectile.oldPos[i] != Vector2.Zero; i++)
                {
                    float bloat = (2 + (i - startIndex)) * swell;
                    float along = 1f - (float)i / Projectile.oldPos.Length;

                    Vector2 swirl = new Vector2(bloat * side, bloat * side)
                        .RotatedBy(MathHelper.ToRadians((Projectile.localAI[2] - i) * swirlSpeed * initialDirection));

                    Vector2 p = Projectile.oldPos[i] + Projectile.Size / 2f + swirl;

                    if (i != startIndex)
                    {
                        float thickness = 1f + along;
                        Vector2 v = last - p;
                        float len = v.Length() / 2f;

                        sb.Draw(
                            SOTSUtils.WhitePixel,
                            p - Main.screenPosition,
                            null,
                            baseColor * along * lifeFade * Projectile.localAI[1] * 1.5f,
                            v.ToRotation(),
                            new Vector2(0f, 1f),
                            new Vector2(len, thickness),
                            SpriteEffects.None,
                            0f
                        );
                    }
                    last = p;
                }
            }
        }

        private void DrawBodyLayers(SpriteBatch sb)
        {
            Vector2 screenPos = Projectile.Center - Main.screenPosition;

            
            Texture2D outline = ModContent.Request<Texture2D>(Texture + "Outline", AssetRequestMode.ImmediateLoad).Value;
            Texture2D fill = ModContent.Request<Texture2D>(Texture + "Fill", AssetRequestMode.ImmediateLoad).Value;


            Vector2 origin = outline.Size() / 2f;

            Color tint = new Color(160, 80, 220, 0);

            for (int i = 0; i < 5; i++)
            {
                float ox = Main.rand.Next(-10, 11) * 0.03f;
                float oy = Main.rand.Next(-10, 11) * 0.03f;

                if (i == 0)
                    sb.Draw(fill, screenPos, null, tint * 0.5f, Projectile.rotation - 0.7853982f, origin, Projectile.scale, SpriteEffects.None, 0f);

                sb.Draw(outline, screenPos + new Vector2(ox, oy), null, tint, Projectile.rotation - 0.7853982f, origin, Projectile.scale, SpriteEffects.None, 0f);
            }
        }

        private void SpawnChargeDust(Color c)
        {
            if (Projectile.ai[0] == 0 && Projectile.localAI[2] > 1f)
            {
                PixelDust
                    .Spawn(Projectile.Center - Projectile.velocity, 0, 0, Main.rand.NextVector2Circular(3f, 3f), c, 12)
                    .scale = Main.rand.NextFloat(1.25f, 1.5f);
            }
        }

        private void TryDeathBurst(Color c)
        {
            if (Projectile.ai[0] == 2 && Projectile.localAI[0] != -1f)
            {
                for (int i = 0; i < 30; i++)
                {
                    PixelDust
                        .Spawn(Projectile.Center - Projectile.velocity, 0, 0, Main.rand.NextVector2Circular(8f, 8f), c, 12)
                        .scale = Main.rand.NextFloat(1.5f, 2f);
                }
                Projectile.localAI[0] = -1f;
            }
        }
    }
}
