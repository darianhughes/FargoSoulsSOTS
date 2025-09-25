using FargoSoulsSOTS.Content.Items.Summons.SOTSCopy;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using SOTS;
using Terraria.ModLoader;
using Terraria;
using SOTS.NPCs.Boss.Excavator;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargoSoulsSOTS.Content.Buffs.Emode;

namespace FargoSoulsSOTS.Content.Bosses.SOTSEternity
{
    public class Excavator : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(ModContent.NPCType<SOTS.NPCs.Boss.Excavator.Excavator>(), ModContent.NPCType<ExcavatorBody>(), ModContent.NPCType<ExcavatorBody2>(), ModContent.NPCType<ExcavatorDrillTail>(), ModContent.NPCType<ExcavatorTail>());
    }

    public class ExcavatorHead : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<SOTS.NPCs.Boss.Excavator.Excavator>());

        public bool DroppedSummon;

        public override bool SafePreAI(NPC npc)
        {
            if (npc.HasPlayerTarget && !DroppedSummon)
            {
                Player player = Main.player[npc.target];

                if (!player.dead)
                {
                    if (!SOTSWorld.downedExcavator && FargoSoulsUtil.HostCheck)
                        Item.NewItem(npc.GetSource_Loot(), player.Hitbox, ModContent.ItemType<ExcavationRemote>());

                    DroppedSummon = true;
                }
            }

            return true;
        }

        public override void SafePostAI(NPC npc)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead && npc.Distance(player.Center) < 8000f)
                {
                    player.AddBuff(ModContent.BuffType<Grounded>(), 1);
                }
            }
        }
    }
}
