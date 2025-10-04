using Fargowiltas.NPCs;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using SOTS.Items;
using Terraria;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Common.NPCChanges
{
    public class SquirrelGlobalNPC : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<Squirrel>();
        }

        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            if (npc.type == ModContent.NPCType<Squirrel>())
            {
                bool sellSubspaceMaterials = false;
                bool soldSubspaceMaterials = false;
                foreach (Player player in Main.player)
                {
                    foreach (Item item in player.inventory)
                    {
                        if (item.type == ModContent.ItemType<SubspaceBoosters>())
                            sellSubspaceMaterials = true;
                    }
                    foreach (Item item in player.armor)
                    {
                        if (item.type == ModContent.ItemType<SubspaceBoosters>())
                            sellSubspaceMaterials = true;
                    }
                    foreach (Item item in player.bank.item)
                    {
                        if (item.type == ModContent.ItemType<SubspaceBoosters>())
                            sellSubspaceMaterials = true;
                    }
                }
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] is null && sellSubspaceMaterials && !soldSubspaceMaterials)
                    {
                        items[i] = new Item(ModContent.ItemType<FlashsparkBoots>()) { shopCustomPrice = Item.buyPrice(gold: 25) };
                        items[i + 1] = new Item(ModContent.ItemType<AeolusBoots>()) { shopCustomPrice = Item.buyPrice(gold: 35) };
                        soldSubspaceMaterials = true;
                    }
                }
            }
        }
    }
}
