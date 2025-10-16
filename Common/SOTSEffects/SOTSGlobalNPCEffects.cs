using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.ModLoader.IO;
using SOTS.Buffs;
using SecretsOfTheSouls.Core.Players;
using SOTS.Common.GlobalNPCs;
using SOTS.NPCs.Constructs;
using SecretsOfTheSouls.Core.Systems;
using Terraria.Localization;
using SOTS.NPCs.Boss;
using SOTS.NPCs.Boss.Glowmoth;
using SOTS.Items.Earth.Glowmoth;
using SOTS.Items.Banners;
using SOTS.NPCs.Boss.Advisor;
using SOTS.Items.Planetarium;
using System.Linq;
using System.Reflection;
using System;
using SOTS.Items.Slime;
using SOTS.NPCs.Boss.Curse;
using SOTS.Items.Pyramid;
using SOTS.NPCs.Boss.Excavator;
using SOTS.Items.AbandonedVillage;
using SOTS.NPCs.Boss.Polaris.NewPolaris;
using SOTS.NPCs.Boss.Polaris;
using SOTS.Items.Permafrost;
using SOTS.NPCs.Boss.Lux;
using SOTS.Items.Chaos;
using SOTS.Items.Celestial;
using SOTS.NPCs.TreasureSlimes;
using SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs;
using SecretsOfTheSouls.Content.Items.Summons.SwarmSummons.Energizers.SOTSEnergizers;
using SecretsOfTheSouls.Content.Projectiles.Eternity.SOTSEternity;

