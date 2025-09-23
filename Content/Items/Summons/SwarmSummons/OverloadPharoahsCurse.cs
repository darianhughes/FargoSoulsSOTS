using Fargowiltas.Items.Summons.SwarmSummons;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using SOTS.NPCs.Boss.Curse;
using SOTS.Items.Pyramid;
using Microsoft.Xna.Framework;
using System;
using Fargowiltas.NPCs;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.Localization;
using FargoSoulsSOTS.Content.Projectiles;
using ssm;

namespace FargoSoulsSOTS.Content.Items.Summons.SwarmSummons
{
    public class OverloadPharoahsCurse : SwarmSummonBase
    {
        private const int SearchRangeTiles = 3;
        public override string Texture => "SOTS/Items/Pyramid/Sarcophagus";

        public OverloadPharoahsCurse() : base(ModContent.NPCType<PharaohsCurse>(), nameof(OverloadPharoahsCurse), 50, "CursedSarcophagus")
        {
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.LihzahrdPowerCell];
        }

        public override bool CanUseItem(Player player)
        {
            return TryFindNearestSarcophagusOrigin(player, SearchRangeTiles, out _, out _);
        }

        public override bool? UseItem(Player player)
        {
            if (!TryFindNearestSarcophagusOrigin(player, SearchRangeTiles, out Point16 originTile, out Vector2 spawnPos))
                return false;

            int usedItems = Math.Min(player.inventory[player.selectedItem].stack, 10);
            Fargowiltas.Fargowiltas.SwarmItemsUsed = usedItems;
            Fargowiltas.Fargowiltas.SwarmNoHyperActive = Fargowiltas.Fargowiltas.SwarmItemsUsed < 5;

            // Spawn exactly like SOTS SarcophagusTile.RightClick:
            // Projectile.NewProjectile(source, origin*16 + new Vector2(48f,16f), Vector2.Zero, SpawnEnemyProj, 0, 0, myPlayer, -1f, 0f, 0f)
            if (player.whoAmI == Main.myPlayer)
            {
                IEntitySource source = player.GetSource_ItemUse(Item);
                Projectile.NewProjectile(
                    source,
                    spawnPos,                       // already origin + (48,16)
                    Vector2.Zero,
                    ModContent.ProjectileType<SpawnEnemyProjNoLimit>(),
                    0,
                    0f,
                    player.whoAmI,
                    -1f, 0f, 1f
                );
            }

            player.inventory[player.selectedItem].stack -= usedItems - 1;

            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey($"Mods.Fargowiltas.MessageInfo.{nameof(OverloadPharoahsCurse)}"), new Color(175, 75, 255));
                NetMessage.SendData(MessageID.WorldData);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue($"Mods.Fargowiltas.MessageInfo.{nameof(OverloadPharoahsCurse)}"), 175, 75, 255);
            }

            SoundEngine.PlaySound(SoundID.Roar, player.position);

            return true;
        }

        private static bool TryFindNearestSarcophagusOrigin(Player player, int rangeTiles, out Point16 originTile, out Vector2 spawnPos)
        {
            originTile = Point16.NegativeOne;
            spawnPos = Vector2.Zero;

            int targetType = ModContent.TileType<SarcophagusTile>();
            Point p = player.Center.ToTileCoordinates();

            int r = rangeTiles;
            float bestDistSq = float.MaxValue;
            bool found = false;

            // Clamp search box to world bounds.
            int xMin = Utilities.Clamp(p.X - r, 0, Main.maxTilesX - 1);
            int xMax = Utilities.Clamp(p.X + r, 0, Main.maxTilesX - 1);
            int yMin = Utilities.Clamp(p.Y - r, 0, Main.maxTilesY - 1);
            int yMax = Utilities.Clamp(p.Y + r, 0, Main.maxTilesY - 1);

            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    Tile t = Main.tile[x, y];
                    if (t == null || !t.HasTile || t.TileType != targetType)
                        continue;

                    // Recover the top-left origin of the multi-tile (exactly like SOTS RightClick).
                    int frameX = t.TileFrameX / 18;
                    int frameY = t.TileFrameY / 18;
                    int originX = x - frameX;
                    int originY = y - frameY;

                    // Convert the SOTS spawn position (origin + (48,16) px).
                    Vector2 candidateSpawn = new Vector2(originX * 16 + 48f, originY * 16 + 16f);

                    float distSq = Vector2.DistanceSquared(candidateSpawn, player.Center);
                    if (distSq < bestDistSq)
                    {
                        bestDistSq = distSq;
                        originTile = new Point16(originX, originY);
                        spawnPos = candidateSpawn;
                        found = true;
                    }
                }
            }

            return found;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(null, "CursedSarcophagus")
                .AddIngredient(ModLoader.GetMod("Fargowiltas"), "Overloader")
                .AddTile(TileID.DemonAltar)
                .Register();
        }

        private static class Utilities
        {
            public static int Clamp(int v, int min, int max) => v < min ? min : (v > max ? max : v);
        }
    }
}
