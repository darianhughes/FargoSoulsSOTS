using System.Collections.Generic;
using FargoSoulsSOTS.Content.Buffs.Emode;
using FargoSoulsSOTS.Core.Players;
using FargowiltasSouls;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Accessories.Expert;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SOTS.Items.Permafrost;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Items.Accessories.Masomode
{
    public class GadgetCoat : SoulsItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargoSOTSConfig.Instance.UnfinishedContent;
        }

        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

        public override List<AccessoryEffect> ActiveSkillTooltips => [AccessoryEffectLoader.GetEffect<PlasmaHook>()];
        public override bool Eternity => true;

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                EquipLoader.AddEquipTexture(Mod, "FargoSoulsSOTS/Content/Items/Accessories/Masomode/DrillCap_Face", EquipType.Head, this);
            }
        }

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 6);

            if (Main.netMode != NetmodeID.Server)
            {
                int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
                ArmorIDs.Face.Sets.PreventHairDraw[Item.faceSlot] = true;
            }
        }

        int counter;

        void PassiveEffect(Player player)
        {
            player.FargoSouls().BoxofGizmos = true;
            if (player.whoAmI == Main.myPlayer && player.FargoSouls().IsStandingStill && player.itemAnimation == 0 && player.HeldItem != null)
            {
                if (++counter > 60)
                {
                    player.detectCreature = true;

                    if (counter % 10 == 0)
                        Main.instance.SpelunkerProjectileHelper.AddSpotToCheck(player.Center);
                }
            }
            else
            {
                counter = 0;
            }
        }

        public override void UpdateInventory(Player player) => PassiveEffect(player);
        public override void UpdateVanity(Player player) => PassiveEffect(player);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //Box of Gizmos
            PassiveEffect(player);

            //Jelly Jumpers
            player.buffImmune[ModContent.BuffType<Corrosion>()] = true;

            player.extraFall += 10;
            player.autoJump = true;

            player.AddEffect<JellyJumpersEffect>(Item);

            //Drill Cap
            player.buffImmune[ModContent.BuffType<Grounded>()] = true;
            player.buffImmune[ModContent.BuffType<LowGroundBuff>()] = true;

            player.AddEffect<DrillCapEffect>(Item);

            //Plasma Grasp
            player.buffImmune[BuffID.Electrified] = true;

            player.AddEffect<PlasmaHook>(Item);

            //Cooled Fidget Spiner

            //Gadget Coat
            FargoSOTSPlayer mp = player.GetModPlayer<FargoSOTSPlayer>();
            mp.GadgetCoat = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BoxofGizmos>()
                .AddIngredient<JellyJumpers>()
                .AddIngredient<DrillCap>()
                .AddIngredient<PlasmaGrasp>()
                //.AddIngredient<CooledFidgetSpiner>()
                .AddIngredient<AbsoluteBar>(5)
                .AddIngredient(ItemID.MartianConduitPlating, 30)
                .AddIngredient<DeviatingEnergy>(10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
