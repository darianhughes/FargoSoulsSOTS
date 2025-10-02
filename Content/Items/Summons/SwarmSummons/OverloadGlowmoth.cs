using Fargowiltas.Items.Summons.SwarmSummons;
using Terraria.ID;
using Terraria.ModLoader;
using SOTS.NPCs.Boss.Glowmoth;
using SOTS.Items.Earth.Glowmoth;

namespace FargoSoulsSOTS.Content.Items.Summons.SwarmSummons
{
    public class OverloadGlowmoth : SwarmSummonBase
    {
        //public override string Texture => "SOTS/Items/Earth/Glowmoth/SuspiciousLookingCandle";

        public OverloadGlowmoth() : base(ModContent.NPCType<Glowmoth>(), nameof(OverloadGlowmoth), 50, "GlowingNylonCandle")
        {
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ModContent.ItemType<SuspiciousLookingCandle>()];
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(null, "GlowingNylonCandle")
                .AddIngredient(ModLoader.GetMod("Fargowiltas"), "Overloader")
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
