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
using FargowiltasSouls.Core.Toggler.Content;
using SOTS.Items.AbandonedVillage;
using FargowiltasSouls.Core.Toggler;
using SOTS.Items;
using SOTS.Items.Permafrost;
using SOTS.Items.Tide;
using SOTS.Items.Evil;
using SOTS.Items.Conduit;

namespace SecretsOfTheSouls.Common.ItemChanges
{
    public class CrossmodAdditions
    {
        public static void UpdateMeleeSoul(Item item, Player player, bool hideVisual)
        {
            if (SecretsOfTheSoulsCrossmod.SOTS.Loaded)
                SOTSAddtions.UpdateMeleeSoul(item, player, hideVisual);
        }

        public static void UpdateRangedSoul(Item item, Player player, bool hideVisual)
        {
            if (SecretsOfTheSoulsCrossmod.SOTS.Loaded)
                SOTSAddtions.UpdateRangerSoul(item, player, hideVisual);
        }

        public static void UpdateUniverseSoul(Item item, Player player, bool hideVisual)
        {
            //Berserker
            UpdateMeleeSoul(item, player, hideVisual);

            //Sniper
            UpdateRangedSoul(item, player, hideVisual);
        }

        public static void UpdateSupersonic(Item item, Player player, bool hideVisual)
        {
            if (SecretsOfTheSoulsCrossmod.SOTS.Loaded)
                SOTSAddtions.UpdateSupersonic(item, player, hideVisual);
            if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
                ConsolariaAddtions.UpdateSupersonic(item, player, hideVisual);
        }

