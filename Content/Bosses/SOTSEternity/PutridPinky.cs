using SecretsOfTheSouls.Content.Items.Summons.SOTSCopy;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using SOTS;
using Terraria.ModLoader;
using Terraria;
using SOTS.NPCs.Boss;
using FargowiltasSouls.Core.Systems;
using Terraria.ID;
using SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs;

namespace SecretsOfTheSouls.Content.Bosses.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class PutridPinky : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<PutridPinkyPhase2>());

        public override bool SafePreAI(NPC npc)
        {
            return true;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(BuffID.Slimed, 60 * 3);
            target.AddBuff(ModContent.BuffType<Corrosion>(), (WorldSavingSystem.MasochistModeReal ? 5 : 3) * 60);
        }
    }
}
