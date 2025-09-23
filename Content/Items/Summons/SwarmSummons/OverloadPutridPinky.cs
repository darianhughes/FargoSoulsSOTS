using Fargowiltas.Items.Summons.SwarmSummons;
using SOTS;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using SOTS.NPCs.Boss;

namespace FargoSoulsSOTS.Content.Items.Summons.SwarmSummons
{
    public class OverloadPutridPinky : SwarmSummonBase
    {
        public override string Texture => "SOTS/Items/Slime/JarOfPeanuts";

        public OverloadPutridPinky() : base(ModContent.NPCType<PutridPinkyPhase2>(), nameof(OverloadPutridPinky), 50, "OffbrandPeanuts")
        {
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.LihzahrdPowerCell];
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(null, "OffbrandPeanuts")
                .AddIngredient(ModLoader.GetMod("Fargowiltas"), "Overloader")
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
