using Terraria.ID;
using Terraria.ModLoader;
using Fargowiltas.Content.Items.Summons.SwarmSummons;
using Consolaria.Content.Items.Summons;
using Consolaria.Content.NPCs.Bosses.Ocram;
using Terraria;

namespace SecretsOfTheSouls.Content.Items.Summons.SwarmSummons.Summons.ConsolariaSummons
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class OverloadOcram : SwarmSummonBase
    {
        //public override string Texture => "Consolaria/Content/Items/Summons/SuspiciousLookingSkull";

        public OverloadOcram() : base(ModContent.NPCType<Ocram>(), nameof(OverloadOcram), 50, ModContent.ItemType<SuspiciousLookingSkull>())
        {
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ModContent.ItemType<SuspiciousLookingSkull>()];
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<SuspiciousLookingSkull>())
                .AddIngredient(ModLoader.GetMod("Fargowiltas"), "Overloader")
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