namespace SecretsOfTheSouls.Common.SOTSEffects
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSGlobalNPCEffects : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool IsCursed;
        public int CursedOwner = -1;
        public int AttachedKeystoneId = -1;

        public static int excavatorBoss = -1;

        public void ApplyCurse(int owner, NPC npc)
        {
            IsCursed = true;
            CursedOwner = owner;
            if (Main.netMode == NetmodeID.Server)
                npc.netUpdate = true;
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.type == ModContent.NPCType<Excavator>())
                excavatorBoss = npc.whoAmI;

            return base.PreAI(npc);
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

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            base.UpdateLifeRegen(npc, ref damage);

            if (npc.HasBuff(ModContent.BuffType<AbyssalInferno>()))
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 60;

                if (damage < 30)
                    damage = 30;
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
                if (npc.type == construct && !SecretsOfTheSoulsWorldSavingSystem.downedConstruct)
                {
                    doDeviText = true;
                    SecretsOfTheSoulsWorldSavingSystem.downedConstruct = true;
                }
            }

            if (npc.ModNPC is TreasureSlime && !SecretsOfTheSoulsWorldSavingSystem.downedTreasureSlime)
            {
                doDeviText = true;
                SecretsOfTheSoulsWorldSavingSystem.downedTreasureSlime = true;
            }

            if (doDeviText && Main.netMode != NetmodeID.Server)
            {
                string seller = Language.GetTextValue($"Mods.Fargowiltas.NPCs.Deviantt.DisplayName");
                Main.NewText(Language.GetTextValue("Mods.Fargowiltas.MessageInfo.NewItemUnlocked", seller), Color.HotPink);
            }
            
            /*
            if (Fargowiltas.Fargowiltas.SwarmActive && (npc.type == ))
            {
                return false;
            }
            */

            if (npc.TryGetFargoSwarmActive(out bool swarm) && Fargowiltas.Fargowiltas.SwarmActive && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.type == ModContent.NPCType<Glowmoth>())
                    Swarm(npc, ModContent.NPCType<Glowmoth>(), ModContent.NPCType<GlowmothMinion>(), ModContent.ItemType<GlowmothBag>(), ModContent.ItemType<GlowmothTrophy>(), ModContent.ItemType<EnergizerGlowmoth>());
                if (npc.type == ModContent.NPCType<PutridPinkyPhase2>())
                    Swarm(npc, ModContent.NPCType<PutridPinkyPhase2>(), ModContent.NPCType<PutridHook>(), ModContent.ItemType<PinkyBag>(), ModContent.ItemType<PutridPinkyTrophy>(), ModContent.ItemType<EnergizerPutrid>());
                if (npc.type == ModContent.NPCType<SOTS.NPCs.Boss.Curse.PharaohsCurse>())
                    Swarm(npc, ModContent.NPCType<SOTS.NPCs.Boss.Curse.PharaohsCurse>(), ModContent.NPCType<SmallGas>(), ModContent.ItemType<CurseBag>(), ModContent.ItemType<CurseTrophy>(), ModContent.ItemType<EnergizerPharoah>());
                if (npc.type == ModContent.NPCType<Excavator>())
                    Swarm(npc, ModContent.NPCType<Excavator>(), -1, ModContent.ItemType<ExcavatorBossBag>(), ModContent.ItemType<ExcavatorTrophy>(), ModContent.ItemType<EnergizerExcavator>());
                if (npc.type == ModContent.NPCType<TheAdvisorHead>())
                    Swarm(npc, ModContent.NPCType<TheAdvisorHead>(), -1, ModContent.ItemType<TheAdvisorBossBag>(), ModContent.ItemType<AdvisorTrophy>(), ModContent.ItemType<EnergizerAdvisor>());
                if (npc.type == ModContent.NPCType<NewPolaris>() || npc.type == ModContent.NPCType<Polaris>())
                    Swarm(npc, ModContent.NPCType<NewPolaris>(), ModContent.NPCType<BulletSnakeHead>(), ModContent.ItemType<PolarisBossBag>(), ModContent.ItemType<PolarisTrophy>(), ModContent.ItemType<EnergizerPolaris>());
                if (npc.type == ModContent.NPCType<Lux>())
                    Swarm(npc, ModContent.NPCType<Lux>(), ModContent.NPCType<FakeLux>(), ModContent.ItemType<LuxBag>(), ModContent.ItemType<LuxTrophy>(), ModContent.ItemType<EnergizerLux>());
                if (npc.type == ModContent.NPCType<SubspaceSerpentHead>())
                    Swarm(npc, ModContent.NPCType<SubspaceSerpentHead>(), -1, ModContent.ItemType<SubspaceBag>(), -1, ModContent.ItemType<EnergizerSubspace>());
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
                        linger = owner.active && SOTSEffectsPlayer.KeystoneLinger(owner);
                    }

                    if (linger && p.ModProjectile is Keystone ks)
                    {
                        ks.StartLinger(SOTSEffectsPlayer.LingerTicks);
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

        private void Swarm(NPC npc, int boss, int minion, int bossbag, int trophy, int reward)
        {
            if (bossbag >= 0 && bossbag != ItemID.DefenderMedal)
            {
                int stack = Fargowiltas.Fargowiltas.SwarmItemsUsed * 5 - 1;
                if (npc.type == NPCID.CultistBoss)
                    stack += 1;
                npc.DropItemInstanced(npc.Center, npc.Size, bossbag, itemStack: stack);
            }
            else if (bossbag >= 0 && bossbag == ItemID.DefenderMedal)
            {
                npc.DropItemInstanced(npc.Center, npc.Size, bossbag, itemStack: 5 * (Fargowiltas.Fargowiltas.SwarmItemsUsed * 5 - 1));
            }

            // Drop swarm reward for every 10 items used
            if (Fargowiltas.Fargowiltas.SwarmItemsUsed >= 10 && reward > 0)
                Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, reward, Stack: Fargowiltas.Fargowiltas.SwarmItemsUsed / 10);


            //drop trophy for every 3 items
            if (Fargowiltas.Fargowiltas.SwarmItemsUsed >= 3 && trophy > 0)
                Item.NewItem(npc.GetSource_Loot(), npc.Hitbox, trophy, Stack: Fargowiltas.Fargowiltas.SwarmItemsUsed / 3);

            if (minion != -1)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == minion)
                    {
                        Main.npc[i].SimpleStrikeNPC(Main.npc[i].lifeMax, -Main.npc[i].direction, true, 0, null, false, 0, true);
                    }
                }
            }
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

    //Why is the swarm variable internal...? 
    public static class FargoBridgeExtensions
    {
        // Adjust if the real namespace differs:
        private const string FargoGlobalNpcFqn = "Fargowiltas.NPCs.FargoGlobalNPC";

        private static Type _fargoGlobalNpcType;
        private static FieldInfo _swarmActiveField;
        private static MethodInfo _getGlobalNpcGeneric;

        private static bool Init()
        {
            if (_fargoGlobalNpcType != null) return true;

            if (!ModLoader.TryGetMod("Fargowiltas", out Mod fargo))
                return false;

            // Resolve the GlobalNPC type
            _fargoGlobalNpcType = fargo.Code?.GetType(FargoGlobalNpcFqn)
                                 ?? AppDomain.CurrentDomain.GetAssemblies()
                                      .Select(a => a.GetType(FargoGlobalNpcFqn, false))
                                      .FirstOrDefault(t => t != null);
            if (_fargoGlobalNpcType == null)
                return false;

            // SwarmActive is internal instance field
            _swarmActiveField = _fargoGlobalNpcType.GetField(
                "SwarmActive",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            // Find NPC.GetGlobalNPC<T>()
            _getGlobalNpcGeneric = typeof(NPC).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(m => m.Name == "GetGlobalNPC"
                                  && m.IsGenericMethodDefinition
                                  && m.GetParameters().Length == 0);
            return _swarmActiveField != null && _getGlobalNpcGeneric != null;
        }

        private static object GetFargoGlobal(NPC npc)
        {
            return _getGlobalNpcGeneric.MakeGenericMethod(_fargoGlobalNpcType).Invoke(npc, null);
        }

        public static bool TryGetFargoSwarmActive(this NPC npc, out bool active)
        {
            active = false;
            if (npc is null || !Init()) return false;

            // npc.GetGlobalNPC<FargoGlobalNPC>()
            object g = _getGlobalNpcGeneric.MakeGenericMethod(_fargoGlobalNpcType).Invoke(npc, null);
            if (g == null) return false;

            object v = _swarmActiveField.GetValue(g);
            if (v is bool b) { active = b; return true; }
            return false;
        }

        public static bool TryGetFargoField<T>(this NPC npc, string fieldName, out T value)
        {
            value = default!;
            if (npc is null || !Init()) return false;

            var field = _fargoGlobalNpcType.GetField(fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null) return false;

            object g = _getGlobalNpcGeneric.MakeGenericMethod(_fargoGlobalNpcType).Invoke(npc, null);
            if (g == null) return false;

            object v = field.GetValue(g);
            if (v is T cast) { value = cast; return true; }
            return false;
        }

        public static bool TrySetFargoSwarmActive(this NPC npc, bool active)
        {
            if (npc is null || !Init()) return false;

            // guard against read-only fields just in case
            if (_swarmActiveField.IsInitOnly)
                return false;

            object g = GetFargoGlobal(npc);
            if (g == null) return false;

            _swarmActiveField.SetValue(g, active);
            return true;
        }

        public static bool TrySetFargoField<T>(this NPC npc, string fieldName, T value)
        {
            if (npc is null || !Init()) return false;

            var field = _fargoGlobalNpcType.GetField(fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null || field.IsInitOnly) return false;

            // Type-compat check: allow implicit assignable
            if (!field.FieldType.IsAssignableFrom(typeof(T)))
                return false;

            object g = GetFargoGlobal(npc);
            if (g == null) return false;

            field.SetValue(g, value);
            return true;
        }
    }
}
