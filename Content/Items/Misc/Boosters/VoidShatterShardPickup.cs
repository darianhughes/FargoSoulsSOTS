using System.Reflection;
using SOTS.Void;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace FargoSoulsSOTS.Content.Items.Misc.Boosters
{
    public class VoidShatterShardPickup : ModItem
    {
        public override string Texture => "SOTS/Projectiles/Permafrost/ShatterShard";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.IsAPickup[Type] = true;
            ItemID.Sets.IgnoresEncumberingStone[Type] = true;
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public static void PickupEffect(Player player)
        {
            VoidPlayer mp = VoidPlayer.ModPlayer(player);

            player.Heal((int)(player.statLifeMax2 * 0.1f));
            player.statMana += (int)(player.statManaMax2 * 0.05f);

            int voidHeal = (int)(mp.voidMeterMax2 * 0.05f);
            mp.voidMeter += voidHeal;
            VoidPlayer.VoidEffect(player, voidHeal);
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
        public override Color? GetAlpha(Color lightColor) => Color.Purple;

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Item.type].Value;
            Vector2 origin = new(tex.Width / 2f, tex.Height / 2f);
            Main.EntitySpriteDraw(
                tex,
                Item.Center - Main.screenPosition,
                null,
                Color.Purple,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                0);
            return false;
        }
    }
}
