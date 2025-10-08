using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Terraria.ModLoader;
using Terraria;
using SOTS.NPCs.Boss.Curse;
using FargowiltasSouls.Core.Systems;
using Terraria.ID;

namespace SecretsOfTheSouls.Content.Bosses.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class PharoahsCurse : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<PharaohsCurse>());

        public override bool SafePreAI(NPC npc)
        {
            return true;
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            if (WorldSavingSystem.MasochistModeReal)
            {
                target.AddBuff(BuffID.Blackout, 60 * 3);
            }
        }
    }
}
