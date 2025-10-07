using SOTS.Void;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SOTS.NPCs.Boss.Curse;
using SOTS;

namespace FargoSoulsSOTS.Content.Projectiles.Masomode.SOTSEternity
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class CursedExplosion : ModProjectile
    {
        private const int Radius = 160;
        private const int LifetimeTicks = 2;
        private const int LocalNPCCooldown = 10;
        private const int VoidHealPerHit = 6;

        public override string Texture => "SOTS/Projectiles/Pyramid/CursedStab";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = LifetimeTicks;
            Projectile.alpha = 0;
            Projectile.hide = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = LocalNPCCooldown;

            Projectile.DamageType = ModContent.GetInstance<VoidGeneric>();
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor) => false;

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1;

                SoundEngine.PlaySound(SoundID.Item62 with { Pitch = 0.4f, Volume = 0.9f }, Projectile.Center);

                CreateBurstVisuals();
            }
        }

        private void CreateBurstVisuals()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active) return;

            var sotsPlayer = SOTSPlayer.ModPlayer(owner);
            if (sotsPlayer?.foamParticleList1 == null) return;

            for (int i1 = 0; i1 < 3; ++i1)
            {
                float num1 = 1f - i1 * 0.1f;
                for (int i2 = 0; i2 < 30; ++i2)
                {
                    float num2 = Main.rand.NextFloat(0.877f, 1.33f) * num1;
                    Vector2 velocity =
                        new Vector2(0f, -Main.rand.NextFloat(3.5f, 12f) * num2)
                        .RotatedBy(MathHelper.ToRadians(i2 * 12f))
                        + Projectile.velocity * 0.5f;

                    sotsPlayer.foamParticleList1.Add(new CurseFoam(Projectile.Center, velocity, 1.25f / num2, true));
                }
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox = new Rectangle(
                (int)(Projectile.Center.X - Radius / 2f),
                (int)(Projectile.Center.Y - Radius / 2f),
                Radius,
                Radius
            );
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var player = Main.player[Projectile.owner];
            if (!player.active || target.friendly || target.immortal || damageDone <= 0) return;

            int newLife = System.Math.Min(player.statLife + (int)(damageDone * 0.25f), player.statLifeMax2);
            int delta = newLife - player.statLife;
            if (delta > 0)
            {
                player.statLife = newLife;
                player.HealEffect(delta, broadcast: true);
            }

            var vp = player.GetModPlayer<VoidPlayer>();
            vp.voidMeter += VoidHealPerHit;
            VoidPlayer.VoidEffect(player, VoidHealPerHit);
        }
    }
}
