using Terraria.ModLoader;
using Terraria;
using SOTS.NPCs.Boss.Glowmoth;
using SOTS.NPCs.Boss;
using SOTS.NPCs.Boss.Curse;
using SOTS.NPCs.Boss.Excavator;
using SOTS.NPCs.Boss.Advisor;
using SOTS.NPCs.Boss.Polaris;
using SOTS.NPCs.Boss.Polaris.NewPolaris;
using SOTS.NPCs.Boss.Lux;
using Microsoft.Xna.Framework;
using System;
using Terraria.ID;

namespace SecretsOfTheSouls.Common.EnergizedGlobalNPCs
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSEnergizedGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool SwarmHealth = false;

        internal static int[] SOTSBosses = [
            ModContent.NPCType<Glowmoth>(),
            ModContent.NPCType<PutridPinkyPhase2>(),
            ModContent.NPCType<PharaohsCurse>(),
            //ModContent.NPCType<Excavator>(),
            ModContent.NPCType<TheAdvisorHead>(),
            ModContent.NPCType<Polaris>(),
            ModContent.NPCType<NewPolaris>(),
            ModContent.NPCType<Lux>(),
            //ModContent.NPCType<SubspaceSerpentHead>(),
        ];

        public override void SetDefaults(NPC npc)
        {
            const int k = 1000;
            const int m = k * k;
            int baseHealth = 28 * k;
            int baseHealthHM = 160 * k;
            bool validBoss = true;

            if (Fargowiltas.Fargowiltas.SwarmSetDefaults)
            {
                if (npc.type == ModContent.NPCType<Glowmoth>())
                    npc.lifeMax = baseHealth;

                else if (npc.type == ModContent.NPCType<PutridPinkyPhase2>())
                    npc.lifeMax = baseHealth;

                else if (npc.type == ModContent.NPCType<PharaohsCurse>())
                    npc.lifeMax = baseHealth;

                else if (npc.type == ModContent.NPCType<Excavator>())
                    npc.lifeMax = baseHealth;

                else if (npc.type == ModContent.NPCType<TheAdvisorHead>())
                    npc.lifeMax = baseHealth;

                else if (npc.type == ModContent.NPCType<Polaris>() || npc.type == ModContent.NPCType<NewPolaris>())
                {
                    npc.lifeMax = baseHealthHM;
                    Fargowiltas.Fargowiltas.HardmodeSwarmActive = true;
                }

                else if (npc.type == ModContent.NPCType<Lux>())
                {
                    npc.lifeMax = baseHealthHM;
                    Fargowiltas.Fargowiltas.HardmodeSwarmActive = true;
                    Fargowiltas.Fargowiltas.LateHardmodeSwarmActive = true;
                }

                else if (npc.type == ModContent.NPCType<SubspaceSerpentHead>())
                {
                    npc.lifeMax = baseHealthHM;
                    Fargowiltas.Fargowiltas.HardmodeSwarmActive = true;
                    Fargowiltas.Fargowiltas.LateHardmodeSwarmActive = true;
                }

                else
                    validBoss = false;
            }
            else
                validBoss = false;

            if (Fargowiltas.Fargowiltas.SwarmActive)
            {
                if (!validBoss)
                {
                    validBoss = true;

                    if (npc.type == ModContent.NPCType<PutridHook>())
                        npc.lifeMax = (int)(0.4 * k);

                    else if (npc.type == ModContent.NPCType<HookTurret>())
                        npc.lifeMax = (int)(0.2 * k);

                    else
                        validBoss = false;
                }

                if (validBoss && Fargowiltas.Fargowiltas.SwarmItemsUsed > 1)
                {
                    npc.lifeMax *= Fargowiltas.Fargowiltas.SwarmItemsUsed;
                    SwarmHealth = true;
                }

                int minDamage = Fargowiltas.Fargowiltas.SwarmMinDamage * 2;
                if (!npc.townNPC && npc.lifeMax > 10 && npc.damage > 0 && npc.damage < minDamage)
                    npc.damage = minDamage;
            }
        }

        private int go = 1;
        public override bool PreAI(NPC npc)
        {
            if (Fargowiltas.Fargowiltas.SwarmNoHyperActive)
                return true;
            if (Fargowiltas.Fargowiltas.LateHardmodeSwarmActive && Main.GameUpdateCount % 3 == 0)
                return true;
            if (Fargowiltas.Fargowiltas.HardmodeSwarmActive && Main.GameUpdateCount % 2 == 0)
                return true;

            if (Fargowiltas.Fargowiltas.SwarmActive && npc.type == ModContent.NPCType<PharaohsCurse>() && go < 2)
            {
                go++;
                npc.AI();
                float speedToRemove = -0.25f; //Pharaohs Curse moves too fast with the base swarm scaling, causing attacks to overlap.
                Vector2 newPos = npc.position + npc.velocity * speedToRemove;
                if (!Collision.SolidCollision(newPos, npc.width, npc.height))
                {
                    npc.position = newPos;
                }
            }

            return true;
        }

        private bool _logged;
        public override void PostAI(NPC npc)
        {
            if (go == 2)
            {
                go = 1;
            }

            if (!Fargowiltas.Fargowiltas.SwarmActive) return;

            if (SotsIds.AdvisorType == -1) return;
            if (npc.type != SotsIds.AdvisorType) return;

            if (!_logged)
            {
                _logged = true;
                if (Main.netMode != NetmodeID.Server)
                    Main.NewText($"[Swarm] Scaling Advisor (id {npc.whoAmI})", 175, 75, 255);
            }

            const int k = 1000;
            int desiredLifeMax = 28 * k * Math.Max(1, Fargowiltas.Fargowiltas.SwarmItemsUsed);

            if (npc.lifeMax != desiredLifeMax)
            {
                float ratio = npc.lifeMax > 0 ? npc.life / (float)npc.lifeMax : 1f;
                npc.lifeMax = desiredLifeMax;
                npc.life = Math.Clamp((int)(desiredLifeMax * ratio), 1, desiredLifeMax);
                npc.netUpdate = true;
            }

            int minDamage = Fargowiltas.Fargowiltas.SwarmMinDamage * 2;
            if (!npc.townNPC && npc.damage > 0 && npc.damage < minDamage)
                npc.damage = minDamage;

            /*
            if (!Fargowiltas.Fargowiltas.SwarmNoHyperActive)
                npc.velocity *= 1.08f;
            */

            npc.boss = true;
            npc.dontTakeDamage = false;
            npc.dontCountMe = false;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public static class SotsIds
    {
        public static int AdvisorType = -1;

        public static void ResolveIds()
        {
            if (AdvisorType != -1) return;
            if (!ModLoader.TryGetMod("SOTS", out var sots)) return;
            if (sots.TryFind<ModNPC>("TheAdvisorHead", out var m))
                AdvisorType = m.Type;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class ResolverSystem : ModSystem
    {
        public override void Load() => SotsIds.ResolveIds();
        public override void PostAddRecipes() => SotsIds.ResolveIds();
    }
}
