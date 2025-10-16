using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using SOTS;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core.Systems;
using Terraria.ID;
using SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs;
using SOTS.Items.Earth.Glowmoth;

namespace SecretsOfTheSouls.Content.Bosses.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class Glowmoth : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<SOTS.NPCs.Boss.Glowmoth.Glowmoth>());

        public override bool SafePreAI(NPC npc)
        {
            return true;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            if (WorldSavingSystem.MasochistModeReal)
            {
                target.AddBuff(BuffID.Poisoned, 60 * 5);
                target.AddBuff(BuffID.Darkness, 60 * 3);
            }

            target.AddBuff(ModContent.BuffType<Lepidopterism>(), (WorldSavingSystem.MasochistModeReal ? 5 : 3) * 60);
        }
    }
}
