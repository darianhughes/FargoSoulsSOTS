using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FargoSoulsSOTS.Content.Items.ForceofVoid;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core.Toggler;

namespace FargoSoulsSOTS.Content.Items.ForceofSecrets
{
    public class SecretForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<WormwoodEnchant>(),
            ];
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int enchant in Enchants[Type])
            {
                recipe.AddIngredient(enchant);
            }
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }

    public class SecretEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public override int ToggleItemType => ModContent.ItemType<SecretForce>();
    }
}
