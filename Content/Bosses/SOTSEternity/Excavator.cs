using SecretsOfTheSouls.Content.Items.Summons.SOTSCopy;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using SOTS;
using Terraria.ModLoader;
using Terraria;
using SOTS.NPCs.Boss.Excavator;
using SecretsOfTheSouls.Content.Buffs.Emode;
using FargowiltasSouls.Content.Buffs.Eternity;

namespace SecretsOfTheSouls.Content.Bosses.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class Excavator : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(ModContent.NPCType<SOTS.NPCs.Boss.Excavator.Excavator>(), ModContent.NPCType<ExcavatorBody>(), ModContent.NPCType<ExcavatorBody2>(), ModContent.NPCType<ExcavatorDrillTail>(), ModContent.NPCType<ExcavatorTail>());
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
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

            if (Main.LocalPlayer.active && !Main.LocalPlayer.ghost && !Main.LocalPlayer.dead && npc.Distance(Main.LocalPlayer.Center) < 2000)
                Main.LocalPlayer.AddBuff(ModContent.BuffType<LowGroundBuff>(), 2);

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
