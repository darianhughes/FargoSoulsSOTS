using FargoSoulsSOTS.Content.Buffs.Emode.SOTSBuffs;
using FargoSoulsSOTS.Core.SoulToggles;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using SOTS.Void;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.Accessories.Masomode.SOTSEternity
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class GlowBulb : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<Lepidopterism>()] = true;

            player.AddEffect<GlowBulbEffect>(Item);
        }
    }

    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class GlowBulbEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<AncientMcGuffinHeader>();
        public override int ToggleItemType => ModContent.ItemType<GlowBulb>();

        public override void PostUpdate(Player player)
        {
            var mp = player.GetModPlayer<VoidPlayer>();

            if (!IsDarkWhereIAm(player))
                return;

            mp.voidRegenSpeed += 0.15f;
        }

        public override void PostUpdateMiscEffects(Player player)
        {
            if (IsDarkWhereIAm(player))
            {
                Lighting.AddLight(player.Center, 0.03f, 0.07f, 0.20f);
            }
        }

        private bool IsDarkWhereIAm(Player player)
        {
            int tx = (int)(player.Center.X / 16f);
            int ty = (int)(player.Center.Y / 16f);
            var c = Lighting.GetColor(tx, ty);
            float r = c.R / 255f, g = c.G / 255f, b = c.B / 255f;
            float luminance = 0.2126f * r + 0.7152f * g + 0.0722f * b;

            return luminance < 0.15f;
        }
    }
}
