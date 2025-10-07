using SecretsOfTheSouls.Core.Players;
using SOTS.Items.Void;
using SOTS.Void;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Misc
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class GrayCrescent : ModItem
    {
        private SoundStyle GrayCrescentSound = SoundID.Item94;
        public override void SetDefaults()
        {
            Item.width = Item.height = 22;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.consumable = true;

            ItemID.Sets.ItemNoGravity[Type] = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<VioletStar>()
                .AddCondition(Condition.NearShimmer)
                .Register();

            CreateRecipe()
                .AddIngredient<ScarletStar>()
                .AddCondition(Condition.NearShimmer)
                .Register();
        }
        public override bool CanUseItem(Player player)
        {
            if (!CanUse(player) && player.altFunctionUse != 2)
            {
                return false;
            }
            if (!CanUse(player, true) && player.altFunctionUse == 2)
            {
                return false;
            }
            return true;
        }

        public override void HoldItem(Player player)
        {
            Item.UseSound = GrayCrescentSound;
        }

        public override bool? UseItem(Player player)
        {
            VoidPlayer vp = player.GetModPlayer<VoidPlayer>();

            if (vp.voidAnkh > 0)
            {
                if (player.altFunctionUse != 2)
                {
                    vp.voidMeterMax -= 20;
                    vp.voidAnkh--;
                }
            }
            else if (vp.voidSoul > 0)
            {
                if (player.altFunctionUse != 2)
                {
                    vp.voidMeterMax -= 50;
                    vp.voidSoul--;
                }
            }
            else if (vp.voidStar > 0)
            {
                if (player.altFunctionUse != 2)
                {
                    vp.voidMeterMax -= 50;
                    vp.voidStar--;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private bool CanUse(Player player, bool rightClick = false)
        {
            VoidPlayer vp = player.GetModPlayer<VoidPlayer>();
            SOTSEffectsPlayer mp = player.GetModPlayer<SOTSEffectsPlayer>();

            if (!rightClick && vp.voidMeterMax > 50)
            {
                return true;
            }

            if (rightClick && !mp.GrayCrescentVoid)
            {
                return true;
            }
            return false;
        }
    }
}
