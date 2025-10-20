using Terraria.ID;
using Terraria.ModLoader;
using SOTS.NPCs.Boss;
using Fargowiltas.Content.Items.Summons.SwarmSummons;
using SecretsOfTheSouls.Content.Items.Summons.SOTSCopy;

namespace SecretsOfTheSouls.Content.Items.Summons.SwarmSummons.Summons.SOTSSummons
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class OverloadPutridPinky : SwarmSummonBase
    {
        public override string Texture => "SOTS/Items/Slime/JarOfPeanuts";

        public OverloadPutridPinky() : base(ModContent.NPCType<PutridPinkyPhase2>(), nameof(OverloadPutridPinky), 50, ModContent.ItemType<OffbrandPeanuts>())
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
