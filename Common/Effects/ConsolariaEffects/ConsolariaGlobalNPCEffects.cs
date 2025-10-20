using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using SecretsOfTheSouls.Common.Effects.SOTSEffects;
using Consolaria.Content.NPCs.Bosses.Lepus;
using Consolaria.Content.Items.Consumables;
using Consolaria.Content.Items.Placeable;
using Consolaria.Content.NPCs.Bosses.Turkor;
using Consolaria.Content.NPCs.Bosses.Ocram;
using SecretsOfTheSouls.Content.Items.Summons.SwarmSummons.Energizers.ConsolariaEnergizers;

namespace SecretsOfTheSouls.Common.Effects.ConsolariaEffects
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class ConsolariaGlobalNPCEffects : GlobalNPC
    {
        private static Mod Consolaria => ModLoader.GetMod(SecretsOfTheSoulsCrossmod.Consolaria.Name);
        public override bool InstancePerEntity => true;

        public override bool PreKill(NPC npc)
        {
            bool doDeviText = false;

            /*
            if (npc.ModNPC is TreasureSlime && !SecretsOfTheSoulsWorldSavingSystem.downedTreasureSlime)
            {
                doDeviText = true;
                SecretsOfTheSoulsWorldSavingSystem.downedTreasureSlime = true;
            }
            */

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
                if (npc.type == Consolaria.Find<ModNPC>("Lepus").Type)
                    Swarm(npc, Consolaria.Find<ModNPC>("Lepus").Type, ModContent.NPCType<DisasterBunny>(), ModContent.ItemType<LepusBag>(), ModContent.ItemType<LepusTrophy>(), ModContent.ItemType<EnergizerLepus>());
                if (npc.type == ModContent.NPCType<TurkortheUngrateful>())
                    Swarm(npc, ModContent.NPCType<TurkortheUngrateful>(), ModContent.NPCType<TurkortheUngratefulHead>(), ModContent.ItemType<TurkorBag>(), ModContent.ItemType<TurkorTrophy>(), ModContent.ItemType<EnergizerTurkor>());
                if (npc.type == ModContent.NPCType<Ocram>())
                    Swarm(npc, ModContent.NPCType<Ocram>(), ModContent.NPCType<ServantofOcram>(), ModContent.ItemType<OcramBag>(), ModContent.ItemType<OcramTrophy>(), ModContent.ItemType<EnergizerOcram>());
            }

            return base.PreKill(npc);
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
    }
}
