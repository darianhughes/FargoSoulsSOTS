using System.Collections.Generic;
using SOTS.NPCs.Boss;
using SOTS.NPCs.Boss.Polaris;
using SOTS.NPCs.Boss.Lux;
using SOTS.NPCs.Boss.Advisor;
using SOTS.NPCs.Boss.Curse;
using SOTS.NPCs.Boss.Glowmoth;
using Terraria;
using Terraria.ModLoader;
using SOTS.NPCs.Boss.Excavator;
using SOTS.NPCs.Boss.Polaris.NewPolaris;
using FargowiltasSouls.Core.Systems;
using Terraria.DataStructures;
using FargowiltasSouls.Core.ItemDropRules.Conditions;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using SOTS.Items.Fishing;
using FargowiltasSouls;
using SOTS.Items.AbandonedVillage;
using SOTS.Items.Slime;
using SOTS.NPCs.Phase;
using SOTS.NPCs;
using SOTS.NPCs.Inferno;
using SOTS.NPCs.Constructs;
using SOTS.NPCs.AbandonedVillage;
using SOTS.NPCs.Tide;
using SOTS.NPCs.Gizmos;
using SOTS.NPCs.Chaos;
using SOTS.NPCs.TreasureSlimes;
using SOTS.Items.Potions;
using SecretsOfTheSouls.Content.Buffs.Emode.SOTSBuffs;
using Fargowiltas.Content.NPCs;
using SecretsOfTheSouls.Content.Items.Accessories.Eternity.SOTSEternity;
using SOTS;
using Terraria.Localization;
using Consolaria.Content.NPCs.Bosses.Lepus;
using Consolaria.Common.ModSystems;
using Consolaria.Content.NPCs.Bosses.Turkor;
using Consolaria.Content.NPCs.Bosses.Ocram;
using Microsoft.CodeAnalysis;

