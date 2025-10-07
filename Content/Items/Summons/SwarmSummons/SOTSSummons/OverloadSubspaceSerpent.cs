using Terraria.ModLoader;
using Terraria;
using SOTS.NPCs.Boss;
using Terraria.ID;
using SOTS.Items.Celestial;
using Fargowiltas.Content.Items.Summons.SwarmSummons;
using SecretsOfTheSouls.Content.Items.Summons.SOTSCopy;

namespace SecretsOfTheSouls.Content.Items.Summons.SwarmSummons.SOTSSummons
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class OverloadSubspaceSerpent : SwarmSummonBase
    {
        //public override string Texture => "SOTS/Items/Celestial/CatalystBomb";

        public OverloadSubspaceSerpent() : base(ModContent.NPCType<SubspaceSerpentHead>(), nameof(OverloadSubspaceSerpent), 50, ModContent.ItemType<CatalystDynamite>())
        {
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ModContent.ItemType<CatalystBomb>()];
        }

        public override bool CanUseItem(Player player)
        {
            return !Fargowiltas.Fargowiltas.SwarmActive && player.ZoneUnderworldHeight;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(null, "CatalyzedCrystal")
                .AddIngredient(ModLoader.GetMod("Fargowiltas"), "Overloader")
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
