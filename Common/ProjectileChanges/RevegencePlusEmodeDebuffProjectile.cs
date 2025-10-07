using System.Collections.Generic;
using Terraria.ModLoader;
using RevengeancePlus.Projectiles;
using FargowiltasSouls.Core.Systems;
using Terraria.ID;
using Terraria;
using FargoSoulsSOTS.Content.Buffs.Emode.SOTSBuffs;

namespace FargoSoulsSOTS.Common.ProjectileChanges
{
    [ExtendsFromMod(FargoSOTSCrossmod.RevengeancePlus.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.RevengeancePlus.Name)]
    public class RevegencePlusEmodeDebuffProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public List<int> slimedDebuffProjectile =
        [
            //Putrid Pinky
            ModContent.ProjectileType<PinkBomb>(),
            ModContent.ProjectileType<PinkHelix>(),
            ModContent.ProjectileType<PinkPellet>(),
            ModContent.ProjectileType<PinkSplat>()
        ];

        public List<int> putridEmodeDebuffProjectile =
        [
            //Putrid Pinky
            ModContent.ProjectileType<PinkBomb>(),
            ModContent.ProjectileType<PinkHelix>(),
            ModContent.ProjectileType<PinkPellet>(),
            ModContent.ProjectileType<PinkSplat>()
        ];

        public List<int> pharoahMasoDebuffProjectile =
        [
            ModContent.ProjectileType<CurseShockwave>(),
        ];

        public List<int> advisorEmodeDebuffProjectile =
        [
            ModContent.ProjectileType<LesserPhaseBolt>(),
        ];

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (ModLoader.HasMod("SOTS"))
            {
                if (slimedDebuffProjectile.Contains(projectile.type) && WorldSavingSystem.EternityMode)
                {
                    target.AddBuff(BuffID.Slimed, 60 * 3);
                }

                if (putridEmodeDebuffProjectile.Contains(projectile.type) && WorldSavingSystem.EternityMode)
                {
                    target.AddBuff(ModContent.BuffType<Corrosion>(), (WorldSavingSystem.MasochistModeReal ? 5 : 3) * 60);
                }

                if (pharoahMasoDebuffProjectile.Contains(projectile.type) && WorldSavingSystem.MasochistModeReal)
                {
                    target.AddBuff(BuffID.Blackout, 60 * 3);
                }

                if (advisorEmodeDebuffProjectile.Contains(projectile.type) && WorldSavingSystem.EternityMode)
                {
                    target.AddBuff(BuffID.Electrified, (WorldSavingSystem.MasochistModeReal ? 5 : 3) * 60);
                }
            }
        }
    }
}
