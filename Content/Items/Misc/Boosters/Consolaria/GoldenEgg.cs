using Consolaria.Content.Buffs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Misc.Boosters.Consolaria
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class GoldenEgg : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsAPickup[Type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Type] = true;
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;
        }

        public override bool CanPickup(Player player)
        {
            if (Item.timeSinceItemSpawned <= 30)
                return false;
            return base.CanPickup(player);
        }

        public static void PickupEffect(Player player)
        {
            player.AddBuff(ModContent.BuffType<Fortuned>(), 60 * 30);
        }

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(SoundID.Coins, Item.position);
            PickupEffect(player);
            return false;
        }
    }
}
