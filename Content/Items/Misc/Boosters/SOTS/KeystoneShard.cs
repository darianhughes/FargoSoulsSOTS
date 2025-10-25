using System.Reflection;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs;

namespace SecretsOfTheSouls.Content.Items.Misc.Boosters.SOTS
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class KeystoneShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsAPickup[Type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Type] = true;
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public static void PickupEffect(Player player)
        {
            player.AddBuff(ModContent.BuffType<VoidEmpowerment>(), 60 * 5);
        }

        public override bool OnPickup(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grab, Item.position);
            PickupEffect(player);
            return false;
        }

        public override void GrabRange(Player player, ref int grabRange)
        {
            grabRange += 100;
        }

        public static MethodInfo PullItem_PickupMethod
        {
            get;
            set;
        }

        public override void Load()
        {
            PullItem_PickupMethod = typeof(Player).GetMethod("PullItem_Pickup", LumUtils.UniversalBindingFlags);
        }

        public override bool GrabStyle(Player player)
        {
            object[] args = [Item, 12f, 5];
            PullItem_PickupMethod.Invoke(player, args);
            return true;
        }
    }
}
