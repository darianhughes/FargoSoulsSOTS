using SecretsOfTheSouls.Content.Items.Summons.SOTSCopy;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using SOTS;
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

        public bool DroppedSummon;

        public override bool SafePreAI(NPC npc)
        {
            if (npc.HasPlayerTarget && !DroppedSummon)
            {
                Player player = Main.player[npc.target];

                if (!player.dead)
                {
                    if (!SOTSWorld.downedCurse && FargoSoulsUtil.HostCheck)
                        Item.NewItem(npc.GetSource_Loot(), player.Hitbox, ModContent.ItemType<CursedScroll>());

                    DroppedSummon = true;
                }
            }

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
