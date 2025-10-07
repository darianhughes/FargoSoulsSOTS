using Fargowiltas.Content.Items.Summons.SwarmSummons;
using SecretsOfTheSouls.Content.Items.Summons.SOTSCopy;
using SOTS.Items.Permafrost;
using SOTS.NPCs.Boss.Polaris.NewPolaris;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Summons.SwarmSummons.SOTSSummons
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class OverloadPolaris : SwarmSummonBase
    {
        public override string Texture => "SecretsOfTheSouls/Content/Items/Summons/SOTSCopy/PolarKey";

        public OverloadPolaris() : base(ModContent.NPCType<NewPolaris>(), nameof(OverloadPolaris), 50, ModContent.ItemType<PolarKey>())
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
