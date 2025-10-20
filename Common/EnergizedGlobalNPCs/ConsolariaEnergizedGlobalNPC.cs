using Consolaria.Content.NPCs.Bosses.Lepus;
using Consolaria.Content.NPCs.Bosses.Ocram;
using Consolaria.Content.NPCs.Bosses.Turkor;
using Terraria;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Common.EnergizedGlobalNPCs
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class ConsolariaEnergizedGlobalNPC : GlobalNPC
    {
        private static Mod Consolaria => ModLoader.GetMod(SecretsOfTheSoulsCrossmod.Consolaria.Name);
        public override bool InstancePerEntity => true;
        public bool SwarmHealth = false;

        internal static int[] ConsolariaBosses = [
            ModContent.NPCType<DisasterBunny>(),
            ModContent.NPCType<TurkortheUngrateful>(),
            ModContent.NPCType<Ocram>()
        ];

        public override void SetDefaults(NPC npc)
        {
            const int k = 1000;
            const int m = k * k;
            int baseHealth = 18 * k;
            int baseHealthHM = 48 * k;
            bool validBoss = true;

            if (Fargowiltas.Fargowiltas.SwarmSetDefaults)
            {
                if (npc.type == Consolaria.Find<ModNPC>("Lepus").Type)
                    npc.lifeMax = baseHealth;

                else if (npc.type == ModContent.NPCType<TurkortheUngrateful>())
                    npc.lifeMax = baseHealth;

                else if (npc.type == ModContent.NPCType<Ocram>())
                    npc.lifeMax = baseHealthHM;

                else
                    validBoss = false;
            }
            else
                validBoss = false;

            if (Fargowiltas.Fargowiltas.SwarmActive)
            {
                /*
                if (!validBoss)
                {
                    validBoss = true;

                    validBoss = false;
                }
                */

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

            return true;
        }
    }
}
