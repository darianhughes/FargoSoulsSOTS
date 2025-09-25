using System.Collections.Generic;
using FargoSoulsSOTS.Content.Buffs.Emode;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Core.Systems;
using SOTS.Projectiles.Earth.Glowmoth;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Common.ProjectileChanges
{
    public class EmodeDebuffProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public List<int> glowmothMasoDebuffProjectile =
        [
            //Glowmoth
            ModContent.ProjectileType<WaveBall>(),
            ModContent.ProjectileType<GlowBombOrb>(),
            ModContent.ProjectileType<GlowBombShard>(),
            ModContent.ProjectileType<GlowSparkle>(),
        ];

        public List<int> glowmothEmodeDebuffProjectile =
        [
            //Deviantt
            ModContent.ProjectileType<DeviLightBall2>(),
        ];

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (glowmothMasoDebuffProjectile.Contains(projectile.type) && WorldSavingSystem.MasochistModeReal)
            {
                target.AddBuff(BuffID.Poisoned, 60 * 5);
                target.AddBuff(BuffID.Darkness, 60 * 3);
            }

            if (glowmothEmodeDebuffProjectile.Contains(projectile.type) && WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Lepidopterism>(), WorldSavingSystem.MasochistModeReal ? 5 : 3);
            }
        }
    }
}
