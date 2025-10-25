using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Projectiles.Eternity.ConsolariaEternity
{
    public class ShadowflameApparitionProj : ModProjectile
    {
        private int MaxHits = 3;
        private float DashSpeed = 18f;
        private int DashCooldown = 18;
        private float ArrivalDistance = 18f;

        public override string Texture => $"Terraria/NPC_{NPCID.ShadowFlameApparition}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            int target = -1;
            float bestDist = 1200f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this) && !npc.friendly && npc.active)
                {
                    float dist = Vector2.Distance(Projectile.Center, npc.Center);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        target = i;
                    }
                }
            }

            int hitsDone = (int)Projectile.localAI[1];

            if (hitsDone >= MaxHits)
            {
                Projectile.alpha += 12;
                Projectile.velocity *= 0.92f;
                if (Projectile.alpha > 250)
                    Projectile.Kill();
                return;
            }

            Projectile.localAI[0]++;

            if (target >= 0)
            {
                NPC npcTarget = Main.npc[target];

                if (Projectile.localAI[0] >= DashCooldown)
                {
                    Vector2 toTarget = npcTarget.Center - Projectile.Center;
                    if (toTarget == Vector2.Zero) toTarget = new Vector2(0.001f, 0);
                    Projectile.velocity = Vector2.Normalize(toTarget) * DashSpeed;
                    Projectile.localAI[0] = 0f;
                }

                if (Vector2.Distance(Projectile.Center, npcTarget.Center) < ArrivalDistance)
                {
                    Projectile.velocity *= 0.6f;
                }
            }
            else
            {
                Projectile.velocity *= 0.96f;
                Projectile.alpha += 4;
                if (Projectile.alpha > 255) Projectile.Kill();
            }

            if (Main.rand.NextBool(3))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1.0f);
                Main.dust[d].noGravity = true;
            }

            if (Projectile.velocity.LengthSquared() > 0.01f)
                Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X >= 0f ? 1 : -1);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localAI[1] = (float)((int)Projectile.localAI[1] + 1);

            target.AddBuff(BuffID.ShadowFlame, 60 * 3);

            // knock projectile away a bit so it can re-target or re-dash
            Projectile.velocity = Vector2.Normalize(Projectile.Center - target.Center) * 6f;

            // visual burst
            for (int i = 0; i < 8; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 100, default, 1.2f);
                Main.dust[d].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 origin = new Vector2(tex.Width, tex.Height) / 2f;

            SpriteEffects fx = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color drawColor = Color.White * (1f - Projectile.alpha / 255f);

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                drawColor,
                0f,
                origin,
                Projectile.scale,
                fx,
                0
            );
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (!target.active || target.friendly || target.dontTakeDamage) return false;
            return base.CanHitNPC(target);
        }
    }
}
