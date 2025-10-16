using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using SOTS.Void;
using Microsoft.Xna.Framework;
using SOTS.Projectiles.Laser;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;

namespace SecretsOfTheSouls.Content.Projectiles.Eternity.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class PolarCannon : ModProjectile
    {
        public float FireProgress;
        public ref float Index => ref Projectile.ai[0];
        public ref float Count => ref Projectile.ai[1];
        public ref float Radius => ref Projectile.localAI[0];
        public ref float SpeedDegPerFrame => ref Projectile.localAI[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = false;
            Main.projPet[Projectile.type] = false;
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<VoidGeneric>();
            Projectile.width = Projectile.height = 26;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.alpha = 0;
            Projectile.netImportant = true;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead) { Projectile.Kill(); return; }

            Projectile.timeLeft = 2;

            float baseAngle = (float)(Main.GameUpdateCount * SpeedDegPerFrame);
            float segment = Count <= 0f ? 1f : 360f / Count;
            float angleDeg = baseAngle + Index * segment;

            Vector2 orbitCenter = owner.Center;
            float angleRad = MathHelper.ToRadians(angleDeg);
            Vector2 desiredPos = orbitCenter + angleRad.ToRotationVector2() * Radius;

            Vector2 toDesired = desiredPos - Projectile.Center;
            float maxStep = 18f;
            if (toDesired.Length() > maxStep)
                toDesired = Vector2.Normalize(toDesired) * maxStep;

            Projectile.velocity = toDesired;

            float aimRot = (Main.MouseWorld - Projectile.Center).ToRotation();
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, aimRot, 0.25f);

            Projectile.ai[2]++;

            if ((int)Projectile.ai[2] % 30 == 0 && owner.whoAmI == Main.myPlayer)
            {
                LaunchLaser(Main.MouseWorld);
                Projectile.netUpdate = true;
            }
        }

        public void LaunchLaser(Vector2 area)
        {
            Vector2 muzzle = Projectile.Center + new Vector2(12f, 0f).RotatedBy(Projectile.rotation);
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                muzzle,
                Vector2.Zero,
                ModContent.ProjectileType<BrightRedLaser>(),
                Projectile.damage,
                1f,
                Projectile.owner,
                area.X,
                area.Y,
                0f
            );
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;

            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(
                "SecretsOfTheSouls/Content/Projectiles/Eternity/SOTSEternity/PolarCannonGlow",
                AssetRequestMode.ImmediateLoad
            ).Value;

            float rot = Projectile.rotation + MathHelper.PiOver4;
            Vector2 origin = new Vector2(tex.Width * 0.5f, tex.Height * 0.5f);

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 pos = Projectile.oldPos[i] + origin - Main.screenPosition;
                pos.Y += Projectile.gfxOffY;

                float t = (Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length;
                Color trailColor = Projectile.GetAlpha(lightColor) * t;

                sb.Draw(tex, pos, null, trailColor, rot, origin, Projectile.scale, SpriteEffects.None, 0f);
                sb.Draw(glow, pos, null, Color.White * (0.5f * t), rot, origin, Projectile.scale, SpriteEffects.None, 0f);
            }

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            sb.Draw(tex, drawPos, null, lightColor, rot, origin, Projectile.scale, SpriteEffects.None, 0f);
            sb.Draw(glow, drawPos, null, Color.White, rot, origin, Projectile.scale, SpriteEffects.None, 0f);

            if (FireProgress != 0f)
            {
                Color bloom = new Color(100, 50, 60, 0) * FireProgress;
                float radius = 16f * (1f - FireProgress);

                for (int i = 0; i < 8; i++)
                {
                    float a = Projectile.rotation + i * MathHelper.PiOver4;
                    Vector2 offset = new Vector2(radius, 0f).RotatedBy(a);
                    sb.Draw(glow, drawPos + offset, null, bloom, rot, origin, Projectile.scale, SpriteEffects.None, 0f);
                }
            }

            return false;
        }
    }
}
