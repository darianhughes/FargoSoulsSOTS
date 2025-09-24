using System;
using Fargowiltas.Items.Summons.SwarmSummons;
using SOTS.NPCs.Boss.Advisor;
using Terraria.Audio;
using Terraria.Chat;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using FargoSoulsSOTS.Common;
using Microsoft.Xna.Framework;
using SOTS.NPCs.Constructs;


namespace FargoSoulsSOTS.Content.Items.Summons.SwarmSummons
{
    public class OverloadAdvisor : SwarmSummonBase
    {
        public override string Texture => "FargoSoulsSOTS/Content/Items/Summons/SOTSCopy/OldCRTTV";

        public OverloadAdvisor() : base(ModContent.NPCType<TheAdvisorHead>(), nameof(OverloadAdvisor), 50, "OldCRTTV")
        {
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.DeerThing];
        }

        public override bool? UseItem(Player player)
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.type == ModContent.NPCType<OtherworldlyConstructHead>() || npc.type == ModContent.NPCType<OtherworldlyConstructHead2>())
                    npc.active = false;
            }

            Fargowiltas.Fargowiltas.SwarmSetDefaults = true;

            Fargowiltas.Fargowiltas.SwarmActive = true;
            int usedItems = Math.Min(player.inventory[player.selectedItem].stack, 10);
            Fargowiltas.Fargowiltas.SwarmItemsUsed = usedItems;
            Fargowiltas.Fargowiltas.SwarmNoHyperActive = Fargowiltas.Fargowiltas.SwarmItemsUsed < 5;

            int boss = NPC.NewNPC(NPC.GetBossSpawnSource(player.whoAmI), (int)player.Center.X, (int)player.Center.Y - 250, ModContent.NPCType<TheAdvisorHead>());
            Main.npc[boss].TrySetFargoSwarmActive(true);

            player.inventory[player.selectedItem].stack -= usedItems - 1;

            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey($"Mods.Fargowiltas.MessageInfo.{nameof(OverloadAdvisor)}"), new Color(175, 75, 255));
                NetMessage.SendData(MessageID.WorldData);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue($"Mods.Fargowiltas.MessageInfo.{nameof(OverloadAdvisor)}"), 175, 75, 255);
            }

            SoundEngine.PlaySound(SoundID.Roar, player.position);

            Fargowiltas.Fargowiltas.SwarmSetDefaults = false;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(null, "OldCRTTV")
                .AddIngredient(ModLoader.GetMod("Fargowiltas"), "Overloader")
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
