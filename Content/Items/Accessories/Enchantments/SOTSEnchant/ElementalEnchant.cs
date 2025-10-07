using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.Items.Chaos;
using SOTS.Items.Fragments;
using FargoSoulsSOTS.Core.SoulToggles;
using FargowiltasSouls.Content.UI.Elements;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls;
using FargoSoulsSOTS.Core.Players;

namespace FargoSoulsSOTS.Content.Items.Accessories.Enchantments.SOTSEnchant
{
    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class ElementalEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargoSOTSConfig.Instance.UnfinishedContent;
        }
        public override Color nameColor => new(116, 122, 159);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(77, 3, 26);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<ChaosTeleport>(Item);
            player.AddEffect<ElementalEffect>(Item);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ElementalHelmet>()
                .AddIngredient<ElementalBreastplate>()
                .AddIngredient<ElementalLeggings>()
                .AddIngredient<HyperlightGeyser>()
                .AddIngredient<TwilightAssassinEnchant>()
                .AddIngredient<RoseBow>()
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }

    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class ChaosTeleport : AccessoryEffect
    {
        public override bool ActiveSkill => true;
        public override Header ToggleHeader => Header.GetHeader<ChaosForceHeader>();
        private int Cooldown = 60 * 25;
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            var FargoSOTSPlayer = player.GetModPlayer<SOTSEffectsPlayer>();
            Tile TeleportTile = Framing.GetTileSafely((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
            if (!stunned && !TeleportTile.HasTile && FargoSOTSPlayer.ChaosCharge >= Cooldown)
            {
                player.Teleport(new(Main.MouseWorld.X, Main.MouseWorld.Y), 1);
                FargoSOTSPlayer.ChaosCharge = 0;
            }
        }
        public override void PostUpdateEquips(Player player)
        {
            var FargoSOTSPlayer = player.GetModPlayer<SOTSEffectsPlayer>();
            if (player.ForceEffect<ChaosTeleport>())
                Cooldown = 60 * 40;
            if (FargoSOTSPlayer.ChaosCharge < Cooldown)
                FargoSOTSPlayer.ChaosCharge++;
            CooldownBarManager.Activate("ChaosTeleport", ModContent.Request<Texture2D>("FargoSoulsSOTS/Content/Items/Accessories/Enchantments/ElementalEnchant").Value, new(116, 122, 159),
                () => (float)FargoSOTSPlayer.ChaosCharge / Cooldown, true, activeFunction: player.HasEffect<ChaosTeleport>);
        }
    }

    [ExtendsFromMod(FargoSOTSCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(FargoSOTSCrossmod.SOTS.Name)]
    public class ElementalEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override void PostUpdateEquips(Player player)
        {
            var dissolvingPlayer = player.GetModPlayer<DissolvingElementsPlayer>();
            dissolvingPlayer.DissolvingNature = 0;
            dissolvingPlayer.DissolvingEarth = 0;
            dissolvingPlayer.DissolvingAurora = 0;
            dissolvingPlayer.DissolvingAether = 0;
            dissolvingPlayer.DissolvingDeluge = 0;
            dissolvingPlayer.DissolvingAurora = 0;
            dissolvingPlayer.DissolvingUmbra = 0;
            dissolvingPlayer.DissolvingNether = 0;
            dissolvingPlayer.DissolvingBrilliance = 0;
        }
    }
}