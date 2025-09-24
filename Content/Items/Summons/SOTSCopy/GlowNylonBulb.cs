using Fargowiltas.Items.Summons;
using Terraria.ID;
using Terraria.ModLoader;
using SOTS.NPCs.Boss.Glowmoth;
using SOTS.Items.Earth.Glowmoth;

namespace FargoSoulsSOTS.Content.Items.Summons.SOTSCopy
{
    [LegacyName("GlowingNylonCandle")]
    public class GlowNylonBulb : BaseSummon
    {
        //public override string Texture => "SOTS/Items/Earth/Glowmoth/SuspiciousLookingCandle";
        public override int NPCType => ModContent.NPCType<Glowmoth>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ModContent.ItemType<SuspiciousLookingCandle>()];
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SuspiciousLookingCandle>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
