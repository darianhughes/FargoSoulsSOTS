using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fargowiltas.Items.Summons;
using Fargowiltas.Projectiles;
using SOTS.Items.Slime;
using SOTS.Items.Tools;
using SOTS.NPCs.Boss;
using SOTS.NPCs.Boss.Advisor;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SOTS.NPCs.Constructs;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using SOTS.Items;
using FargowiltasSouls.Core.Toggler;

namespace FargoSoulsSOTS.Content.Items.Summons.SOTSCopy
{
    public class OldCRTTV : BaseSummon
    {
        public override int NPCType => ModContent.NPCType<TheAdvisorHead>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.DeerThing];
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.accessory = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.AddEffect<TennaEffect>(Item);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<TennaEffect>(Item);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (ResetTimeWhenUsed)
            {
                Main.time = 0;

                if (Main.netMode == NetmodeID.Server) //sync time
                    NetMessage.SendData(MessageID.WorldData, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
            }

            foreach (NPC npc in Main.npc)
            {
                if (npc.type == ModContent.NPCType<OtherworldlyConstructHead>() || npc.type == ModContent.NPCType<OtherworldlyConstructHead2>())
                    npc.active = false;
            }

            Vector2 pos = new Vector2((int)player.Center.X, (int)player.Center.Y - 250);


            Projectile.NewProjectile(player.GetSource_ItemUse(source.Item), pos, Vector2.Zero, ModContent.ProjectileType<SpawnProj>(), 0, 0, Main.myPlayer, NPCType);

            LocalizedText text = Language.GetText("Fargowiltas.Announcement.HasAwoken");
            string npcName = NPCName ?? (ModContent.GetModNPC(NPCType) == null ? Lang.GetNPCNameValue(NPCType) : ModContent.GetModNPC(NPCType).DisplayName.Value);

            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(text.ToNetworkText(npcName), new Color(175, 75, 255));
            }

            SoundEngine.PlaySound(SoundID.Roar, player.position);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<WorldgenScanner>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class TennaEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DeviEnergyHeader>();
        public override int ToggleItemType => ModContent.ItemType<OldCRTTV>();
    }
}
