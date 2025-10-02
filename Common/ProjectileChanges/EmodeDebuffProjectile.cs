using System.Collections.Generic;
using FargoSoulsSOTS.Content.Buffs.Emode;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Core.Systems;
using SOTS.NPCs;
using SOTS.NPCs.Boss;
using SOTS.NPCs.Boss.Curse;
using SOTS.NPCs.TreasureSlimes;
using SOTS.Projectiles;
using SOTS.Projectiles.Earth.Glowmoth;
using SOTS.Projectiles.Laser;
using SOTS.Projectiles.Minions;
using SOTS.Projectiles.Pyramid;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Common.ProjectileChanges
{
    public class EmodeDebuffProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public List<int> slimedDebuffProjectile =
        [
            ModContent.ProjectileType<RecollectHook>(),
            ModContent.ProjectileType<PinkBullet>(),
            ModContent.ProjectileType<PinkTracer>(),
            ModContent.ProjectileType<PinkLaser>(),
        ];

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

        public List<int> putridEmodeDebuffProjectile =
        [
            //Putrid Pinky
            ModContent.ProjectileType<RecollectHook>(),
            ModContent.ProjectileType<PinkBullet>(),
            ModContent.ProjectileType<PinkTracer>(),
            ModContent.ProjectileType<PinkLaser>(),

            //Other
            ModContent.ProjectileType<SOTS.Projectiles.Minions.FluxSlimeBall>(),
        ];

        public List<int> pharoahMasoDebuffProjectile = 
        [
            ModContent.ProjectileType<CurseWave>(),
            ModContent.ProjectileType<ShadeSpear>(),
        ];

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (slimedDebuffProjectile.Contains(projectile.type) && WorldSavingSystem.EternityMode)
            {
                target.AddBuff(BuffID.Slimed, 60 * 3);
            }

            if (glowmothMasoDebuffProjectile.Contains(projectile.type) && WorldSavingSystem.MasochistModeReal)
            {
                target.AddBuff(BuffID.Poisoned, 60 * 5);
                target.AddBuff(BuffID.Darkness, 60 * 3);
            }

            if (glowmothEmodeDebuffProjectile.Contains(projectile.type) && WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Lepidopterism>(), (WorldSavingSystem.MasochistModeReal ? 5 : 3) * 60);
            }

            if (putridEmodeDebuffProjectile.Contains(projectile.type) && WorldSavingSystem.EternityMode)
            {
                target.AddBuff(ModContent.BuffType<Corrosion>(), (WorldSavingSystem.MasochistModeReal ? 5 : 3) * 60);
            }

            if (pharoahMasoDebuffProjectile.Contains(projectile.type) && WorldSavingSystem.MasochistModeReal)
            {
                target.AddBuff(BuffID.Blackout, 60 * 3);
            }
        }
    }

    public class EmodeDebuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public List<int> slimedDebuffNPC =
        [
            //Putrid Pinky
            ModContent.NPCType<PutridPinky1>(),
            ModContent.NPCType<PutridHook>(),
            ModContent.NPCType<HookTurret>(),
        ];

        public List<int> putridEmodDebuffNPC =
        [
            //Putrid Pinky
            ModContent.NPCType<PutridPinky1>(),
            ModContent.NPCType<PutridHook>(),
            ModContent.NPCType<HookTurret>(),

            //Other
            ModContent.NPCType<FluxSlime>(),
            ModContent.NPCType<SOTS.NPCs.FluxSlimeBall>(),
            ModContent.NPCType<MutagenTreasureSlime>(),
        ];

        public List<int> pharohMasoDebuffNPC =
        [
            ModContent.NPCType<SmallGas>(),
        ];

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            if (slimedDebuffNPC.Contains(npc.type) && WorldSavingSystem.EternityMode)
                target.AddBuff(BuffID.Slimed, 60 * 3);

            if (putridEmodDebuffNPC.Contains(npc.type) && WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<Corrosion>(), (WorldSavingSystem.MasochistModeReal ? 5 : 3) * 60);

            if (pharohMasoDebuffNPC.Contains(npc.type) && WorldSavingSystem.MasochistModeReal)
                target.AddBuff(BuffID.Blackout, 60 * 3);
        }
    }
}
