using FargoSoulsSOTS.Content.Items.Summons.SOTSCopy;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.NPCs.Boss.Advisor;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.ModScenes
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class AdvisorAltMusicScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/ItsTVTime");

        public override bool IsSceneEffectActive(Player player)
        {
            if (Main.gameMenu) return false;
            if (!player.active) return false;

            if (!player.HasEffect<TennaEffect>()) return false;

            if (!NPC.AnyNPCs(ModContent.NPCType<TheAdvisorHead>()))
                return false;

            foreach (NPC npc in Main.npc)
            {
                if (npc.type == ModContent.NPCType<TheAdvisorHead>() && npc.boss)
                    return true;
            }

            return false;
        }
    }
}
