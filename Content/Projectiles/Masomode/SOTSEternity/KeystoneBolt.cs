using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using SOTS.Void;
using Microsoft.Xna.Framework;

namespace FargoSoulsSOTS.Content.Projectiles.Masomode.SOTSEternity
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class KeystoneBolt : ModProjectile
    {
        public ref float HostNpcId => ref Projectile.ai[0]; // the cursed host to avoid

        public override void SetDefaults()
        {
            Projectile.width = 3;
            Projectile.height = 3;

            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<VoidMagic>();
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;

            Projectile.scale = 1f / 3f;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.35f);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Dust.NewDustDirect(Projectile.position, 1, 1, DustID.PurpleCrystalShard, 0f, 0f, 120, default, 0.7f);
        }

        public override bool? CanHitNPC(NPC target)
        {
            // Never hit the host carrying the keystone
            if (target.whoAmI == (int)HostNpcId)
                return false;
            return base.CanHitNPC(target);
        }
    }
}
