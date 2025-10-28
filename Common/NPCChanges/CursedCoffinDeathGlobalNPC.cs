using FargowiltasSouls.Content.Bosses.CursedCoffin;
using Microsoft.Xna.Framework;
using SecretsOfTheSouls.Core.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Common.NPCChanges
{
    public class CursedCoffinDeathGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<CursedCoffin>();
        }

        public override void OnKill(NPC npc)
        {
            // Check if this is the first time the Cursed Coffin has been defeated
            if (!SecretsOfTheSoulsWorldSavingSystem.downedCursedCoffin)
            {
                SecretsOfTheSoulsWorldSavingSystem.downedCursedCoffin = true;

                // Display the special message to all players
                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText("The coffin's curse fades, revealing the keyhole on the mysterious gate...",
                        new Color(175, 75, 255)); // Purple color
                }
            }
        }
    }
}
