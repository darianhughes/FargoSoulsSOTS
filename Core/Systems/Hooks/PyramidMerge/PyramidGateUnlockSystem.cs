using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems.Hooks.PyramidMerge
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public sealed class PyramidGateUnlockSystem : ModSystem
    {
        private static Mod sotsMod;
        private Hook pyramidGateRightClickHook;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("SOTS", out sotsMod))
                return;

            // Hook PyramidGate RightClick to require both bosses defeated
            Type pyramidGateTileType = sotsMod.Code.GetType("SOTS.Items.Pyramid.PyramidGateTile");
            if (pyramidGateTileType != null)
            {
                MethodInfo rightClickMethod = pyramidGateTileType.GetMethod("RightClick", BindingFlags.Public | BindingFlags.Instance);
                if (rightClickMethod != null)
                {
                    pyramidGateRightClickHook = new Hook(rightClickMethod, HookPyramidGateRightClick);
                    Mod.Logger.Info("PyramidGate RightClick hook installed for boss requirement check.");
                }
            }
        }

        public override void Unload()
        {
            pyramidGateRightClickHook?.Dispose();
        }

        private delegate bool orig_PyramidGateRightClick(ModTile instance, int i, int j);

        private bool HookPyramidGateRightClick(orig_PyramidGateRightClick orig, ModTile instance, int i, int j)
        {
            Player player = Main.LocalPlayer;

            bool defeatedEvilBoss = NPC.downedBoss2;
            bool defeatedCursedCoffin = WorldSavingSystem.DownedBoss[(int)WorldSavingSystem.Downed.CursedCoffin];

            // Check if neither boss has been defeated
            if (!defeatedEvilBoss && !defeatedCursedCoffin)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText("The gate is locked and the keyhole is obscured by a dark curse...",
                        new Color(175, 100, 175)); // Purple-red mix
                }
                return false;
            }

            // Check if Eater of Worlds or Brain of Cthulhu has been defeated
            if (!defeatedEvilBoss)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText("The gate is locked... perhaps you could find the key in the depths of the evil biome?",
                        new Color(200, 100, 100)); // Red-ish color
                }
                return false;
            }

            // Check if Cursed Coffin has been defeated
            if (!defeatedCursedCoffin)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText("The gate's keyhole is obscured by a dark curse...",
                        new Color(150, 75, 200)); // Purple-ish color
                }
                return false;
            }

            // Both conditions met, call original method
            return orig(instance, i, j);
        }
    }
}
