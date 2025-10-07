using SecretsOfTheSouls.Content.Items.Summons.SOTSCopy;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using SOTS;
using Terraria.ModLoader;
using Terraria;
using SOTS.NPCs.Boss.Polaris.NewPolaris;
using Terraria.ID;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using SOTS.NPCs.Boss.Polaris;
using SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs;

namespace SecretsOfTheSouls.Content.Bosses.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class Polaris : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<NewPolaris>());

        public bool DroppedSummon;
        public bool HasSpawnedBulletSnakes;

        public override bool SafePreAI(NPC npc)
        {
            if (npc.HasPlayerTarget && !DroppedSummon)
            {
                Player player = Main.player[npc.target];

                if (!player.dead)
                {
                    if (!SOTSWorld.downedAmalgamation && FargoSoulsUtil.HostCheck)
                        Item.NewItem(npc.GetSource_Loot(), player.Hitbox, ModContent.ItemType<PolarKey>());

                    DroppedSummon = true;
                }
            }

            if (npc.life <= npc.lifeMax / 2 && !HasSpawnedBulletSnakes && npc.HasPlayerTarget)
            {
                Player player = Main.player[npc.target];

                SOTSUtils.PlaySound(SoundID.Item119, npc.Center);
                Vector2 vector2 = npc.Center - Utils.SafeNormalize(player.Center - npc.Center, Vector2.Zero) * 1200f;
                for (int i = 0; i < (WorldSavingSystem.MasochistModeReal ? 3 : 2); i++)
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)vector2.X, (int)vector2.Y, ModContent.NPCType<BulletSnakeHead>());
            }

            return true;
        }

        public override void ModifyHitByAnything(NPC npc, Player player, ref NPC.HitModifiers modifiers)
        {
            
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);

            target.AddBuff(ModContent.BuffType<CryomagneticDisruption>(),  3 * 60);
        }
    }
}
