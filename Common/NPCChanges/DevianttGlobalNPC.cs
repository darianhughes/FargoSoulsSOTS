using System.Collections.Generic;
using System.Reflection;
using FargoSoulsSOTS.Core.Systems;
using Fargowiltas.Items.Summons.Deviantt;
using Fargowiltas.Items.Tiles;
using Fargowiltas;
using Fargowiltas.NPCs;
using MonoMod.RuntimeDetour;
using SOTS.Items;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Common.NPCChanges
{
    public class DevianttGlobalNPC : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<Deviantt>();
        }
        public override bool InstancePerEntity => true;

        internal static int currentShop;

        internal static readonly List<NPCShop> ModShops = [];

        public static void CycleShop()
        {
            currentShop++;
            currentShop %= ModShops.Count + 1;
        }

        public static void AddSOTSShop()
        {
            NPCShop shop = new(ModContent.NPCType<Deviantt>(), Language.GetTextValue("Mods.FargoSoulsSOTS.NPCs.ShopModSwapper.SOTS"));

            Condition killedConstruct = new Condition("Mods.FargoSoulsSOTS.Conditions.ConstructDowned", () => FargoSoulsSOTSWorldSavingSystem.downedConstruct);

            shop.Add(new Item(ModContent.ItemType<ElectromagneticLure>()) { shopCustomPrice = Item.buyPrice(gold: 5) }, killedConstruct);

            shop.Register();
            ModShops.Add(shop);
        }

        public override void Load()
        {
            DeviChatButtonHook = new(DevianttOnChatButtonClickedMethod, DevianttOnChatButtonClickedDetour);
            DeviChatButtonHook.Apply();

            DeviShopHook = new(DevianttAddShopsMethod, DevianttAddShopsDetour);
            DeviShopHook.Apply();
        }
        public override void Unload()
        {
            DeviChatButtonHook.Undo();
            DeviShopHook.Undo();
        }
        public delegate void Orig_DevianttOnChatButtonClicked(Deviantt self, bool firstButton, ref string shopName);
        public delegate void Orig_DevianttAddShops(Deviantt self);

        private static readonly MethodInfo DevianttOnChatButtonClickedMethod = typeof(Deviantt).GetMethod("OnChatButtonClicked", LumUtils.UniversalBindingFlags);
        private static readonly MethodInfo DevianttAddShopsMethod = typeof(Deviantt).GetMethod("AddShops", LumUtils.UniversalBindingFlags);

        Hook DeviChatButtonHook;
        Hook DeviShopHook;

        internal static void DevianttOnChatButtonClickedDetour(Orig_DevianttOnChatButtonClicked orig, Deviantt self, bool firstButton, ref string shopName)
        {
            orig(self, firstButton, ref shopName);

            if (firstButton)
            {
                if (currentShop == 0)
                {
                    shopName = Deviantt.ShopName;
                }
                else
                {
                    shopName = ModShops[currentShop - 1].Name;
                }
            }
        }

        internal static void DevianttAddShopsDetour(Orig_DevianttAddShops orig, Deviantt self)
        {
            AddVanillaShop();
            if (FargoSOTSCrossmod.FargowiltasCrossmod.Loaded && FargoSOTSCrossmod.CalamityMod.Loaded) //i don't wanna recode the ui but we will keep this stuff because yeah
            {
                DeviantCrossmodHelper.GetCalamityShop();
                AddSOTSShop();
            }
        }

        public static Dictionary<string, bool> FargoWorldDownedBools => typeof(FargoWorld).GetField("DownedBools", LumUtils.UniversalBindingFlags).GetValue(null) as Dictionary<string, bool>;

        public static void AddVanillaShop()
        {
            var npcShop = new NPCShop(ModContent.NPCType<Deviantt>(), Deviantt.ShopName);

            if (ModLoader.TryGetMod("FargowiltasSoulsDLC", out Mod soulDLC) && soulDLC.TryFind("PandorasBox", out ModItem pandorasBox))
            {
                npcShop.Add(new Item(pandorasBox.Type));
            }

            Condition killedConstruct = new Condition("Mods.FargoSoulsSOTS.Conditions.ConstructDowned", () => FargoSoulsSOTSWorldSavingSystem.downedConstruct);

            npcShop
                .Add(new Item(ModContent.ItemType<WormSnack>()) { shopCustomPrice = Item.buyPrice(copper: 20000) }, new Condition("Mods.Fargowiltas.Conditions.WormDown", () => FargoWorldDownedBools["worm"]))
                .Add(new Item(ModContent.ItemType<PinkSlimeCrown>()) { shopCustomPrice = Item.buyPrice(copper: 50000) }, new Condition("Mods.Fargowiltas.Conditions.PinkyDown", () => FargoWorldDownedBools["pinky"]))
                .Add(new Item(ModContent.ItemType<GoblinScrap>()) { shopCustomPrice = Item.buyPrice(copper: 10000) }, new Condition("Mods.Fargowiltas.Conditions.ScoutDown", () => FargoWorldDownedBools["goblinScout"]))
                .Add(new Item(ModContent.ItemType<Eggplant>()) { shopCustomPrice = Item.buyPrice(copper: 20000) }, new Condition("Mods.Fargowiltas.Conditions.DoctorDown", () => FargoWorldDownedBools["doctorBones"]))
                .Add(new Item(ModContent.ItemType<AttractiveOre>()) { shopCustomPrice = Item.buyPrice(copper: 30000) }, new Condition("Mods.Fargowiltas.Conditions.MinerDown", () => FargoWorldDownedBools["undeadMiner"]))
                .Add(new Item(ModContent.ItemType<HolyGrail>()) { shopCustomPrice = Item.buyPrice(copper: 50000) }, new Condition("Mods.Fargowiltas.Conditions.TimDown", () => FargoWorldDownedBools["tim"]))
                .Add(new Item(ModContent.ItemType<GnomeHat>()) { shopCustomPrice = Item.buyPrice(copper: 50000) }, new Condition("Mods.Fargowiltas.Conditions.GnomeDown", () => FargoWorldDownedBools["gnome"]))
                .Add(new Item(ModContent.ItemType<GoldenSlimeCrown>()) { shopCustomPrice = Item.buyPrice(copper: 600000) }, new Condition("Mods.Fargowiltas.Conditions.GoldSlimeDown", () => FargoWorldDownedBools["goldenSlime"]))
                .Add(new Item(ModContent.ItemType<SlimyLockBox>()) { shopCustomPrice = Item.buyPrice(copper: 100000) }, new Condition("Mods.Fargowiltas.Conditions.DungeonSlimeDown", () => NPC.downedBoss3 && FargoWorldDownedBools["dungeonSlime"]))
                .Add(new Item(ModContent.ItemType<AthenianIdol>()) { shopCustomPrice = Item.buyPrice(copper: 50000) }, new Condition("Mods.Fargowiltas.Conditions.MedusaDown", () => Main.hardMode && FargoWorldDownedBools["medusa"]))
                .Add(new Item(ModContent.ItemType<ClownLicense>()) { shopCustomPrice = Item.buyPrice(copper: 50000) }, new Condition("Mods.Fargowiltas.Conditions.ClownDown", () => Main.hardMode && FargoWorldDownedBools["clown"]))
                .Add(new Item(ModContent.ItemType<HeartChocolate>()) { shopCustomPrice = Item.buyPrice(copper: 100000) }, new Condition("Mods.Fargowiltas.Conditions.NymphDown", () => FargoWorldDownedBools["nymph"]))
                .Add(new Item(ModContent.ItemType<MothLamp>()) { shopCustomPrice = Item.buyPrice(copper: 100000) }, new Condition("Mods.Fargowiltas.Conditions.MothDown", () => Main.hardMode && FargoWorldDownedBools["moth"]))
                .Add(new Item(ModContent.ItemType<DilutedRainbowMatter>()) { shopCustomPrice = Item.buyPrice(copper: 100000) }, new Condition("Mods.Fargowiltas.Conditions.RainbowSlimeDown", () => Main.hardMode && FargoWorldDownedBools["rainbowSlime"]))
                .Add(new Item(ModContent.ItemType<CloudSnack>()) { shopCustomPrice = Item.buyPrice(copper: 100000) }, new Condition("Mods.Fargowiltas.Conditions.WyvernDown", () => Main.hardMode && FargoWorldDownedBools["wyvern"]))
                .Add(new Item(ModContent.ItemType<RuneOrb>()) { shopCustomPrice = Item.buyPrice(copper: 150000) }, new Condition("Mods.Fargowiltas.Conditions.RuneDown", () => Main.hardMode && FargoWorldDownedBools["runeWizard"]))
                .Add(new Item(ModContent.ItemType<SuspiciousLookingChest>()) { shopCustomPrice = Item.buyPrice(copper: 300000) }, new Condition("Mods.Fargowiltas.Conditions.MimicDown", () => Main.hardMode && FargoWorldDownedBools["mimic"]))
                .Add(new Item(ModContent.ItemType<HallowChest>()) { shopCustomPrice = Item.buyPrice(copper: 300000) }, new Condition("Mods.Fargowiltas.Conditions.MimicHallowDown", () => Main.hardMode && FargoWorldDownedBools["mimicHallow"]))
                .Add(new Item(ModContent.ItemType<CorruptChest>()) { shopCustomPrice = Item.buyPrice(copper: 300000) }, new Condition("Mods.Fargowiltas.Conditions.MimicCorruptDown", () => Main.hardMode && (FargoWorldDownedBools["mimicCorrupt"] || FargoWorldDownedBools["mimicCrimson"])))
                .Add(new Item(ModContent.ItemType<CrimsonChest>()) { shopCustomPrice = Item.buyPrice(copper: 300000) }, new Condition("Mods.Fargowiltas.Conditions.MimicCrimsonDown", () => Main.hardMode && (FargoWorldDownedBools["mimicCorrupt"] || FargoWorldDownedBools["mimicCrimson"])))
                .Add(new Item(ModContent.ItemType<JungleChest>()) { shopCustomPrice = Item.buyPrice(copper: 300000) }, new Condition("Mods.Fargowiltas.Conditions.MimicJungleDown", () => Main.hardMode && FargoWorldDownedBools["mimicJungle"]))
                .Add(new Item(ModContent.ItemType<CoreoftheFrostCore>()) { shopCustomPrice = Item.buyPrice(copper: 100000) }, new Condition("Mods.Fargowiltas.Conditions.IceGolemDown", () => Main.hardMode && FargoWorldDownedBools["iceGolem"]))
                .Add(new Item(ModContent.ItemType<ForbiddenForbiddenFragment>()) { shopCustomPrice = Item.buyPrice(copper: 100000) }, new Condition("Mods.Fargowiltas.Conditions.SandDown", () => Main.hardMode && FargoWorldDownedBools["sandElemental"]))
                .Add(new Item(ModContent.ItemType<DemonicPlushie>()) { shopCustomPrice = Item.buyPrice(copper: 100000) }, new Condition("Mods.Fargowiltas.Conditions.DevilDown", () => NPC.downedMechBossAny && FargoWorldDownedBools["redDevil"]))
                .Add(new Item(ModContent.ItemType<SuspiciousLookingLure>()) { shopCustomPrice = Item.buyPrice(copper: 100000) }, new Condition("Mods.Fargowiltas.Conditions.BloodFishDown", () => FargoWorldDownedBools["eyeFish"] || FargoWorldDownedBools["zombieMerman"]))
                .Add(new Item(ModContent.ItemType<BloodUrchin>()) { shopCustomPrice = Item.buyPrice(copper: 100000) }, new Condition("Mods.Fargowiltas.Conditions.BloodEelDown", () => Main.hardMode && FargoWorldDownedBools["bloodEel"]))
                .Add(new Item(ModContent.ItemType<HemoclawCrab>()) { shopCustomPrice = Item.buyPrice(copper: 100000) }, new Condition("Mods.Fargowiltas.Conditions.BloodGoblinDown", () => Main.hardMode && FargoWorldDownedBools["goblinShark"]))
                .Add(new Item(ModContent.ItemType<BloodSushiPlatter>()) { shopCustomPrice = Item.buyPrice(copper: 200000) }, new Condition("Mods.Fargowiltas.Conditions.BloodNautDown", () => Main.hardMode && FargoWorldDownedBools["dreadnautilus"]))
                .Add(new Item(ModContent.ItemType<ShadowflameIcon>()) { shopCustomPrice = Item.buyPrice(copper: 100000) }, new Condition("Mods.Fargowiltas.Conditions.SummonerDown", () => Main.hardMode && NPC.downedGoblins && FargoWorldDownedBools["goblinSummoner"]))
                .Add(new Item(ModContent.ItemType<PirateFlag>()) { shopCustomPrice = Item.buyPrice(copper: 150000) }, new Condition("Mods.Fargowiltas.Conditions.PirateDown", () => Main.hardMode && NPC.downedPirates && FargoWorldDownedBools["pirateCaptain"]))
                .Add(new Item(ModContent.ItemType<Pincushion>()) { shopCustomPrice = Item.buyPrice(copper: 150000) }, new Condition("Mods.Fargowiltas.Conditions.NailheadDown", () => NPC.downedPlantBoss && FargoWorldDownedBools["nailhead"]))
                .Add(new Item(ModContent.ItemType<MothronEgg>()) { shopCustomPrice = Item.buyPrice(copper: 150000) }, new Condition("Mods.Fargowiltas.Conditions.MothronDown", () => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && FargoWorldDownedBools["mothron"]))
                .Add(new Item(ModContent.ItemType<LeesHeadband>()) { shopCustomPrice = Item.buyPrice(copper: 150000) }, new Condition("Mods.Fargowiltas.Conditions.LeeDown", () => NPC.downedPlantBoss && FargoWorldDownedBools["boneLee"]))
                .Add(new Item(ModContent.ItemType<GrandCross>()) { shopCustomPrice = Item.buyPrice(copper: 150000) }, new Condition("Mods.Fargowiltas.Conditions.PaladinDown", () => NPC.downedPlantBoss && FargoWorldDownedBools["paladin"]))
                .Add(new Item(ModContent.ItemType<AmalgamatedSkull>()) { shopCustomPrice = Item.buyPrice(copper: 300000) }, new Condition("Mods.Fargowiltas.Conditions.SkeleGunDown", () => NPC.downedPlantBoss && FargoWorldDownedBools["skeletonGun"]))
                .Add(new Item(ModContent.ItemType<AmalgamatedSpirit>()) { shopCustomPrice = Item.buyPrice(copper: 300000) }, new Condition("Mods.Fargowiltas.Conditions.SkeleGunDown", () => NPC.downedPlantBoss && FargoWorldDownedBools["skeletonMage"]))
                .Add(new Item(ModContent.ItemType<SiblingPylon>()), Condition.HappyEnoughToSellPylons, Condition.NpcIsPresent(ModContent.NPCType<Mutant>()), Condition.NpcIsPresent(ModContent.NPCType<Abominationn>()))
            ;

            if (!FargoSOTSCrossmod.FargowiltasCrossmod.Loaded)
            {
                npcShop.Add(new Item(ModContent.ItemType<ElectromagneticLure>()) { shopCustomPrice = Item.buyPrice(gold: 5) }, killedConstruct);
            }

            npcShop.Register();
        }
    }

    [JITWhenModsEnabled(FargoSOTSCrossmod.FargowiltasCrossmod.Name, FargoSOTSCrossmod.CalamityMod.Name)]
    [ExtendsFromMod(FargoSOTSCrossmod.FargowiltasCrossmod.Name)]
    public class DeviantCrossmodHelper
    {
        public static void GetCalamityShop()
        {
            FargowiltasCrossmod.Core.Common.Globals.DevianttGlobalNPC.AddCalamityShop();
        }
    }
}
