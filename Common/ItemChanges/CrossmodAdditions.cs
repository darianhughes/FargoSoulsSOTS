using FargowiltasSouls.Content.Items.Accessories.Souls;
using SOTS.Items.DoorItems;
using SOTS.Items.Wings;
using SOTS.Void;
using SOTS;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using SecretsOfTheSouls.Content.Items.Accessories.Enchantments.SOTSEnchant;
using SecretsOfTheSouls.Content.Items.Accessories.Forces.SOTSForce;

namespace SecretsOfTheSouls.Common.ItemChanges
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSAddtions
    {
        private static Mod sots = ModLoader.GetMod("SOTS");
        private static void GetPlayers(Player player, out DoorPlayer dp, out SOTSPlayer sotsPlayer, out VoidPlayer voidPlayer)
        {
            dp = player.GetModPlayer<DoorPlayer>();
            sotsPlayer = SOTSPlayer.ModPlayer(player);
            voidPlayer = VoidPlayer.ModPlayer(player);
        }

        public static void UpdateSupersonic(Item item, Player player, bool hideVisual)
        {
            player.AddEffect<BandofDoorEffect>(item);

            if (player.AddEffect<FlashsparkEffect>(item))
            {
                ModItem sb = sots.Find<ModItem>("SubspaceBoosters");

                sb.UpdateAccessory(player, hideVisual);

                //remove extra things added
                player.lavaMax -= 600;
                if (player.HasEffect<SupersonicRocketBoots>())
                    player.rocketBoots = player.vanityRocketBoots = ArmorIDs.RocketBoots.TerrasparkBoots;
                else
                {
                    player.rocketBoots = 0;
                }
                player.moveSpeed -= 0.2f;
                player.accRunSpeed = player.HasEffect<RunSpeed>() ? 15.6f : 6.75f;
            }
        }

        public static void UpdateFlightMastery(Item item, Player player, bool hideVisual)
        {
            GetPlayers(player, out DoorPlayer dp, out SOTSPlayer sotsPlayer, out VoidPlayer voidPlayer);

            voidPlayer.bonusVoidGain += 3f;
            voidPlayer.voidRegenSpeed += 0.25f;
            sotsPlayer.SpiritSymphony = true;
            MachinaBoosterPlayer modPlayer = player.GetModPlayer<MachinaBoosterPlayer>();
            int num;
            bool flag = (num = 1) != 0;
            modPlayer.CreativeFlightTier2 = num != 0;
            modPlayer.canCreativeFlight = flag;

            //player.AddEffect<GravityAnchorEffect>(item);
            //player.noKnockback = true;
        }

        public static void UpdateMicroverseSoul(Item item, Player player, bool hideVisual)
        {
            if (SecretsOfTheSoulsConfig.Instance.UnfinishedContent)
            {
                ModContent.GetInstance<ChaosForce>().UpdateAccessory(player, hideVisual);
                ModContent.GetInstance<SpaceForce>().UpdateAccessory(player, hideVisual);
            }
            else
            {
                ModContent.GetInstance<VoidForce>().UpdateAccessory(player, hideVisual);
            }
        }

        public static void UpdateTerrariaSoul(Item item, Player player, bool hideVisual)
        {
            if (SecretsOfTheSoulsConfig.Instance.UnfinishedContent)
            {
                if (!SecretsOfTheSoulsCrossmod.CommunitySoulsExpansion.Loaded)
                {
                    ModContent.GetInstance<ChaosForce>().UpdateAccessory(player, hideVisual);
                    ModContent.GetInstance<SpaceForce>().UpdateAccessory(player, hideVisual);
                }
            }
            else
            {
                ModContent.GetInstance<VoidForce>().UpdateAccessory(player, hideVisual);
            }
        }

        public static void UpdateEternitySoul(Item item, Player player, bool hideVisual)
        {
            GetPlayers(player, out DoorPlayer dp, out SOTSPlayer sotsPlayer, out VoidPlayer voidPlayer);

            //Supersonic
            UpdateSupersonic(item, player, hideVisual);

            //Flight Mastery
            UpdateFlightMastery(item, player, hideVisual);

            //Trawler
            //player.AddEffect<TwilightFishingEffect>(item);

            //World Shapter
            player.AddEffect<EarthenEffect>(item);

            //Terraria Soul
            UpdateTerrariaSoul(item, player, hideVisual);
        }
    }
}
