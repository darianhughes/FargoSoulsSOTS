using Fargowiltas.Items.Summons.SwarmSummons;
using SOTS.NPCs.Boss.Excavator;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.Summons.SwarmSummons.SOTSSummons
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class OverloadExcavator : SwarmSummonBase
    {
        public override string Texture => "SOTS/Items/AbandonedVillage/SeismicStation";

        public OverloadExcavator() : base(ModContent.NPCType<Excavator>(), nameof(OverloadExcavator), 50, "ExcavationRemote")
        {
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.DeerThing];
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(null, "ExcavationRemote")
                .AddIngredient(ModLoader.GetMod("Fargowiltas"), "Overloader")
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
