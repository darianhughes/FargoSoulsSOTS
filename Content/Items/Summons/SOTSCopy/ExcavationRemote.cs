using Fargowiltas.Items.Summons;
using SOTS.Items.Earth.Glowmoth;
using SOTS.NPCs.Boss.Excavator;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.Summons.SOTSCopy
{
    public class ExcavationRemote : BaseSummon
    {
        public override string Texture => "SOTS/Items/AbandonedVillage/SeismicStation";
        public override int NPCType => ModContent.NPCType<Excavator>();

        public override bool CanUseItem(Player player)
        {
            return player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.DeerThing];
        }
    }
}
