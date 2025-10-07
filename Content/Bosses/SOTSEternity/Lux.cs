using SecretsOfTheSouls.Content.Items.Summons.SOTSCopy;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using SOTS;
using Terraria.ModLoader;
using Terraria;

namespace SecretsOfTheSouls.Content.Bosses.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class Lux : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<SOTS.NPCs.Boss.Lux.Lux>());

        public bool DroppedSummon;

        public override bool SafePreAI(NPC npc)
        {
            if (npc.HasPlayerTarget && !DroppedSummon)
            {
                Player player = Main.player[npc.target];

                if (!player.dead)
                {
                    if (!SOTSWorld.downedLux && FargoSoulsUtil.HostCheck)
                        Item.NewItem(npc.GetSource_Loot(), player.Hitbox, ModContent.ItemType<ChaosLure>());

                    DroppedSummon = true;
                }
            }

            return true;
        }
    }
}
