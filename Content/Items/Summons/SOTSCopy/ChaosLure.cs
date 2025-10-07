using Fargowiltas.Items.Summons;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using SOTS.NPCs.Boss.Lux;
using SOTS.Items;
using SOTS;
using Terraria.DataStructures;

namespace FargoSoulsSOTS.Content.Items.Summons.SOTSCopy
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class ChaosLure : BaseSummon
    {
        public override string Texture => "SOTS/Items/ElectromagneticLure";
        public override int NPCType => ModContent.NPCType<Lux>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 6, false));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            this.SetResearchCost(1);

            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.LihzahrdPowerCell];
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ElectromagneticLure>()
                .AddTile(TileID.WorkBenches)
                .AddCondition(Condition.Hardmode)
                .Register();
        }
    }
}
