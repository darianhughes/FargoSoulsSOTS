using Fargowiltas.Content.Items.Summons.SwarmSummons;
using SecretsOfTheSouls.Content.Items.Summons.SOTSCopy;
using SOTS.NPCs.Boss.Excavator;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Summons.SwarmSummons.Summons.SOTSSummons
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class OverloadExcavator : SwarmSummonBase
    {
        public override string Texture => "SOTS/Items/AbandonedVillage/SeismicStation";

        public OverloadExcavator() : base(ModContent.NPCType<Excavator>(), nameof(OverloadExcavator), 50, ModContent.ItemType<ExcavationRemote>())
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
