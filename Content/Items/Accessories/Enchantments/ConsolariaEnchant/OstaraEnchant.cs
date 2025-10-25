using Consolaria;
using Consolaria.Content.Items.Accessories;
using Consolaria.Content.Items.Armor.Misc;
using Consolaria.Content.Items.Consumables;
using Consolaria.Content.Items.Weapons.Ranged;
using Consolaria.Content.NPCs.Bosses.Lepus;
using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Microsoft.Xna.Framework;
using SecretsOfTheSouls.Content.Items.Misc.Boosters.Consolaria;
using SecretsOfTheSouls.Core.Players;
using SecretsOfTheSouls.Core.SoulToggles.ConsolariaToggles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Accessories.Enchantments.ConsolariaEnchant
{
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class OstaraEnchant : BaseEnchant
    {
        public override Color nameColor => new Color(148, 214, 107);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Green;
            Item.value = 50000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ConsolariaEffectsPlayer>().ostaraEnchant = true;

            player.jumpSpeedBoost += 1.25f;

            player.AddEffect<OstaraEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<OstaraHat>()
                .AddIngredient<OstaraJacket>()
                .AddIngredient<OstaraBoots>()
                .AddIngredient<OstarasGift>()
                .AddIngredient<EggCannon>()
                .AddIngredient<CandiedFruit>()
                .AddTile<EnchantedTreeSheet>()
                .Register();
        }
    }

    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class OstaraEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<MightForceHeader>();
        public override int ToggleItemType => ModContent.ItemType<OstaraEnchant>();

        private int prevJump;
        public override void PostUpdateEquips(Player player)
        {
            bool jumpedThisTick = player.jump > 0 && prevJump == 0;

            if (jumpedThisTick)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Main.rand.NextChance(player.ForceEffect<OstaraEffect>() ? 0.15 : 0.1))
                    {
                        int num = NPC.NewNPC(player.GetSource_Misc("OstaraJump"), (int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<ChocolateEgg>());
                        if (num >= 0 && num < Main.maxNPCs && Main.netMode == NetmodeID.Server)
                            NetMessage.SendData(MessageID.SyncNPC, number: num);
                    }
                    if (Main.rand.NextChance(player.ForceEffect<OstaraEffect>() ? 0.1 : 0.05))
                    {
                        int num = Item.NewItem(player.GetSource_Misc("OstaraJump"), player.Center, ModContent.ItemType<GoldenEgg>());
                    }
                }

            }
            prevJump = player.jump;
        }
    }
}