namespace SecretsOfTheSouls.Common.NPCChanges
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSNPCChanges : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private const float eternityHPMult = 1.10f;
        private const float eternityDamageMult = 1.10f;
        private const float masoHPMult = 1.25f;
        private const float masoDamageMult = 1.20f;
        private const float masoDamageReductionMult = 0.90f;

        private static readonly HashSet<int> SotsBossIds = new()
        {
            ModContent.NPCType<Glowmoth>(),
            ModContent.NPCType<PutridPinkyPhase2>(),
            ModContent.NPCType<PharaohsCurse>(),
            ModContent.NPCType<Excavator>(),
            ModContent.NPCType<TheAdvisorHead>(),
            ModContent.NPCType<Polaris>(),
            ModContent.NPCType<NewPolaris>(),
            ModContent.NPCType<Lux>(),
            ModContent.NPCType<SubspaceSerpentHead>(),
        };

        private static readonly Dictionary<int, (float emHp, float emDmg, float masoHp, float masoDmg)> Overrides
            = new()
            {
                // [ModContent.NPCType<TheAdvisorHead>()] = (1.12f, 1.12f, 1.30f, 1.22f), (EXAMPLE)
                [ModContent.NPCType<NewPolaris>()] = (1.08f, eternityDamageMult, 1.20f, masoDamageMult)
            };

        private static bool IsSotsBoss(int type) => SotsBossIds.Contains(type);

        private static void GetMults(int type, out float emHp, out float emDmg, out float masoHp, out float masoDmg)
        {
            if (Overrides.TryGetValue(type, out var t))
            {
                emHp = t.emHp; emDmg = t.emDmg; masoHp = t.masoHp; masoDmg = t.masoDmg;
            }
            else
            {
                emHp = eternityHPMult; emDmg = eternityDamageMult; masoHp = masoHPMult; masoDmg = masoDamageMult;
            }
        }

        public override void SetDefaults(NPC npc)
        {
            bool eternity = WorldSavingSystem.EternityMode;
            bool maso = WorldSavingSystem.MasochistModeReal;

            if (npc.type == ModContent.NPCType<Glowmoth>())
                npc.buffImmune[ModContent.BuffType<Lepidopterism>()] = true;

            if (npc.type == ModContent.NPCType<NewPolaris>())
                npc.buffImmune[ModContent.BuffType<CryomagneticDisruption>()] = true;

            if (npc.type == ModContent.NPCType<BulletSnakeHead>())
                npc.lifeMax = (int)(npc.lifeMax * (maso ? 1.75f : 1.5f));

            if (!npc.boss || !IsSotsBoss(npc.type))
                return;

            if (!eternity) return;

            GetMults(npc.type, out float emHp, out _, out float masoHp, out _);

            float hpMul = emHp * (maso ? masoHp : 1f);
            if (hpMul != 1f)
            {
                npc.lifeMax = (int)(npc.lifeMax * hpMul);
                if (npc.lifeMax < 1) npc.lifeMax = 1;
            }

            GetMults(npc.type, out _, out float emDmg, out _, out float masoDmg);
            float dmgMul = emDmg * (maso ? masoDmg : 1f);
            npc.damage = (int)(npc.damage * dmgMul);
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            if (npc.type == ModContent.NPCType<GlowmothMinion>() && WorldSavingSystem.EternityMode)
            {
                if (WorldSavingSystem.MasochistModeReal)
                {
                    target.AddBuff(BuffID.Poisoned, 60 * 5);
                    target.AddBuff(BuffID.Darkness, 60 * 3);
                }

                target.AddBuff(ModContent.BuffType<Lepidopterism>(), (WorldSavingSystem.MasochistModeReal ? 5 : 3) * 60);
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (!npc.boss || !IsSotsBoss(npc.type))
                return;

            if (!WorldSavingSystem.EternityMode)
                return;

            bool maso = WorldSavingSystem.MasochistModeReal;
            GetMults(npc.type, out _, out float emDmg, out _, out float masoDmg);

            float mul = emDmg * (maso ? masoDmg : 1f);
            if (mul != 1f)
                modifiers.SourceDamage *= mul;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (!npc.boss || !IsSotsBoss(npc.type))
                return;

            if (WorldSavingSystem.EternityMode && WorldSavingSystem.MasochistModeReal && masoDamageReductionMult != 1f)
                modifiers.FinalDamage *= masoDamageReductionMult;
        }

        public override void OnSpawn(NPC npc, IEntitySource source) { }

        #region SOTS Potion Dropping
        public static List<int> DropsAssassinationPotion =
        [
            ModContent.NPCType<PhaseAssaulterHead>(),
            NPCID.GoblinThief,
            ModContent.NPCType<TwilightDevil>()
        ];

        public static List<int> DropsBlueFirePotion =
        [
            NPCID.DungeonSpirit,
            ModContent.NPCType<LesserWisp>()
        ];

        public static List<int> DropsBrittlePotion =
        [
            NPCID.IceGolem,
            NPCID.IceElemental,
            ModContent.NPCType<PermafrostConstruct>(),
            ModContent.NPCType<PermafrostSpirit>(),
            NPCID.IceQueen,
            NPCID.SnowBalla,
            NPCID.SnowmanGangsta,
            NPCID.MisterStabby
        ];

        public static List<int> DropsDoubleVisionPotion =
        [
            ModContent.NPCType<Ghast>(),
            ModContent.NPCType<Maligmor>(),
            ModContent.NPCType<FlamingGhast>(),
            ModContent.NPCType<BleedingGhast>(),
            ModContent.NPCType<Teratoma>(),
            NPCID.AncientCultistSquidhead,
        ];

        public static List<int> DropsHarmonyPotion =
        [
            NPCID.ChaosElemental,
            ModContent.NPCType<PhaseSpeeder>(),
            NPCID.IlluminantSlime
        ];

        public static List<int> DropsNightmarePotion =
        [
            NPCID.Wraith,
            NPCID.DesertGhoulCorruption,
            NPCID.DesertGhoulCrimson,
            NPCID.FaceMonster,
            NPCID.SandsharkCorrupt,
            NPCID.SandsharkCrimson,
            NPCID.Eyezor,
            NPCID.ThePossessed,
            NPCID.Reaper,
            ModContent.NPCType<Throe>(),
            ModContent.NPCType<Famished>(),
            ModContent.NPCType<Fistfull>(),
            ModContent.NPCType<Pupa>(),
            ModContent.NPCType<RotWalker>(),
        ];

        public static List<int> DropsRipplePotion =
        [
            ModContent.NPCType<TidalSpirit>(),
            ModContent.NPCType<TidalConstruct>(),
            ModContent.NPCType<PhantarayBig>(),
            ModContent.NPCType<PhantarayCore>(),
            NPCID.ZombieMerman,
            NPCID.CreatureFromTheDeep
        ];

        public static List<int> DropsRoughSkinPotion =
        [
            ModContent.NPCType<Snake>(),
            NPCID.RockGolem,
            ModContent.NPCType<EarthenGizmo>(),
            ModContent.NPCType<CoalCart>(),
            NPCID.Crawdad,
            ModContent.NPCType<Chimera>(),
            NPCID.Crawdad2,
            NPCID.Lihzahrd,
            NPCID.FlyingSnake,
        ];

        public static List<int> DropsSoulAccessPotion =
        [
            ModContent.NPCType<LostSoul>(),
            NPCID.EaterofSouls,
            NPCID.BigEater,
            NPCID.LittleEater,
            NPCID.DesertDjinn,
        ];

        public static List<int> DropsVibePotion =
        [
            NPCID.Dandelion,
            ModContent.NPCType<NatureSlime>(),
            ModContent.NPCType<NatureConstruct>(),
            ModContent.NPCType<NatureSpirit>(),
        ];

        public static List<int> DropsVigorPotion =
        [
            NPCID.BoneLee,
            ModContent.NPCType<VoidTreasureSlime>(),
        ];
        #endregion

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            LeadingConditionRule emodeRule = new(new EModeDropCondition());

            #region Crates & Emode Accessories
            if (npc.type == ModContent.NPCType<Glowmoth>())
            {
                emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<GlowBulb>()));
                //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.IronCrate, 3));
            }
            if (npc.type == ModContent.NPCType<PutridPinkyPhase2>())
            {
                emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<JellyJumpers>()));
                //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.IronCrate, 5));
            }
            if (npc.type == ModContent.NPCType<PharaohsCurse>())
            {
                emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<AnkhoftheCursedRuler>()));
                //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<PyramidCrate>(), 5));
            }
            if (npc.type == ModContent.NPCType<Excavator>())
            {
                emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<DrillCap>()));
                //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(WorldGen.crimson ? ItemID.CrimsonFishingCrate : ItemID.CorruptFishingCrate, 5));
            }
            if (npc.type == ModContent.NPCType<TheAdvisorHead>())
            {
                //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<PlanetariumCrate>(),5));
            }
            if (npc.type == ModContent.NPCType<Polaris>() || npc.type == ModContent.NPCType<NewPolaris>())
            {
                //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.FrozenCrateHard, 5));
            }
            if (npc.type == ModContent.NPCType<Lux>())
            {
                //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.HallowedFishingCrateHard, 5));
            }
            if (npc.type == ModContent.NPCType<SubspaceSerpentHead>())
            {
                //emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.LavaCrateHard, 5));
            }
            #endregion

            #region Tim's Concotion drop
            void TimsConcoctionDrop(IItemDropRule rule)
            {
                TimsConcoctionDropCondition dropCondition = new();
                IItemDropRule conditionalRule = new LeadingConditionRule(dropCondition);
                conditionalRule.OnSuccess(rule);
                npcLoot.Add(conditionalRule);
            }
            if (DropsAssassinationPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<AssassinationPotion>(), 1, 1, 6));
            }
            if (DropsBlueFirePotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<BluefirePotion>(), 1, 1, 6));
            }
            if (DropsBrittlePotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<BrittlePotion>(), 1, 1, 6));
            }
            if (DropsDoubleVisionPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<DoubleVisionPotion>(), 1, 1, 3));
            }
            if (DropsHarmonyPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<HarmonyPotion>(), 1, 1, 3));
            }
            if (DropsNightmarePotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<NightmarePotion>(), 1, 1, 3));
            }
            if (DropsRipplePotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<RipplePotion>(), 1, 1, 6));
            }
            if (DropsRoughSkinPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<RoughskinPotion>(), 1, 1, 6));
            }
            if (DropsSoulAccessPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<SoulAccessPotion>(), 1, 1, 3));
            }
            if (DropsVibePotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<VibePotion>(), 1, 1, 6));
            }
            if (DropsVigorPotion.Contains(npc.type))
            {
                TimsConcoctionDrop(ItemDropRule.Common(ModContent.ItemType<VigorPotion>(), 1, 1, 1));
            }
            #endregion

            npcLoot.Add(emodeRule);
        }

        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == ModContent.NPCType<LumberJack>())
            {
                shop.Add(new Item(ModContent.ItemType<CharredWood>()) { shopCustomPrice = Item.buyPrice(copper: 20) });
                shop.Add(new Item(ModContent.ItemType<Wormwood>()) { shopCustomPrice = Item.buyPrice(copper: 23) }, Condition.DownedKingSlime);
            }
            base.ModifyShop(shop);
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public class SOTSEmodeFirstKillDrop : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            List<IItemDropRule> rules = [];

            if (npc.type == ModContent.NPCType<Glowmoth>())
            {
                rules.Add(FirstKillDrop(5, ItemID.IronCrate));
            }
            else if (npc.type == ModContent.NPCType<PutridPinkyPhase2>())
            {
                rules.Add(FirstKillDrop(5, ItemID.IronCrate));
            }
            else if (npc.type == ModContent.NPCType<PharaohsCurse>())
            {
                rules.Add(FirstKillDrop(5, ModContent.ItemType<PyramidCrate>()));
            }
            else if (npc.type == ModContent.NPCType<Excavator>())
            {
                rules.Add(FirstKillDrop(5, WorldGen.crimson ? ItemID.CrimsonFishingCrate : ItemID.CorruptFishingCrate));
            }
            else if (npc.type == ModContent.NPCType<TheAdvisorHead>())
            {
                rules.Add(FirstKillDrop(5, ModContent.ItemType<PlanetariumCrate>()));
            }
            else if (npc.type == ModContent.NPCType<Polaris>() || npc.type == ModContent.NPCType<NewPolaris>())
            {
                rules.Add(FirstKillDrop(5, ItemID.FrozenCrateHard));
            }
            else if (npc.type == ModContent.NPCType<Lux>())
            {
                rules.Add(FirstKillDrop(5, ItemID.HallowedFishingCrateHard));
            }
            else if (npc.type == ModContent.NPCType<SubspaceSerpentHead>())
            {
                rules.Add(FirstKillDrop(5, ItemID.LavaCrateHard));
            }

            foreach (var rule in rules)
            {
                npcLoot.Add(rule);
            }
        }

        private static IItemDropRule Drop(int count, int itemID) => ItemDropRule.Common(itemID, minimumDropped: count, maximumDropped: count);

        public static IItemDropRule FirstKillDrop(int amount, int itemID)
        {
            IItemDropRule rule = new LeadingConditionRule(new CrossmodFirstKillCondition());
            rule.OnSuccess(Drop(amount, itemID));
            return rule;
        }
    }

    public class CrossmodFirstKillCondition : IItemDropRuleCondition
    {
        public bool CanDrop(DropAttemptInfo info)
        {
            bool sotsCanDrop = false;
            bool consolariaCanDrop = false;

            if (SecretsOfTheSoulsCrossmod.SOTS.Loaded)
                sotsCanDrop = SOTSDropHelper.SOTSCanDrop(info.npc.type);

            if (SecretsOfTheSoulsCrossmod.Consolaria.Loaded)
                consolariaCanDrop = ConsolariaDropHelper.ConsolariaCanDrop(info.npc.type);

            return !info.IsInSimulation && WorldSavingSystem.EternityMode && (sotsCanDrop || consolariaCanDrop);
        }
        public bool CanShowItemDropInUI() => true;

        public string GetConditionDescription() => Language.GetTextValue("Mods.FargowiltasSouls.Conditions.FirstKill");
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public static class SOTSDropHelper
    {
        public static bool SOTSCanDrop(int type)
        {
            if (type == ModContent.NPCType<Glowmoth>())
                return !SOTSWorld.downedGlowmoth;   
            if (type == ModContent.NPCType<PutridPinkyPhase2>())
                return !SOTSWorld.downedPinky;
            if (type == ModContent.NPCType<PharaohsCurse>())
                return !SOTSWorld.downedCurse;
            if (type == ModContent.NPCType<Excavator>())
                return !SOTSWorld.downedExcavator;
            if (type == ModContent.NPCType<TheAdvisorHead>())
                return !SOTSWorld.downedAdvisor;
            if (type == ModContent.NPCType<Polaris>() || type == ModContent.NPCType<NewPolaris>())
                return !SOTSWorld.downedAmalgamation;
            if (type == ModContent.NPCType<Lux>())
                return !SOTSWorld.downedLux;
            if (type == ModContent.NPCType<SubspaceSerpentHead>())
                return !SOTSWorld.downedSubspace;
            return false;
        }
    }

    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.Consolaria.Name)]
    public static class ConsolariaDropHelper
    {
        private static Mod Consolaria => ModLoader.GetMod(SecretsOfTheSoulsCrossmod.Consolaria.Name);
        public static bool ConsolariaCanDrop(int type)
        {
            if (type == Consolaria.Find<ModNPC>("Lepus").Type)
                return !DownedBossSystem.downedLepus;
            if (type == ModContent.NPCType<TurkortheUngrateful>())
                return !DownedBossSystem.downedTurkor;
            if (type == ModContent.NPCType<Ocram>())
                return !DownedBossSystem.downedOcram;
            return false;
        }
    }
}
