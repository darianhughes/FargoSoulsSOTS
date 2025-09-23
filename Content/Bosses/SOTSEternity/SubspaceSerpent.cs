using FargoSoulsSOTS.Content.Items.Summons.SOTSCopy;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using SOTS;
using SOTS.NPCs.Boss;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Bosses.SOTSEternity
{
    public class SubspaceSerpent : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(ModContent.NPCType<SOTS.NPCs.Boss.SubspaceSerpentHead>(), ModContent.NPCType<SubspaceSerpentBody>(), ModContent.NPCType<SubspaceSerpentTail>()); 
    }

    public class SubspaceSerpentHead : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<SOTS.NPCs.Boss.SubspaceSerpentHead>());

        public bool DroppedSummon;

        public override bool SafePreAI(NPC npc)
        {
            if (npc.HasPlayerTarget && !DroppedSummon)
            {
                Player player = Main.player[npc.target];

                if (!player.dead)
                {
                    if (!SOTSWorld.downedSubspace && FargoSoulsUtil.HostCheck)
                        Item.NewItem(npc.GetSource_Loot(), player.Hitbox, ModContent.ItemType<CatalystDynamite>());

                    DroppedSummon = true;
                }
            }

            return true;
        }
    }
}
