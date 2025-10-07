using Fargowiltas.Items.Summons;
using SOTS.Items.Slime;
using SOTS.NPCs.Boss;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.Summons.SOTSCopy
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class OffbrandPeanuts : BaseSummon
    {
        //public override string Texture => "SOTS/Items/Slime/JarOfPeanuts";
        public override int NPCType => ModContent.NPCType<PutridPinkyPhase2>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ModContent.ItemType<JarOfPeanuts>()];
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<JarOfPeanuts>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
