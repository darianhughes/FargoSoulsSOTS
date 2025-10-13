using SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SecretsOfTheSouls.Core.SoulToggles.SOTSToggles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.Xna.Framework;

namespace SecretsOfTheSouls.Content.Items.Accessories.Eternity.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class JellyJumpers : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = (int)(62 * 0.6f);
            Item.height = (int)(60 * 0.6f);
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 3);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Corrosion>()] = true;

            player.extraFall += 10;
            player.autoJump = true;

            player.AddEffect<JellyJumpersEffect>(Item);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Rectangle frame = Main.itemAnimations[Type]?.GetFrame(tex) ?? tex.Frame();
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 pos = Item.Center - Main.screenPosition;
            Color drawColor = Item.GetAlpha(Lighting.GetColor((int)(Item.Center.X / 16f), (int)(Item.Center.Y / 16f)));

            const float quarter = 0.6f;
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

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class JellyJumpersEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<GadgetCoatHeader>();
        public override int ToggleItemType => ModContent.ItemType<JellyJumpers>();
    }
}
