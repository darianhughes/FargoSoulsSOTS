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
using Fargowiltas.NPCs;
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
using FargoSoulsSOTS.Content.Buffs.Emode;
using FargoSoulsSOTS.Content.Items.Accessories.Masomode;

namespace FargoSoulsSOTS.Common.NPCChanges
{
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
                // [ModContent.NPCType<TheAdvisorHead>()] = (1.12f, 1.12f, 1.30f, 1.22f),
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
            if (npc.type == ModContent.NPCType<Glowmoth>())
                npc.buffImmune[ModContent.BuffType<Lepidopterism>()] = true;

            if (!npc.boss || !IsSotsBoss(npc.type))
                return;

            bool eternity = WorldSavingSystem.EternityMode;
            if (!eternity) return;

            bool maso = WorldSavingSystem.MasochistModeReal;

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
                emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.IronCrate, 3));
            }
            if (npc.type == ModContent.NPCType<PutridPinkyPhase2>())
            {
                emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.IronCrate, 5));
            }
            if (npc.type == ModContent.NPCType<PharaohsCurse>())
            {
                emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<PyramidCrate>(), 5));
            }
            if (npc.type == ModContent.NPCType<Excavator>())
            {
                emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(WorldGen.crimson ? ItemID.CrimsonFishingCrate : ItemID.CorruptFishingCrate, 5));
            }
            if (npc.type == ModContent.NPCType<TheAdvisorHead>())
            {
                emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ModContent.ItemType<PlanetariumCrate>(),5));
            }
            if (npc.type == ModContent.NPCType<Polaris>() || npc.type == ModContent.NPCType<NewPolaris>())
            {
                emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.FrozenCrateHard, 5));
            }
            if (npc.type == ModContent.NPCType<Lux>())
            {
                emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.HallowedFishingCrateHard, 5));
            }
            if (npc.type == ModContent.NPCType<SubspaceSerpentHead>())
            {
                emodeRule.OnSuccess(FargoSoulsUtil.BossBagDropCustom(ItemID.LavaCrateHard, 5));
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
}
