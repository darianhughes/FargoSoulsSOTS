using Fargowiltas.Items.Summons;
using SOTS.Items.Permafrost;
using SOTS.NPCs.Boss;
using SOTS.NPCs.Boss.Polaris.NewPolaris;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.Summons.SOTSCopy
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class PolarKey : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<NewPolaris>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ModContent.ItemType<FrostedKey>()];
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FrostedKey>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
