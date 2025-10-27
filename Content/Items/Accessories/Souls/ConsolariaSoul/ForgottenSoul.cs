using System.Collections.Generic;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using SecretsOfTheSouls.Content.Items.Accessories.Forces.ConsolariaForce;
using Terraria.ID;
using FargowiltasSouls.Core.ModPlayers;
using FargowiltasSouls;
using Fargowiltas.Content.Items.Tiles;
using FargowiltasSouls.Content.Items.Materials;
using Terraria.Localization;
using FargowiltasSouls.Content.Items;

namespace SecretsOfTheSouls.Content.Items.Accessories.Souls.ConsolariaSoul
{
    public class ForgottenSoul : BaseSoul
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return SecretsOfTheSoulsConfig.Instance.UnfinishedContent && (SecretsOfTheSoulsCrossmod.Consolaria.Loaded 
                //|| SecretsOfTheSoulsCrossmod.RiseOfAges.Loaded
                ); 
        }

        public static List<int> Forces
        {
            get
            {
                List<int> forces = new List<int>();
                if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
                {
                    forces.Add(ModContent.ItemType<MightForce>());
                }

                return forces;
            }
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, 24));
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = 5000000;
            Item.rare = ItemRarityID.Expert;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            foreach (int force in Forces)
                modPlayer.ForceEffects.Add(force);

            if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
                ModContent.GetInstance<MightForce>().UpdateAccessory(player, hideVisual);

            if (SecretsOfTheSoulsCrossmod.Heartbeataria.Loaded)
            {
                ModItem otherworldCore =SecretsOfTheSoulsCrossmod.Heartbeataria.Mod.Find<ModItem>("OtherworldCore");
                otherworldCore.UpdateAccessory(player, hideVisual);
            }
        }

        public override void SafeModifyTooltips(List<TooltipLine> tooltips)
        {
            string debuffs = "";
            string ruminateDebuff = "";
            string forces = "";
            string modNames = "";
            string ruminateForces = "";
            string other = "";
            string ruminateOther = "";

            if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
            {
                forces += $"[i:{Mod.Name}/MightForce]";
                modNames += SecretsOfTheSoulsCrossmod.Consolaria.Mod.Name;
                ruminateForces += $"[i:{Mod.Name}/MightForce] Force of Might effects :D";
            }
            if (SecretsOfTheSoulsCrossmod.Heartbeataria.Loaded)
            {
                other += $"[i:{SecretsOfTheSoulsCrossmod.Heartbeataria.Name}/OtherworldCore]";
                ruminateOther += $"[i:{SecretsOfTheSoulsCrossmod.Heartbeataria.Name}/OtherworldCore] {Language.GetTextValue("Mods.XDContentMod.Items.OtherworldCore.Tooltip")}";
            }

            if (IsNotRuminating(Item))
            {
                if (!string.IsNullOrEmpty(debuffs))
                    tooltips.Add(new(Mod, "Debuffs", Language.GetTextValue("Mods.SecretsOfTheSouls.Items.ForgottenSoul.Debuffs", debuffs)));
                if (!string.IsNullOrEmpty(forces))
                {
                    tooltips.Add(new(Mod, "Forces", Language.GetTextValue("Mods.SecretsOfTheSouls.Items.ForgottenSoul.Force", forces, modNames)));
                }
                if (!string.IsNullOrEmpty(other))
                {
                    tooltips.Add(new(Mod, "Other", Language.GetTextValue("Mods.SecretsOfTheSouls.Items.ForgottenSoul.Other", other)));
                }
            }
            else
            {
                //if (!string.IsNullOrEmpty(ruminateDebuff))
                if (!string.IsNullOrEmpty(ruminateForces))
                    tooltips.Add(new(Mod, "Forces", ruminateForces));
                if (!string.IsNullOrEmpty(ruminateOther))
                    tooltips.Add(new(Mod, "Other", ruminateOther));
            }
            tooltips.Add(new(Mod, "Splash", Language.GetTextValue("Mods.SecretsOfTheSouls.Items.ForgottenSoul.Splash")));
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int force in Forces)
                recipe.AddIngredient(force);
            if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
            {
                
            }
            if (SecretsOfTheSoulsCrossmod.Heartbeataria.Loaded)
            {
                recipe.AddIngredient(SecretsOfTheSoulsCrossmod.Heartbeataria.Mod.Find<ModItem>("OtherworldCore").Type);
            }
            recipe.AddIngredient<AbomEnergy>(10);
            recipe.AddTile<CrucibleCosmosSheet>();
            recipe.Register();
        }
    }
}
