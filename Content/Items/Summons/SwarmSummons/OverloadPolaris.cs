using Fargowiltas.Items.Summons.SwarmSummons;
using SOTS.Items.Permafrost;
using SOTS.NPCs.Boss.Polaris.NewPolaris;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.Summons.SwarmSummons
{
    public class OverloadPolaris : SwarmSummonBase
    {
        public override string Texture => "FargoSoulsSOTS/Content/Items/Summons/SOTSCopy/PolarKey";

        public OverloadPolaris() : base(ModContent.NPCType<NewPolaris>(), nameof(OverloadPolaris), 50, "PolarKey")
        {
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ModContent.ItemType<FrostedKey>()];
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(null, "PolarKey")
                .AddIngredient(ModLoader.GetMod("Fargowiltas"), "Overloader")
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
