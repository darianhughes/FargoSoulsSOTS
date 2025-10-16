using Consolaria.Common.ModSystems;
using Consolaria.Content.Items.Summons;
using Terraria;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Common.ItemChanges
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class ConsolariaGlobalItem : GlobalItem
    {
        public static readonly int[] AlwyasUsableConsolariaSummons = [ModContent.ItemType<SuspiciousLookingEgg>(), ModContent.ItemType<CursedStuffing>(), ModContent.ItemType<SuspiciousLookingSkull>()];
        public static readonly int[] NightSettingConsolariaSummons = [ModContent.ItemType<SuspiciousLookingSkull>()];

        public override bool ConsumeItem(Item item, Player player)
        {
            if ((item.type == ModContent.ItemType<SuspiciousLookingEgg>() && !DownedBossSystem.downedLepus) ||
                (item.type == ModContent.ItemType<CursedStuffing>() && !DownedBossSystem.downedTurkor) ||
                (item.type == ModContent.ItemType<SuspiciousLookingSkull>() && !DownedBossSystem.downedOcram)
                )
            {
                return false;
            }
            return base.ConsumeItem(item, player);
        }
    }
}
