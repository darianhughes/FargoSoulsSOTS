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
        private int PhaseFrames = 10;
        private float OvershootBoost = 1.0f;

        public override string Texture => $"Terraria/Images/NPC_{NPCID.ShadowFlameApparition}";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;

            Main.projFrames[Type] = Main.npcFrameCount[NPCID.ShadowFlameApparition];
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
            Projectile.localNPCHitCooldown = 12;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            if (Projectile.velocity.LengthSquared() > 0.01f)
                Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X >= 0f ? 1 : -1);
            Projectile.rotation = 0f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6) { Projectile.frameCounter = 0; Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type]; }

            int hitsDone = (int)Projectile.localAI[1];

            if (hitsDone >= MaxHits)
            {
                Projectile.alpha += 12;
                Projectile.velocity *= 0.96f;
                if (Projectile.alpha > 250) Projectile.Kill();
                return;
            }

            if (Projectile.localAI[2] > 0f)
            {
                Projectile.localAI[2]--;
                return;
            }

            int target = -1;
            float bestDist = 1200f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n.CanBeChasedBy(this))
                {
                    float d = Vector2.Distance(Projectile.Center, n.Center);
                    if (d < bestDist) { bestDist = d; target = i; }
                }
            }

            Projectile.localAI[0]++;

            if (target >= 0 && Projectile.localAI[0] >= DashCooldown)
            {
                Vector2 to = Main.npc[target].Center - Projectile.Center;
                if (to == Vector2.Zero) to = Vector2.UnitX;
                to.Normalize();
                Projectile.velocity = to * DashSpeed * (1f + OvershootBoost * 0.0f);
                Projectile.localAI[0] = 0f;
            }

            if (target < 0)
            {
                Projectile.velocity *= 0.98f;
                Projectile.alpha = (int)MathHelper.Clamp(Projectile.alpha + 3, 0, 255);
                if (Projectile.alpha >= 255) Projectile.Kill();
            }

            if (Main.rand.NextBool(4))
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, 0f, 100);
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = Projectile.velocity * 0.2f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localAI[1] = (int)Projectile.localAI[1] + 1;
            Projectile.localAI[2] = PhaseFrames;
            target.AddBuff(BuffID.ShadowFlame, 180);

            for (int i = 0; i < 8; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                                      DustID.Shadowflame,
                                      Main.rand.NextFloat(-2.5f, 2.5f),
                                      Main.rand.NextFloat(-2.5f, 2.5f),
                                      120, default, 1.1f);
                Main.dust[d].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            int frames = Main.projFrames[Type];
            int frameHeight = tex.Height / frames;
            Rectangle src = new Rectangle(0, frameHeight * Projectile.frame, tex.Width, frameHeight);

            Vector2 origin = new Vector2(src.Width * 0.5f, src.Height * 0.5f);
            SpriteEffects fx = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Color drawColor = Color.White * (1f - Projectile.alpha / 255f);

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                src,
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
