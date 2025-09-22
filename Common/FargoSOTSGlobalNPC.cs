using FargoSoulsSOTS.Content.Buffs;
using FargoSoulsSOTS.Content.Projectiles.Masomode;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.ModLoader.IO;
using SOTS.Buffs;
using FargoSoulsSOTS.Core.Players;
using SOTS.Common.GlobalNPCs;
using SOTS.NPCs.Constructs;
using FargoSoulsSOTS.Core.Systems;
using Terraria.Localization;

namespace FargoSoulsSOTS.Common
{
    public class FargoSOTSGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool IsCursed;
        public int CursedOwner = -1;
        public int AttachedKeystoneId = -1;

        public void ApplyCurse(int owner, NPC npc)
        {
            IsCursed = true;
            CursedOwner = owner;
            if (Main.netMode == NetmodeID.Server)
                npc.netUpdate = true;
        }

        public override void AI(NPC npc)
        {
            if (!IsCursed)
                return;

            npc.AddBuff(ModContent.BuffType<CursedVision>(), 2);

            if (!PlayerStillValid(CursedOwner))
            {
                ClearCurse(npc);
                return;
            }

            // Keep keystone alive while cursed
            if (AttachedKeystoneId >= 0 && Main.projectile.IndexInRange(AttachedKeystoneId))
            {
                var p = Main.projectile[AttachedKeystoneId];
                if (p.active && p.type == ModContent.ProjectileType<Keystone>())
                    p.timeLeft = 2;
            }
            else
            {
                // If somehow missing, silently respawn it (owner client spawns; server-safe fallback)
                if (Main.myPlayer == CursedOwner && npc.active)
                {
                    int proj = Projectile.NewProjectile(
                        new EntitySource_Misc("CursedVisionKeystoneReattach"),
                        npc.Center - new Vector2(0f, npc.height * 0.9f),
                        Vector2.Zero,
                        ModContent.ProjectileType<Keystone>(),
                        0, 0f, CursedOwner, npc.whoAmI
                    );
                    AttachedKeystoneId = proj;
                }
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];

            if (projectile.type == ModContent.ProjectileType<CodeBurst>())
            {
                DebuffNPC debuffNPC = npc.GetGlobalNPC<DebuffNPC>();
                if (debuffNPC.DestableCurse < 20)
                    ++debuffNPC.DestableCurse;
                if (Main.myPlayer == player.whoAmI && Main.netMode == NetmodeID.MultiplayerClient)
                    debuffNPC.SendClientChanges(player, npc);
            }
        }

        public override bool PreKill(NPC npc)
        {
            bool doDeviText = false;

            int[] constructTypes =
            {
                ModContent.NPCType<EarthenSpirit>(),
                ModContent.NPCType<NatureSpirit>(),
                ModContent.NPCType<TidalSpirit>(),
                ModContent.NPCType<EvilSpirit>(),
                ModContent.NPCType<InfernoSpirit>(),
                ModContent.NPCType<PermafrostSpirit>(),
                ModContent.NPCType<ChaosSpirit>(),
            };

            foreach (int construct in constructTypes)
            {
                if (npc.type == construct && !FargoSoulsSOTSWorldSavingSystem.downedConstruct)
                {
                    doDeviText = true;
                    FargoSoulsSOTSWorldSavingSystem.downedConstruct = true;
                }
            }

            if (doDeviText && Main.netMode != NetmodeID.Server)
            {
                string seller = Language.GetTextValue($"Mods.Fargowiltas.NPCs.Deviantt.DisplayName");
                Main.NewText(Language.GetTextValue("Mods.Fargowiltas.MessageInfo.NewItemUnlocked", seller), Color.HotPink);
            }

            return base.PreKill(npc);
        }

        public override void OnKill(NPC npc)
        {
            if (IsCursed)
            {
                ClearCurse(npc);
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            if (IsCursed)
            {
                target.AddBuff(ModContent.BuffType<VoidBurn>(), 60 * 5);
            }
        }

        private static bool PlayerStillValid(int ownerId)
        {
            if (!Main.player.IndexInRange(ownerId)) return false;
            Player p = Main.player[ownerId];
            return p.active && !p.dead;
        }

        private void ClearCurse(NPC npc)
        {
            IsCursed = false;
            int ownerId = CursedOwner;
            CursedOwner = -1;

            if (AttachedKeystoneId >= 0 && Main.projectile.IndexInRange(AttachedKeystoneId))
            {
                var p = Main.projectile[AttachedKeystoneId];
                if (p.active && p.type == ModContent.ProjectileType<Keystone>())
                {
                    bool linger = false;
                    if (Main.player.IndexInRange(ownerId))
                    {
                        Player owner = Main.player[ownerId];
                        linger = owner.active && FargoSOTSPlayer.KeystoneLinger(owner);
                    }

                    if (linger && p.ModProjectile is Keystone ks)
                    {
                        ks.StartLinger(FargoSOTSPlayer.LingerTicks);
                        p.netUpdate = true;
                    }
                    else
                    {
                        p.Kill();
                    }
                }
            }

            AttachedKeystoneId = -1;

            if (Main.netMode == NetmodeID.Server)
                npc.netUpdate = true;
        }

        // --- Net sync for MP ---
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(IsCursed);
            binaryWriter.Write((short)CursedOwner);
            binaryWriter.Write((short)AttachedKeystoneId);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            IsCursed = bitReader.ReadBit();
            CursedOwner = binaryReader.ReadInt16();
            AttachedKeystoneId = binaryReader.ReadInt16();
        }
    }
}
