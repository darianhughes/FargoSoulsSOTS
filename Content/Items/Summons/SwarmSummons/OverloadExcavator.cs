using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fargowiltas.Items.Summons.SwarmSummons;
using SOTS.Items.Earth.Glowmoth;
using SOTS.NPCs.Boss.Excavator;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.Summons.SwarmSummons
{
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
