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
using SecretsOfTheSouls.Core.SecretsofTheSoulsUtils;
using System.Runtime.Versioning;

namespace SecretsOfTheSouls.Content.Bosses.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class Polaris : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<NewPolaris>());

        public bool HasSpawnedBulletSnakes;
        public bool HasSpawnedBulletSnakes75;
        public bool HasSpawnedBulletSnakes25;

        public override bool SafePreAI(NPC npc)
        {
            if (WorldSavingSystem.MasochistModeReal)
            {
                if (npc.life <= npc.lifeMax * 0.75 && !HasSpawnedBulletSnakes75 && npc.HasPlayerTarget)
                {
                    Player player = Main.player[npc.target];

                    SOTSUtils.PlaySound(SoundID.Item119, npc.Center);
                    Vector2 vector2NPC1 = GetRandomSpawnPos(npc, player);
                    Vector2 vector2NPC2 = GetRandomSpawnPos(npc, player);
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)vector2NPC1.X, (int)vector2NPC1.Y, ModContent.NPCType<BulletSnakeHead>());
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)vector2NPC2.X, (int)vector2NPC2.Y, ModContent.NPCType<BulletSnakeHead>());

                    HasSpawnedBulletSnakes75 = true;
                }
            }

            if (npc.life <= npc.lifeMax / 2 && !HasSpawnedBulletSnakes && npc.HasPlayerTarget)
            {
                Player player = Main.player[npc.target];

                SOTSUtils.PlaySound(SoundID.Item119, npc.Center);
                if (WorldSavingSystem.MasochistModeReal)
                {
                    Vector2 vector2 = GetRandomSpawnPos(npc, player);
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)vector2.X, (int)vector2.Y, ModContent.NPCType<BulletSnakeHead>());
                }

                Vector2 vector2NPC1 = GetRandomSpawnPos(npc, player);
                Vector2 vector2NPC2 = GetRandomSpawnPos(npc, player);
                NPC.NewNPC(npc.GetSource_FromAI(), (int)vector2NPC1.X, (int)vector2NPC1.Y, ModContent.NPCType<BulletSnakeHead>());
                NPC.NewNPC(npc.GetSource_FromAI(), (int)vector2NPC2.X, (int)vector2NPC2.Y, ModContent.NPCType<BulletSnakeHead>());

                HasSpawnedBulletSnakes = true;
            }

            if (npc.life <= npc.lifeMax / 4 && !HasSpawnedBulletSnakes25 && npc.HasPlayerTarget)
            {
                Player player = Main.player[npc.target];

                SOTSUtils.PlaySound(SoundID.Item119, npc.Center);
                if (WorldSavingSystem.MasochistModeReal)
                {
                    Vector2 vector2 = GetRandomSpawnPos(npc, player);
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)vector2.X, (int)vector2.Y, ModContent.NPCType<BulletSnakeHead>());
                }

                Vector2 vector2NPC1 = GetRandomSpawnPos(npc, player);
                Vector2 vector2NPC2 = GetRandomSpawnPos(npc, player);
                Vector2 vector2NPC3 = GetRandomSpawnPos(npc, player);
                NPC.NewNPC(npc.GetSource_FromAI(), (int)vector2NPC1.X, (int)vector2NPC1.Y, ModContent.NPCType<BulletSnakeHead>());
                NPC.NewNPC(npc.GetSource_FromAI(), (int)vector2NPC2.X, (int)vector2NPC2.Y, ModContent.NPCType<BulletSnakeHead>());
                NPC.NewNPC(npc.GetSource_FromAI(), (int)vector2NPC3.X, (int)vector2NPC3.Y, ModContent.NPCType<BulletSnakeHead>());

                HasSpawnedBulletSnakes25 = true;
            }

            if (NPC.AnyNPCs(ModContent.NPCType<BulletSnakeHead>()) && !WorldSavingSystem.MasochistModeReal)
            {
                npc.dontTakeDamage = true;
                npc.hide = true;

                NPCHelpers.FollowPlayer(npc, Main.player[npc.target]);

                return false;
            }
            else
            {
                npc.dontTakeDamage = false;
                npc.hide = false;
            }


            return true;
        }

        public override void ModifyHitByAnything(NPC npc, Player player, ref NPC.HitModifiers modifiers)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<BulletSnakeHead>()))
            {
                modifiers.FinalDamage *= 0.1f;
            }
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<BulletSnakeHead>()))
                return false;

            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(npc, target, hurtInfo);
            target.AddBuff(ModContent.BuffType<CryomagneticDisruption>(),  3 * 60);
        }

        private static Vector2 GetRandomSpawnPos(NPC npc, Player player)
        {
            return npc.Center - Utils.SafeNormalize(player.Center - npc.Center, Vector2.Zero) * Main.rand.Next(1000, 1401);
        }
    }
}