        public static void UpdateEternitySoul(Item item, Player player, bool hideVisual)
        {
            if (SecretsOfTheSoulsCrossmod.SOTS.Loaded)
                SOTSAddtions.UpdateEternitySoul(item, player, hideVisual);
            if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
                ConsolariaAddtions.UpdateSupersonic(item, player, hideVisual);
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSAddtions
    {
        private static Mod sots = ModLoader.GetMod("SOTS");
        private static Mod secretSouls = ModLoader.GetMod("SecretsOfTheSouls");
        private static void GetPlayers(Player player, out DoorPlayer dp, out SOTSPlayer sotsPlayer, out VoidPlayer voidPlayer)
        {
            dp = player.GetModPlayer<DoorPlayer>();
            sotsPlayer = SOTSPlayer.ModPlayer(player);
            voidPlayer = VoidPlayer.ModPlayer(player);
        }

        public static void UpdateMeleeSoul(Item item, Player player, bool hideVisual)
        {
            GetPlayers(player, out _, out SOTSPlayer sotsPlayer, out VoidPlayer voidPlayer);

            player.AddEffect<SupernovaSeekersEffect>(item);

            voidPlayer.bonusVoidGain += 2f;
            voidPlayer.voidMeterMax2 += 50;
            voidPlayer.GainVoidOnHurt += 0.12f;

            voidPlayer.voidCost -= 0.1f;
            voidPlayer.CrushResistor = true;
            voidPlayer.CrushCapacitor = true;
            ++voidPlayer.BonusCrushRangeMax;
            ++voidPlayer.BonusCrushRangeMin;
        }

        public static void UpdateRangerSoul(Item item, Player player, bool hideVisual)
        {
            GetPlayers(player, out _, out SOTSPlayer sotsPlayer, out VoidPlayer voidPlayer);

            player.AddEffect<RippleWavesEffect>(item);
            player.AddEffect<NightmareArmsEffect>(item);

            player.AddEffect<BackupBowEffect>(item);
            sotsPlayer.backUpBowVisual = !hideVisual;

            player.AddEffect<InfinityPouchEffect>(item);

            player.AddEffect<PetAdvisorEffect>(item);
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

        public static void UpdateWorldShaper(Item item, Player player, bool hideVisual)
        {
            GetPlayers(player, out DoorPlayer dp, out SOTSPlayer sotsPlayer, out VoidPlayer voidPlayer);

            sotsPlayer.GoldenTrowel = true;
            sotsPlayer.KeepersBox = true;
            ++sotsPlayer.DamageGenerateMoney;
            player.VoidPlayer().VoidGenerateMoney += 2f;

            sotsPlayer.DrillHandVanity = !hideVisual;
            player.AddEffect<MiningMode>(item);

            SOTSPlayer.ModPlayer(player).ConduitBelt = true;

            player.SOTSPlayer().Lockpick = true;

            player.AddEffect<EarthenEffect>(item);
        }

        public static void UpdateTrawlerSoul(Item item, Player player, bool hideVisual)
        {
            player.AddEffect<TwilightFishingEffect>(item);
            player.AddEffect<ZombieHandEffect>(item);
        }

        public static void ProbesGen(Player player, Item item)
        {
            player.SOTSPlayer().artifactProbeDamage += SOTSPlayer.ApplyDamageClassModWithGeneric(player, item.DamageType, item.damage);
            player.SOTSPlayer().artifactProbeNum += 8;
        }

        public static void UpdateDevilshield(Item item, Player player, bool hideVisual, bool isColossus = false)
        {
            if (!isColossus)
            {
                item.DamageType = DamageClass.Summon;
                item.damage = 40;
            }

            GetPlayers(player, out _, out SOTSPlayer sotsPlayer, out _);

            if (player.AddEffect<ShatterHeartEffect>(item))
            {
                int num = Main.rand.Next(10);
                if (num >= 0 && num <= 1)
                    sotsPlayer.shardOnHit += 3;
                if (num >= 2 && num <= 4)
                    sotsPlayer.shardOnHit += 4;
                if (num >= 5)
                    sotsPlayer.shardOnHit += 5;
                sotsPlayer.bonusShardDamage += SOTSPlayer.ApplyDamageClassModWithGeneric(player, item.DamageType, item.damage);
            }

            if (player.AddEffect<PermafrostMedallionEffect>(item))
                ProbesGen(player, item);
        }

        public static void UpdateColossusSoul(Item item, Player player, bool hideVisual)
        {
            GetPlayers(player, out _, out SOTSPlayer sotsPlayer, out VoidPlayer voidPlayer);

            UpdateDevilshield(item, player, hideVisual);

            item.DamageType = DamageClass.Generic;
            item.damage = 40;

            voidPlayer.bonusVoidGain += 2f;
            if (player.AddEffect<BulwarkEffect>(item))
            {
                int num = SOTSPlayer.ApplyDamageClassModWithGeneric(player, DamageClass.Generic, item.damage);
                sotsPlayer.tPlanetDamage += num;
                sotsPlayer.tPlanetNum += 2;
                sotsPlayer.aqueductDamage += num;
                sotsPlayer.aqueductNum += 2;
            }

            sotsPlayer.additionalHeal += 100;
            sotsPlayer.additionalPotionMana += 100;
            sotsPlayer.PotionBuffDegradeRate -= 0.4f;

            sotsPlayer.baguetteDrops = true;
        }

        public static void UpdateMicroverseSoul(Item item, Player player, bool hideVisual)
        {
            if (SecretsOfTheSoulsConfig.Instance.UnfinishedContent)
            {
                ModContent.GetInstance<ChaosForce>().UpdateAccessory(player, hideVisual);
                ModContent.GetInstance<SpaceForce>().UpdateAccessory(player, hideVisual);
            }
        }

        public static void UpdateTerrariaSoul(Item item, Player player, bool hideVisual)
        {
            if (!SecretsOfTheSoulsCrossmod.CommunitySoulsExpansion.Loaded && !SecretsOfTheSoulsConfig.Instance.UnfinishedContent)
            {
                ModContent.GetInstance<ChaosForce>().UpdateAccessory(player, hideVisual);
                ModContent.GetInstance<SpaceForce>().UpdateAccessory(player, hideVisual);
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
            UpdateTrawlerSoul(item, player, hideVisual);

            //World Shapter
            UpdateWorldShaper(item, player, hideVisual);

            //Terraria Soul
            UpdateTerrariaSoul(item, player, hideVisual);

            //Subspace Voyager Soul
            ModItem SVS = secretSouls.Find<ModItem>("SubspaceVoyagerSoul");
            SVS.UpdateAccessory(player, hideVisual);
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public class ConsolariaAddtions
    {
        private static Mod consolaria = SecretsOfTheSoulsCrossmod.Consolaria.Mod;
        public static void UpdateSupersonic(Item item, Player player, bool hideVisual)
        {
            ModItem shadowboundExo = consolaria.Find<ModItem>("ShadowboundExoskeleton");
            shadowboundExo.UpdateAccessory(player, hideVisual);
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class MiningMode : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<WorldShaperHeader>();
        public override int ToggleItemType => ModContent.ItemType<DrillHand>();
        public override void PostUpdateEquips(Player player)
        {
            SOTSPlayer sotsPlayer = player.SOTSPlayer();
            sotsPlayer.DrillHand = true;
            if (player.HeldItem.pick > 0)
            {
                sotsPlayer.Pick3x3 = true;
                sotsPlayer.bonusPickaxePower -= 10;
                player.pickSpeed += 0.25f;
            }
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class ZombieHandEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<TrawlerHeader>();
        public override int ToggleItemType => ModContent.ItemType<ZombieHand>();

        public override void PostUpdateEquips(Player player)
        {
            SOTSPlayer.ModPlayer(player).CanKillNPC = true;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class PermafrostMedallionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
        public override int ToggleItemType => ModContent.ItemType<PermafrostMedallion>();
        public override bool MinionEffect => true;
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class ShatterHeartEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
        public override int ToggleItemType => ModContent.ItemType<ShatterHeartShield>();
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class BulwarkEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ColossusHeader>();
        public override int ToggleItemType => ModContent.ItemType<BulwarkOfTheAncients>();
        public override bool MinionEffect => true;
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SupernovaSeekersEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<UniverseHeader>();
        public override int ToggleItemType => ModContent.ItemType<SupernovaEmblem>();
        public override bool ExtraAttackEffect => true;

        public override void PostUpdateEquips(Player player)
        {
            player.SOTSPlayer().SupernovaEmblem = true;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class RippleWavesEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<UniverseHeader>();
        public override int ToggleItemType => ModContent.ItemType<PrismarineNecklace>();

        public override void PostUpdateEquips(Player player)
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            ++sotsPlayer.rippleTimer;
            sotsPlayer.rippleBonusDamage += 10;
            sotsPlayer.rippleEffect = true;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class NightmareArmsEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<UniverseHeader>();
        public override int ToggleItemType => ModContent.ItemType<WitchHeart>();
        public override bool ExtraAttackEffect => true;

        public override void PostUpdateEquips(Player player)
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            sotsPlayer.CritNightmare = true;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class BackupBowEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<UniverseHeader>();
        public override int ToggleItemType => ModContent.ItemType<BackupBow>();
        public override bool ExtraAttackEffect => true;

        public override void PostUpdateEquips(Player player)
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            sotsPlayer.backUpBow = true;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class InfinityPouchEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<UniverseHeader>();
        public override int ToggleItemType => ModContent.ItemType<InfinityPouch>();

        public override void PostUpdateEquips(Player player)
        {
            player.SOTSPlayer().InfinityPouch = true;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class PetAdvisorEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<UniverseHeader>();
        public override int ToggleItemType => ModContent.ItemType<InfinityPouch>();
        public override bool MinionEffect => true;

        public override void PostUpdateEquips(Player player)
        {
            SOTSPlayer sotsPlayer = SOTSPlayer.ModPlayer(player);
            sotsPlayer.typhonRange = 96;
            sotsPlayer.petAdvisor = true;
        }
    }
}
