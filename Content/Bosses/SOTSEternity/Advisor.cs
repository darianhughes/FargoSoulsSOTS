using SecretsOfTheSouls.Content.Items.Summons.SOTSCopy;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using SOTS;
using Terraria.ModLoader;
using Terraria;
using SOTS.NPCs.Boss.Advisor;

namespace SecretsOfTheSouls.Content.Bosses.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class Advisor : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<TheAdvisorHead>());

        public bool DroppedSummon;

        public override bool SafePreAI(NPC npc)
        {
            if (npc.HasPlayerTarget && !DroppedSummon && npc.boss)
            {
                Player player = Main.player[npc.target];

                if (!player.dead)
                {
                    if (!SOTSWorld.downedAdvisor && FargoSoulsUtil.HostCheck)
                        Item.NewItem(npc.GetSource_Loot(), player.Hitbox, ModContent.ItemType<OldCRTTV>());

                    DroppedSummon = true;
                }
            }

            return true;
        }
    }
}
