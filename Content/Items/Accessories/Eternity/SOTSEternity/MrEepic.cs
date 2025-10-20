using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FargowiltasSouls.Content.Items;
using FargowiltasSouls.Content.Items.Materials;
using SOTS;
using SOTS.Items;
using SOTS.Items.Permafrost;
using SOTS.Void;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Content.Items.Accessories.Eternity.SOTSEternity
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class MrEepic : SoulsItem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return SecretsOfTheSoulsConfig.Instance.UnfinishedContent;
        }
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";

        public override void SetStaticDefaults()
        {
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 20;
            //Item.width = ;
            //Item.height = ;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<AnomalyRarity>();
            Item.value = Item.sellPrice(0, 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.SOTSPlayer().Dreamcatcher = true;
            player.SOTSPlayer().MrBurns = true;
            player.VoidPlayer().voidMeterMax2 += 125;
            player.VoidPlayer().voidRegenSpeed += 0.5f;
            ref StatModifier local = ref player.GetDamage<VoidGeneric>();
            local += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MrEepy>()
                .AddIngredient<MrGlorp>()
                .AddIngredient<MrMcMillen>()
                .AddIngredient<AbsoluteBar>(10)
                .AddIngredient<DeviatingEnergy>(10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}
