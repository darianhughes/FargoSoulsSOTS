using FargoSoulsSOTS.Content.Buffs.Emode;
using FargowiltasSouls.Content.Items;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargoSoulsSOTS.Core.SoulToggles;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace FargoSoulsSOTS.Content.Items.Accessories.Masomode
{
    [AutoloadEquip(EquipType.Face)]
    public class DrillCap : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorIDs.Face.Sets.PreventHairDraw[Item.faceSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Grounded>()] = true;
            player.buffImmune[ModContent.BuffType<LowGroundBuff>()] = true;

            player.AddEffect<DrillCapEffect>(Item);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Rectangle frame = Main.itemAnimations[Type]?.GetFrame(tex) ?? tex.Frame();
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 pos = Item.Center - Main.screenPosition;
            Color drawColor = Item.GetAlpha(Lighting.GetColor((int)(Item.Center.X / 16f), (int)(Item.Center.Y / 16f)));

            const float quarter = 0.75f;
            Main.EntitySpriteDraw(
                tex,
                pos,
                frame,
                drawColor,
                rotation,
                origin,
                Item.scale * quarter,
                SpriteEffects.None,
                0
            );

            return false; // suppress default draw
        }
    }

    public class DrillCapEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<GadgetCoatHeader>();
        public override int ToggleItemType => ModContent.ItemType<DrillCap>();

        private bool wasSolidAboveLastTick;

        public override void PostUpdate(Player player)
        {
            bool solidAbove = IsSolidImmediatelyAboveHead(player);
            bool upward = player.velocity.Y < -0.1f;

            if (upward && solidAbove && !wasSolidAboveLastTick)
            {
                TryMine3x1Above(player);
            }

            wasSolidAboveLastTick = solidAbove;
        }

        private static bool IsSolidImmediatelyAboveHead(Player player)
        {
            var probePos = new Vector2(player.position.X, player.Top.Y - 4f);
            int probeW = player.width;
            int probeH = 6;

            return Collision.SolidCollision(probePos, probeW, probeH);
        }

        private static int GetBestPickPowerInInventory(Player p)
        {
            int best = 0;
            for (int i = 0; i < p.inventory.Length; i++)
            {
                Item it = p.inventory[i];
                if (it?.active == true && it.pick > best)
                    best = it.pick;
            }
            return best;
        }

        private void TryMine3x1Above(Player p)
        {
            int bestPick = GetBestPickPowerInInventory(p);
            if (bestPick <= 0) return;

            int cx = (int)(p.Center.X / 16f);
            int cy = (int)((p.Top.Y / 16f)) - 1;

            for (int x = cx - 1; x <= cx + 1; x++)
            {
                MineTileIfAllowed(p, x, cy, bestPick);
            }

            int sound = Main.rand.Next(0, 2);
            if (sound == 0)
            {
                SoundEngine.PlaySound(SoundID.Item22 with { Volume = 0.7f }, p.Center);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Item23 with { Volume = 0.7f }, p.Center);
            }
        }

        private void MineTileIfAllowed(Player p, int i, int j, int bestPick)
        {
            if (!WorldGen.InWorld(i, j, 10)) return;

            Tile tile = Framing.GetTileSafely(i, j);
            if (tile == null || !tile.HasTile) return;

            if (IsProtectedTile(i, j, tile)) return;

            const int swings = 8;
            for (int s = 0; s < swings; s++)
                p.PickTile(i, j, bestPick);

            if (!Framing.GetTileSafely(i, j).HasTile)
            {
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.TileManipulation, number: 0, number2: i, number3: j);
                }
            }
        }

        private static bool IsProtectedTile(int i, int j, Tile tile)
        {
            if (TileID.Sets.BasicChest[tile.TileType]) return true;
            if (TileID.Sets.BasicDresser[tile.TileType]) return true;
            if (tile.TileType == TileID.DemonAltar) return true;

            var data = TileObjectData.GetTileData(tile);
            if (data != null && (data.AnchorBottom.tileCount > 0 || data.AnchorTop.tileCount > 0 || data.AnchorWall))
                return true;

            if (tile.TileType == TileID.Platforms || tile.TileType == TileID.Rope) return true;

            return false;
        }
    }
}
