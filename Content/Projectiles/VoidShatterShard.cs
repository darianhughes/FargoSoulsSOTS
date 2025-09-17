using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Projectiles
{
    public class VoidShatterShard : ModProjectile
    {
        // ai[0] = index in ring (0..count-1)
        // ai[1] = total count in ring (3 or 6)
        // localAI[0] = orbit radius (pixels)
        // localAI[1] = angular speed (deg/frame)
        public ref float Index => ref Projectile.ai[0];
        public ref float Count => ref Projectile.ai[1];
        public ref float Radius => ref Projectile.localAI[0];
        public ref float SpeedDegPerFrame => ref Projectile.localAI[1];

        public override string Texture => "SOTS/Projectiles/Permafrost/ShatterShard";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;

            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            // fallback damage (spawner also sets it)
            if (Projectile.damage <= 0) Projectile.damage = 10;

            // short life; will be refreshed in AI each tick
            Projectile.timeLeft = 2;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead) { Projectile.Kill(); return; }

            Projectile.timeLeft = 2;

            float baseAngle = (float)(Main.GameUpdateCount * SpeedDegPerFrame);
            float segment = (Count <= 0f ? 1f : 360f / Count);
            float angleDeg = baseAngle + (Index * segment);

            Vector2 orbitCenter = owner.Center;
            float angleRad = MathHelper.ToRadians(angleDeg);
            Vector2 desiredPos = orbitCenter + angleRad.ToRotationVector2() * Radius;

            Vector2 toDesired = desiredPos - Projectile.Center;
            float maxStep = 18f;
            if (toDesired.Length() > maxStep)
                toDesired = Vector2.Normalize(toDesired) * maxStep;

            Projectile.velocity = toDesired;
            Projectile.rotation = (desiredPos - orbitCenter).ToRotation() + MathHelper.PiOver2;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Purple;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(tex.Width / 2f, tex.Height / 2f);
            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                Color.Purple,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0);
            return false;
        }
    }
}
