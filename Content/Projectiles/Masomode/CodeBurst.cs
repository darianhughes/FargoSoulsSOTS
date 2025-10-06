using FargowiltasSouls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SOTS;
using SOTS.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Projectiles.Masomode
{
    public class CodeBurst : ModProjectile
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/GlowRingHollow";
        public const int Duration = 20;
        public const int BaseRadius = 52;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = BaseRadius;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Duration;
            Projectile.tileCollide = false;
            Projectile.light = 0.75f;
            Projectile.ignoreWater = true;

            AIType = ProjectileID.Bullet;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
            Projectile.FargoSouls().CanSplit = false;

            Projectile.scale = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SOTSUtils.PlaySound(SoundID.Item94, Projectile.Center.X, Projectile.Center.Y, 0.25f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.Distance(FargoSoulsUtil.ClosestPointInHitbox(targetHitbox, Projectile.Center)) < projHitbox.Width / 2;
        }

        public static readonly Color twilightColor = new(77, 200, 193);
        public override Color? GetAlpha(Color lightColor)
        {
            return twilightColor * Projectile.Opacity * (Main.mouseTextColor / 255f) * 0.9f;
        }

        public override void AI()
        {
            int num = ModContent.DustType<CodeDust2>();
            for (int index1 = 0; index1 < Main.rand.Next(2) + 1; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(Projectile.Center.X - 4f, Projectile.Center.Y - 4f), 4, 4, num, 0.0f, 0.0f, 0, new Color(), 1f);
                Dust dust = Main.dust[index2];
                dust.velocity *= 0.6f;
                dust.velocity += Projectile.velocity * 0.1f;
                dust.noGravity = true;
                dust.scale *= 1.75f;
            }

            Projectile.position = Projectile.Center;
            Projectile.scale += 5f / Duration;
            Projectile.width = Projectile.height = (int)(BaseRadius * Projectile.scale);
            Projectile.Center = Projectile.position;
            if (Projectile.timeLeft < 8)
                Projectile.Opacity -= 0.15f;
        }

        public override void OnKill(int timeLeft)
        {
            for (int index1 = 0; index1 < 30; ++index1)
            {
                int num = ModContent.DustType<CodeDust2>();
                int index2 = Dust.NewDust(new Vector2(((Entity)this.Projectile).position.X - 4f, ((Entity)this.Projectile).position.Y - 4f), ((Entity)this.Projectile).width, ((Entity)this.Projectile).height, num, 0.0f, 0.0f, 0, new Color(), 1f);
                Dust dust = Main.dust[index2];
                dust.velocity *= 1.3f;
                dust.velocity += Projectile.velocity * 0.3f;
                dust.noGravity = true;
                dust.scale *= 2.75f;
            }
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            float rotation = Projectile.rotation;
            Vector2 drawPos = Projectile.Center;
            var texture = TextureAssets.Projectile[Projectile.type].Value;

            int sizeY = texture.Height / Main.projFrames[Projectile.type];
            int frameY = Projectile.frame * sizeY;
            Rectangle rectangle = new(0, frameY, texture.Width, sizeY);
            Vector2 origin = rectangle.Size() / 2f;
            float scaleModifier = (float)BaseRadius / sizeY;
            SpriteEffects spriteEffects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(lightColor),
                    rotation, origin, Projectile.scale * scaleModifier, spriteEffects, 0);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Knockback *= 4;
            modifiers.DisableCrit();
        }
    }
}
