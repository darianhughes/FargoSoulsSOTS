using System.Collections.Generic;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.CursedCoffin;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using SOTS.Void;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Common.NPCChanges
{
    public class VoidDamageNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public List<int> voidNPCs =
        [
            ModContent.NPCType<CursedSpirit>(),
            ModContent.NPCType<DeviBoss>(),
            ModContent.NPCType<ShadowChampion>(),
            ModContent.NPCType<AbomBoss>(),
            ModContent.NPCType<MutantBoss>()
        ];
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return voidNPCs.Contains(entity.type);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            int damage = 1 + npc.damage / 6;
            VoidPlayer.VoidDamage(Mod, target, damage);
        }
    }

    public class VoidDamageProjectiles : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public List<int> voidProjectiles =
        [
            //Cursed Coffin
            ModContent.ProjectileType<CoffinDarkSouls>(),
            ModContent.ProjectileType<CoffinWaveShot>(),

            //Deviantt
            ModContent.ProjectileType<DeviEnergyHeart>(),
            ModContent.ProjectileType<DeviDeathray>(),
            ModContent.ProjectileType<DeviRainHeart>(),
            ModContent.ProjectileType<DeviRainHeart2>(),
            ModContent.ProjectileType<DeviLostSoul>(),
            ModContent.ProjectileType<DeviLightBall>(),
            ModContent.ProjectileType<DeviLightBall2>(),

            //Champion of Death

            //Champion of Spirit

            //Abominationn
            ModContent.ProjectileType<AbomDeathScythe>(),
            ModContent.ProjectileType<AbomDeathray>(),
            ModContent.ProjectileType<AbomDeathraySmall>(),
            ModContent.ProjectileType<AbomDeathraySmall2>(),
            ModContent.ProjectileType<AbomPhoenix>(),
            ModContent.ProjectileType<AbomScytheFlaming>(),
            ModContent.ProjectileType<AbomScytheSpin>(),
            ModContent.ProjectileType<AbomScytheSplit>(),
            ModContent.ProjectileType<AbomSickle>(),
            ModContent.ProjectileType<AbomSickle2>(),
            ModContent.ProjectileType<AbomSickle3>(),
            ModContent.ProjectileType<AbomSickleSplit1>(),
            ModContent.ProjectileType<AbomSickleSplit2>(),
            ModContent.ProjectileType<AbomStyxGazer>(),
            ModContent.ProjectileType<AbomStyxGazerDash>(),
            ModContent.ProjectileType<AbomSword>(),
            ModContent.ProjectileType<AbomSwordHandle>(),

            //Mutant
            ModContent.ProjectileType<MutantDeathray1>(),
            ModContent.ProjectileType<MutantDeathray2>(),
            ModContent.ProjectileType<MutantDeathray3>(),
            ModContent.ProjectileType<MutantDeathraySmall>(),
            ModContent.ProjectileType<MutantEye>(),
            ModContent.ProjectileType<MutantEyeHoming>(),
            ModContent.ProjectileType<MutantEyeWavy>(),
            ModContent.ProjectileType<MutantMark1>(),
            ModContent.ProjectileType<MutantMark2>(),
            ModContent.ProjectileType<MutantSansBeam>(),
            ModContent.ProjectileType<MutantScythe1>(),
            ModContent.ProjectileType<MutantScythe2>(),
            ModContent.ProjectileType<MutantShadowHand>(),
            ModContent.ProjectileType<MutantSpearAim>(),
            ModContent.ProjectileType<MutantSpearDash>(),
            ModContent.ProjectileType<MutantSpearMagical>(),
            ModContent.ProjectileType<MutantSpearSpin>(),
            ModContent.ProjectileType<MutantSpearThrown>(),
            ModContent.ProjectileType<MutantSphereRing>(),
            ModContent.ProjectileType<MutantSphereSmall>(),
            ModContent.ProjectileType<MutantSword>(),
            ModContent.ProjectileType<MutantTrueEyeDeathray>(),
            ModContent.ProjectileType<MutantTrueEyeL>(),
            ModContent.ProjectileType<MutantTrueEyeR>(),
            ModContent.ProjectileType<MutantTrueEyeS>(),
            ModContent.ProjectileType<MutantTrueEyeSphere>(),
        ];
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return voidProjectiles.Contains(entity.type);
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            int damage = 1 + projectile.damage / 6;
            VoidPlayer.VoidDamage(Mod, target, damage);
        }
    }
}
