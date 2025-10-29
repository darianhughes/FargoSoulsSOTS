using Microsoft.Xna.Framework;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems.Hooks.PyramidMerge
{
    [ExtendsFromMod(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    [JITWhenModsEnabled(SecretsOfTheSoulsCrossmod.SOTS.Name)]
    public sealed class PyramidArenaIntegrationSystem : ModSystem
    {
        private static Mod sotsMod;
        private static Mod fargoMod;
        private static Type pyramidHelperType;
        private static FieldInfo placementLocationField;

        private Hook generateSOTSPyramidHook;
        private ILHook generateSOTSPyramidILHook;
        private Hook generatePyramidPathHook;
        private Hook generatePyramidRoomHook;
        private Hook generatePyramidCrystalRoomHook;
        private Hook generateShrineRoomHook;
        private Hook generatePyramidOvalHook;
        private Hook generateInfectionHook;

        // Context storage for second corridor mirroring
        private static int pyramidPathCallCount = 0;
        private static int storedEndingTileX = 0;

        // Gate relocation tile types
        private static int pyramidGateTileType = -1;
        private static int royalGoldBrickTileType = -1;
        private static int pyramidChestTileType = -1;

        // Threshold for preventing generation above gate level
        private static int gateThresholdY = -1;

        // Arena placement context for gate relocation
        private static int storedDirection = 0;
        private static int storedArenaLeft = 0;
        private static int storedArenaRight = 0;
        private static int storedArenaBottom = 0;
        private static int storedMarkerX = 0;
        private static int storedMarkerY = 0;

        // Gate position for post-generation unsafe wall replacement
        private static int storedNewGateX = -1;
        private static int storedCorridorFloorY = -1;

        // Track replaced tile range for post-generation restoration
        private static int replacedTilesStartX = -1;
        private static int replacedTilesEndX = -1;
        private static int replacedTilesY = -1;

        public override void Load()
        {
            if (!ModLoader.TryGetMod("SOTS", out sotsMod))
                return;

            if (!ModLoader.TryGetMod("FargowiltasSouls", out fargoMod))
                return;

            pyramidHelperType = sotsMod.Code.GetType("SOTS.WorldgenHelpers.PyramidWorldgenHelper");
            if (pyramidHelperType == null)
                return;

            placementLocationField = pyramidHelperType.GetField("placementLocation", BindingFlags.Public | BindingFlags.Static);
            if (placementLocationField == null)
                return;

            // Initialize gate relocation tile types
            pyramidGateTileType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidGateTile")?.Type ?? -1;
            royalGoldBrickTileType = ModContent.Find<ModTile>(sotsMod.Name, "RoyalGoldBrickTile")?.Type ?? -1;
            pyramidChestTileType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidChestTile")?.Type ?? -1;

            // Disable vanilla pyramid generation and block chests above gate
            On_WorldGen.Pyramid += DisableVanillaPyramids;
            On_WorldGen.PlaceTile += BlockAboveGateChests;

            MethodInfo generateSOTSPyramidMethod = pyramidHelperType.GetMethod("GenerateSOTSPyramid", BindingFlags.Public | BindingFlags.Static);
            if (generateSOTSPyramidMethod != null)
            {
                generateSOTSPyramidHook = new Hook(generateSOTSPyramidMethod, HookGenerateSOTSPyramid);
                generateSOTSPyramidILHook = new ILHook(generateSOTSPyramidMethod, PatchGenerateSOTSPyramid_IL);
                Mod.Logger.Info("Pyramid IL patch installed for deterministic zigzag depths.");
            }

            MethodInfo generatePyramidPathMethod = pyramidHelperType.GetMethod("GeneratePyramidPath", BindingFlags.Public | BindingFlags.Static);
            if (generatePyramidPathMethod != null)
            {
                generatePyramidPathHook = new Hook(generatePyramidPathMethod, HookGeneratePyramidPath);
                Mod.Logger.Info("GeneratePyramidPath hook installed for corridor control.");
            }

            MethodInfo generatePyramidRoomMethod = pyramidHelperType.GetMethod("GeneratePyramidRoom", BindingFlags.Public | BindingFlags.Static);
            if (generatePyramidRoomMethod != null)
            {
                generatePyramidRoomHook = new Hook(generatePyramidRoomMethod, HookGeneratePyramidRoom);
            }

            MethodInfo generateCrystalRoomMethod = pyramidHelperType.GetMethod("GeneratePyramidCrystalRoom", BindingFlags.Public | BindingFlags.Static);
            if (generateCrystalRoomMethod != null)
            {
                generatePyramidCrystalRoomHook = new Hook(generateCrystalRoomMethod, HookGeneratePyramidCrystalRoom);
            }

            MethodInfo generateShrineRoomMethod = pyramidHelperType.GetMethod("GenerateShrineRoom", BindingFlags.Public | BindingFlags.Static);
            if (generateShrineRoomMethod != null)
            {
                generateShrineRoomHook = new Hook(generateShrineRoomMethod, HookGenerateShrineRoom);
            }

            MethodInfo generatePyramidOvalMethod = pyramidHelperType.GetMethod("GeneratePyramidOval", BindingFlags.Public | BindingFlags.Static);
            if (generatePyramidOvalMethod != null)
            {
                generatePyramidOvalHook = new Hook(generatePyramidOvalMethod, HookGeneratePyramidOval);
            }

            MethodInfo generateInfectionMethod = pyramidHelperType.GetMethod("GenerateInfection", BindingFlags.Public | BindingFlags.Static);
            if (generateInfectionMethod != null)
            {
                generateInfectionHook = new Hook(generateInfectionMethod, HookGenerateInfection);
            }
        }

        public override void Unload()
        {
            On_WorldGen.Pyramid -= DisableVanillaPyramids;
            On_WorldGen.PlaceTile -= BlockAboveGateChests;

            generateSOTSPyramidHook?.Dispose();
            generateSOTSPyramidHook = null;

            generateSOTSPyramidILHook?.Dispose();
            generateSOTSPyramidILHook = null;

            generatePyramidPathHook?.Dispose();
            generatePyramidPathHook = null;

            generatePyramidRoomHook?.Dispose();
            generatePyramidRoomHook = null;

            generatePyramidCrystalRoomHook?.Dispose();
            generatePyramidCrystalRoomHook = null;

            generateShrineRoomHook?.Dispose();
            generateShrineRoomHook = null;

            generatePyramidOvalHook?.Dispose();
            generatePyramidOvalHook = null;

            generateInfectionHook?.Dispose();
            generateInfectionHook = null;

            sotsMod = null;
            fargoMod = null;
            pyramidHelperType = null;
            placementLocationField = null;
        }

        private static bool DisableVanillaPyramids(On_WorldGen.orig_Pyramid orig, int i, int j)
        {
            // Prevent vanilla pyramid generation - SOTS pyramids will be generated instead
            return false;
        }

        private static bool BlockAboveGateChests(On_WorldGen.orig_PlaceTile orig, int i, int j, int Type, bool mute, bool forced, int plr, int style)
        {
            // Block pyramid chests from spawning above gate threshold during worldgen
            if (WorldGen.generatingWorld && pyramidChestTileType != -1 && Type == pyramidChestTileType && IsAboveGateThreshold(j))
                return false;

            return orig(i, j, Type, mute, forced, plr, style);
        }

        private static bool IsAboveGateThreshold(int y)
        {
            // If threshold not set yet, allow generation (gate hasn't been placed)
            if (gateThresholdY == -1)
                return false;

            return y < gateThresholdY;
        }

        private delegate void orig_GenerateSOTSPyramid(Mod mod, bool generateAllSteps, int generateCertainStep, int manualPlacementX, int manualPlacementY, int manualSeed);

        private void HookGenerateSOTSPyramid(orig_GenerateSOTSPyramid orig, Mod mod, bool generateAllSteps, int generateCertainStep, int manualPlacementX, int manualPlacementY, int manualSeed)
        {
            // Reset counter and stored values at the start of each pyramid generation
            pyramidPathCallCount = 0;
            storedNewGateX = -1;
            storedCorridorFloorY = -1;
            gateThresholdY = -1;
            replacedTilesStartX = -1;
            replacedTilesEndX = -1;
            replacedTilesY = -1;

            try
            {
                // Allow full pyramid generation
                orig(mod, generateAllSteps, generateCertainStep, manualPlacementX, manualPlacementY, manualSeed);

                Vector2 pyramidLocation = (Vector2)placementLocationField.GetValue(null);
                int pyramidX = (int)pyramidLocation.X;
                int pyramidY = (int)pyramidLocation.Y;

                Mod.Logger.Info($"SOTS Pyramid generated with arena integration at location: ({pyramidX}, {pyramidY})");

                // Replace unsafe walls after SOTS generation completes
                // (doing this during generation causes SOTS to hang, likely because it's searching for unsafe walls)
                if (storedNewGateX != -1 && storedCorridorFloorY != -1)
                {
                    int pyramidWallType = ModContent.Find<ModWall>(sotsMod.Name, "PyramidWallWall")?.Type ?? -1;
                    if (pyramidWallType != -1)
                    {
                        ReplaceUnsafeWallsAboveGate(pyramidX, pyramidY, storedNewGateX, storedCorridorFloorY, pyramidWallType);
                        Mod.Logger.Info("Replaced unsafe walls above gate after SOTS generation completed");
                    }
                }

                // Restore replaced tiles back to PyramidSlabTile after SOTS generation completes
                // (only if they're still PyramidBrickTile - some may have been changed by other worldgen)
                if (replacedTilesStartX != -1 && replacedTilesEndX != -1 && replacedTilesY != -1)
                {
                    int pyramidSlabType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidSlabTile")?.Type ?? -1;
                    int pyramidBrickType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidBrickTile")?.Type ?? -1;

                    if (pyramidSlabType != -1 && pyramidBrickType != -1)
                    {
                        int restoredCount = 0;
                        for (int scanX = replacedTilesStartX; scanX <= replacedTilesEndX; scanX++)
                        {
                            if (!WorldGen.InWorld(scanX, replacedTilesY, 30))
                                continue;

                            Tile tile = Main.tile[scanX, replacedTilesY];

                            // Only restore if still PyramidBrickTile (unchanged by other worldgen)
                            if (tile.HasTile && tile.TileType == pyramidBrickType)
                            {
                                tile.TileType = (ushort)pyramidSlabType;
                                restoredCount++;
                            }
                        }

                        Mod.Logger.Info($"Restored {restoredCount} PyramidBrickTile → PyramidSlabTile at gate level (Y={replacedTilesY}) after SOTS generation");
                    }
                }

                // Remove all crates from arena area
                // This prevents crates from spawning inside the Cursed Coffin boss arena
                if (storedArenaLeft != 0 && storedArenaRight != 0 && storedArenaBottom != 0)
                {
                    int pyramidCrateType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidCrateTile")?.Type ?? -1;
                    int vanillaCrateType = 376; // TileID.FishingCrate (vanilla oasis crates)

                    if (pyramidCrateType != -1)
                    {
                        int arenaTop = storedMarkerY; // Arena starts at marker position
                        int removedCount = 0;

                        // Scan entire arena bounds for any crates
                        for (int scanX = storedArenaLeft; scanX <= storedArenaRight; scanX++)
                        {
                            for (int scanY = arenaTop; scanY <= storedArenaBottom; scanY++)
                            {
                                if (!WorldGen.InWorld(scanX, scanY, 30))
                                    continue;

                                Tile tile = Main.tile[scanX, scanY];

                                // Check for both SOTS pyramid crates and vanilla oasis crates
                                if (tile.HasTile && (tile.TileType == pyramidCrateType || tile.TileType == vanillaCrateType))
                                {
                                    // Kill the crate (works on any tile of the 2x2, removes entire crate)
                                    WorldGen.KillTile(scanX, scanY, noItem: true);
                                    removedCount++;
                                }
                            }
                        }

                        if (removedCount > 0)
                        {
                            Mod.Logger.Info($"Removed {removedCount} crates from arena area (bounds: X={storedArenaLeft}-{storedArenaRight}, Y={arenaTop}-{storedArenaBottom})");
                        }
                    }
                }

                // Convert SOTS pyramid pots to vanilla pyramid pots above gate level
                // Two-pass approach: first clear all pots, then place vanilla pots
                if (storedCorridorFloorY != -1)
                {
                    int sotsPyramidPotsType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidPots")?.Type ?? -1;
                    if (sotsPyramidPotsType != -1)
                    {
                        int gateY = storedCorridorFloorY;
                        const int scanWidth = 300;

                        // Pass 1: Find all pot positions and clear them
                        System.Collections.Generic.List<Point> potFloorPositions = new System.Collections.Generic.List<Point>();
                        for (int scanX = pyramidX - scanWidth; scanX <= pyramidX + scanWidth; scanX++)
                        {
                            for (int scanY = pyramidY; scanY < gateY; scanY++)
                            {
                                if (!WorldGen.InWorld(scanX, scanY, 30))
                                    continue;

                                Tile tile = Main.tile[scanX, scanY];

                                // Check if this is a SOTS pyramid pot (origin is at bottom-left, so we're looking for the bottom tiles)
                                if (tile.HasTile && tile.TileType == sotsPyramidPotsType)
                                {
                                    // Get the pot's bottom-left tile position
                                    // Pots are 2x2 with origin at (0, 1) meaning bottom-left
                                    // PyramidPots has 9 styles with StyleHorizontal=true and StyleWrapLimit=3
                                    // This means styles wrap every 3, creating rows of styles
                                    // TileFrameX includes style offset: each style is 2 tiles wide (36 pixels)
                                    // TileFrameY includes style row offset: each row is 2 tiles tall (36 pixels)

                                    // Get column within the pot (0 = left, 1 = right)
                                    int colOffset = tile.TileFrameX / 18 % 2;

                                    // Get row within the pot (0 = top, 1 = bottom)
                                    int rowOffset = tile.TileFrameY / 18 % 2;

                                    // Calculate bottom-left tile position
                                    int potOriginX = scanX - colOffset;
                                    int potOriginY = scanY + (1 - rowOffset); // scanY + 1 if top, scanY if bottom

                                    // Floor is one tile below the bottom row
                                    int floorY = potOriginY + 1;
                                    Point floorPos = new Point(potOriginX, floorY);

                                    // Only add if we haven't already found this pot
                                    if (!potFloorPositions.Contains(floorPos))
                                    {
                                        potFloorPositions.Add(floorPos);

                                        // Kill the entire pot (KillTile on any tile of a multi-tile kills the whole thing)
                                        WorldGen.KillTile(scanX, scanY, noItem: true);
                                    }
                                }
                            }
                        }

                        // Pass 2: Place vanilla pyramid pots at all collected floor positions
                        int placedCount = 0;
                        int pyramidBrickType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidBrickTile")?.Type ?? -1;
                        int pyramidSlabType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidSlabTile")?.Type ?? -1;

                        foreach (Point floorPos in potFloorPositions)
                        {
                            int floorX = floorPos.X;
                            int floorY = floorPos.Y;

                            // Verify floor tile is solid
                            Tile floorTile = Main.tile[floorX, floorY];
                            if (!floorTile.HasTile)
                            {
                                Mod.Logger.Debug($"Pot at ({floorX}, {floorY}): floor has no tile, skipping");
                                continue;
                            }

                            // Clear the 2x2 area above the floor to ensure clean placement
                            for (int dx = 0; dx < 2; dx++)
                            {
                                for (int dy = 1; dy <= 2; dy++)
                                {
                                    int clearX = floorX + dx;
                                    int clearY = floorY - dy;
                                    if (WorldGen.InWorld(clearX, clearY, 30))
                                    {
                                        Main.tile[clearX, clearY].ClearTile();
                                    }
                                }
                            }

                            // Ensure floor tiles are valid pyramid tiles (not air)
                            for (int dx = 0; dx < 2; dx++)
                            {
                                Tile floor = Main.tile[floorX + dx, floorY];
                                if (!floor.HasTile)
                                {
                                    // Restore floor tile if it got cleared
                                    floor.HasTile = true;
                                    floor.TileType = (ushort)(pyramidBrickType != -1 ? pyramidBrickType : pyramidSlabType);
                                }
                            }

                            // Place vanilla pyramid pot using WorldGen.PlacePot
                            // Place at floorY - 1 (one tile above floor, which is the pot's origin position)
                            // Signature: PlacePot(x, y, type, style)
                            int potStyle = WorldGen.genRand.Next(25, 28);
                            bool placed = WorldGen.PlacePot(floorX, floorY - 1, 28, potStyle);

                            if (placed)
                            {
                                placedCount++;
                            }
                            else
                            {
                                Mod.Logger.Debug($"Failed to place pot at ({floorX}, {floorY - 1}), floor type: {floorTile.TileType}");
                            }
                        }

                        Mod.Logger.Info($"Converted {potFloorPositions.Count} SOTS PyramidPots → {placedCount} Vanilla Pyramid Pots above gate (Y < {gateY})");
                    }
                }

                // Convert SOTS pyramid crates to vanilla oasis crates above gate level
                // Two-pass approach: first clear all SOTS crates, then place vanilla oasis crates
                if (storedCorridorFloorY != -1)
                {
                    int pyramidCrateType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidCrateTile")?.Type ?? -1;
                    int vanillaCrateType = 376; // TileID.FishingCrate (vanilla oasis crates)

                    if (pyramidCrateType != -1)
                    {
                        int gateY = storedCorridorFloorY;
                        const int scanWidth = 300;

                        // Pass 1: Find all SOTS pyramid crate positions and clear them
                        System.Collections.Generic.List<Point> crateFloorPositions = new System.Collections.Generic.List<Point>();
                        for (int scanX = pyramidX - scanWidth; scanX <= pyramidX + scanWidth; scanX++)
                        {
                            for (int scanY = pyramidY; scanY < gateY; scanY++)
                            {
                                if (!WorldGen.InWorld(scanX, scanY, 30))
                                    continue;

                                Tile tile = Main.tile[scanX, scanY];

                                // Check if this is a SOTS pyramid crate (2x2 multi-tile)
                                // Only process vanilla oasis crates are left alone
                                if (tile.HasTile && tile.TileType == pyramidCrateType)
                                {
                                    // Get frame coordinates to identify origin tile (bottom-left)
                                    int frameX = tile.TileFrameX;
                                    int frameY = tile.TileFrameY;

                                    // Crates are 2x2 with frame dimensions 18x18 per tile
                                    // Origin (bottom-left) has frameX=0, frameY=18
                                    if (frameX == 0 && frameY == 18)
                                    {
                                        // This is the bottom-left origin tile
                                        // Record the floor position (one tile below the crate)
                                        int floorX = scanX;
                                        int floorY = scanY + 1;

                                        // Verify floor tile exists
                                        if (WorldGen.InWorld(floorX, floorY, 30))
                                        {
                                            Tile floorTile = Main.tile[floorX, floorY];
                                            if (floorTile.HasTile)
                                            {
                                                crateFloorPositions.Add(new Point(floorX, floorY));

                                                // Kill the crate (removing from origin tile kills entire 2x2)
                                                WorldGen.KillTile(scanX, scanY, noItem: true);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Pass 2: Place vanilla oasis crates at all collected floor positions
                        int placedCount = 0;
                        int pyramidBrickType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidBrickTile")?.Type ?? -1;
                        int pyramidSlabType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidSlabTile")?.Type ?? -1;

                        foreach (Point floorPos in crateFloorPositions)
                        {
                            int floorX = floorPos.X;
                            int floorY = floorPos.Y;

                            if (!WorldGen.InWorld(floorX, floorY, 30))
                                continue;

                            Tile floorTile = Main.tile[floorX, floorY];
                            if (!floorTile.HasTile)
                                continue;

                            // Clear the 2x2 area above the floor to ensure clean placement
                            for (int dx = 0; dx < 2; dx++)
                            {
                                for (int dy = 1; dy <= 2; dy++)
                                {
                                    int clearX = floorX + dx;
                                    int clearY = floorY - dy;
                                    if (WorldGen.InWorld(clearX, clearY, 30))
                                    {
                                        Main.tile[clearX, clearY].ClearTile();
                                    }
                                }
                            }

                            // Ensure floor tiles are valid pyramid tiles (not air)
                            for (int dx = 0; dx < 2; dx++)
                            {
                                Tile floor = Main.tile[floorX + dx, floorY];
                                if (!floor.HasTile)
                                {
                                    // Restore floor tile if it got cleared
                                    floor.HasTile = true;
                                    floor.TileType = (ushort)(pyramidBrickType != -1 ? pyramidBrickType : pyramidSlabType);
                                }
                            }

                            // Place vanilla oasis crate
                            // Use random style 0-2 to match SOTS's natural generation
                            // Crate placement position is one tile above floor (floorY - 1)
                            // Use forced: true to match SOTS's GenerateCrate behavior
                            int crateStyle = WorldGen.genRand.Next(3); // Styles 0, 1, or 2
                            bool placed = WorldGen.PlaceTile(floorX, floorY - 1, vanillaCrateType, mute: true, forced: true, -1, crateStyle);

                            if (placed)
                            {
                                placedCount++;
                            }
                            else
                            {
                                Mod.Logger.Debug($"Failed to place oasis crate at ({floorX}, {floorY - 1}), floor type: {floorTile.TileType}");
                            }
                        }

                        if (crateFloorPositions.Count > 0)
                        {
                            Mod.Logger.Info($"Converted {crateFloorPositions.Count} SOTS Pyramid Crates → {placedCount} Vanilla Oasis Crates above gate (Y < {gateY})");
                        }
                    }
                }

                // Drain all liquids in pyramid area above gate level
                // Pyramids expand by 1 block on each side per layer down from the apex
                if (storedCorridorFloorY != -1)
                {
                    int gateY = storedCorridorFloorY;

                    int drainedCount = 0;
                    for (int scanY = pyramidY; scanY < gateY; scanY++)
                    {
                        // Calculate pyramid width at this Y level
                        // Pyramid grows by 1 block on each side per layer down
                        int depth = scanY - pyramidY;
                        int pyramidHalfWidth = depth + 1;

                        for (int scanX = pyramidX - pyramidHalfWidth; scanX <= pyramidX + pyramidHalfWidth; scanX++)
                        {
                            if (!WorldGen.InWorld(scanX, scanY, 30))
                                continue;

                            Tile tile = Main.tile[scanX, scanY];

                            // Drain any liquids
                            if (tile.LiquidAmount > 0)
                            {
                                tile.LiquidAmount = 0;
                                tile.LiquidType = 0;
                                drainedCount++;
                            }
                        }
                    }

                    Mod.Logger.Info($"Drained {drainedCount} tiles with liquid above gate (Y < {gateY})");
                }
            }
            catch (Exception ex)
            {
                Mod.Logger.Error($"PyramidArenaIntegrationSystem: Error during pyramid integration: {ex.Message}");
                Mod.Logger.Error($"Stack trace: {ex.StackTrace}");
            }
        }

        private void PatchGenerateSOTSPyramid_IL(ILContext il)
        {
            var cursor = new ILCursor(il);

            try
            {
                // Patch #1: Initial nextAmount = WorldGen.genRand.Next(6, 16) → 10
                // Line 470: int nextAmount = WorldGen.genRand.Next(6, 16);
                if (cursor.TryGotoNext(MoveType.Before,
                    i => i.MatchLdcI4(6),
                    i => i.MatchLdcI4(16),
                    i => i.MatchCallvirt<Terraria.Utilities.UnifiedRandom>("Next")))
                {
                    // Move back one instruction to also remove the get_genRand() call
                    cursor.Index--;

                    // Remove 4 instructions: get_genRand(), ldc.i4.6, ldc.i4.s 16, callvirt Next
                    cursor.RemoveRange(4);

                    // Insert our constant value of 10
                    cursor.EmitLdcI4(10);

                    Mod.Logger.Info("Successfully patched initial nextAmount = WorldGen.genRand.Next(6, 16) → 10");
                }
                else
                {
                    Mod.Logger.Warn("Could not find initial WorldGen.genRand.Next(6, 16) pattern");
                }

                // Reset cursor to beginning for second patch
                cursor.Index = 0;

                // Patch #2: Loop nextAmount = WorldGen.genRand.Next(6, 31) → 10
                // Line 590: nextAmount = WorldGen.genRand.Next(6, 31);
                if (cursor.TryGotoNext(MoveType.Before,
                    i => i.MatchLdcI4(6),
                    i => i.MatchLdcI4(31),
                    i => i.MatchCallvirt<Terraria.Utilities.UnifiedRandom>("Next")))
                {
                    // Move back one instruction to also remove the get_genRand() call
                    cursor.Index--;

                    // Remove 4 instructions: get_genRand(), ldc.i4.6, ldc.i4.s 31, callvirt Next
                    cursor.RemoveRange(4);

                    // Insert our constant value of 10
                    cursor.EmitLdcI4(10);

                    Mod.Logger.Info("Successfully patched loop nextAmount = WorldGen.genRand.Next(6, 31) → 10");
                }
                else
                {
                    Mod.Logger.Warn("Could not find loop WorldGen.genRand.Next(6, 31) pattern");
                }

                // Reset cursor to beginning for third patch
                cursor.Index = 0;

                // Patch #3: Second divert corridor opposite side length: -length / 2 → -length / 5
                // Line 651: for (int cooridorX = -length / 2; cooridorX < length; cooridorX++)
                // IL pattern:
                //   ldloc.s 53      // load length
                //   neg             // negate it
                //   ldc.i4.2        // push 2  <-- We want to change this to 5
                //   div             // divide
                //   stloc.s 55      // store to loop variable
                if (cursor.TryGotoNext(MoveType.Before,
                    i => i.MatchLdcI4(20),     // int length = totalAmount + 20
                    i => i.MatchAdd()))
                {
                    // Move past the add and the stloc that stores 'length'
                    cursor.Index += 3;

                    // Now we should be at the loop initialization
                    // Find the pattern: ldloc, neg, ldc.i4.2, div
                    if (cursor.TryGotoNext(MoveType.Before,
                        i => i.MatchLdcI4(2),
                        i => i.MatchDiv()))
                    {
                        // Replace ldc.i4.2 with ldc.i4.5
                        cursor.Remove();
                        cursor.EmitLdcI4(5);

                        Mod.Logger.Info("Successfully patched second divert corridor: -length / 2 → -length / 5");
                    }
                    else
                    {
                        Mod.Logger.Warn("Could not find -length / 2 division pattern");
                    }
                }
                else
                {
                    Mod.Logger.Warn("Could not find 'length = totalAmount + 20' pattern");
                }

                // Reset cursor to beginning for fourth patch
                cursor.Index = 0;

                // Patch #4: Capture endingTileX before second corridor's GeneratePyramidPath call
                // IL pattern around the call:
                //   IL_056c: ldloc.s 6     // load endingTileX
                //   IL_056e: ldloc.s 53    // load length
                //   IL_0570: ldloc.3       // load finalDirection
                //   IL_0571: mul
                //   IL_0572: add
                //   IL_0573: ldc.i4.5
                //   IL_0574: ldloc.3       // load finalDirection again
                //   IL_0575: mul
                //   IL_0576: sub
                //   IL_0577: stloc.s 54    // store to spawnX
                //   IL_0579: ldarg.0       // GeneratePyramidPath call starts here
                //   ...
                //   IL_0597: ldc.i4.s 24   // max parameter
                //   IL_0599: ldc.i4.1      // surround = true

                if (cursor.TryGotoNext(MoveType.After,
                    i => i.MatchLdcI4(5),
                    i => i.MatchLdloc(3),   // finalDirection
                    i => i.MatchMul(),
                    i => i.MatchSub(),
                    i => i.MatchStloc(54))) // spawnX = endingTileX + length * finalDirection - 5 * finalDirection
                {
                    // Now we're right after stloc.s 54, which is the perfect place to capture endingTileX
                    // Check if the next few instructions lead to the max=24 call
                    cursor.EmitLdloc(6);  // Load endingTileX (num7)
                    cursor.EmitDelegate<Action<int>>((endingTileX) =>
                    {
                        storedEndingTileX = endingTileX;
                        ModContent.GetInstance<PyramidArenaIntegrationSystem>().Mod.Logger.Info(
                            $"Captured second corridor endingTileX: {endingTileX}");
                    });

                    Mod.Logger.Info("Successfully injected endingTileX capture before second corridor GeneratePyramidPath call");
                }
                else
                {
                    Mod.Logger.Warn("Could not find second corridor spawnX calculation pattern");
                }

                // Reset cursor to beginning for fifth patch
                cursor.Index = 0;

                // Patch #5: Change midPoint from size / 2 to size * 0.6
                // Line 492: int midPoint = size / 2;
                // IL pattern:
                //   ldloc.s 5       // load size
                //   ldc.i4.2        // push 2
                //   div             // divide
                //   stloc.s 12      // store to midPoint
                // We want to change to: size * 0.6 (integer)
                if (cursor.TryGotoNext(MoveType.Before,
                    i => i.MatchLdcI4(2),
                    i => i.MatchDiv()))
                {
                    // Check if the next instruction after div is stloc.s with index 12 (midPoint)
                    // Move to the ldc.i4.2 instruction
                    var ldcInstruction = cursor.Next;

                    // Remove ldc.i4.2 and div (2 instructions)
                    cursor.RemoveRange(2);

                    // Emit: conv.r4, ldc.r4 0.6, mul, conv.i4
                    cursor.EmitDelegate<Func<int, int>>(size => (int)(size * 0.6f));

                    Mod.Logger.Info("Successfully patched midPoint: size / 2 → size * 0.6");
                }
                else
                {
                    Mod.Logger.Warn("Could not find midPoint = size / 2 pattern");
                }

                Mod.Logger.Info("Pyramid zigzag depth is now fully deterministic (both initial and loop nextAmount = 10)");
            }
            catch (Exception ex)
            {
                Mod.Logger.Error($"Failed to apply IL patch to GenerateSOTSPyramid: {ex.Message}");
            }
        }

        private delegate void orig_GeneratePyramidPath(Mod mod, int spawnX, int spawnY, int endX, int endY, int direction, int max, bool surround);

        private void HookGeneratePyramidPath(orig_GeneratePyramidPath orig, Mod mod, int spawnX, int spawnY, int endX, int endY, int direction, int max, bool surround)
        {
            pyramidPathCallCount++;

            if (pyramidPathCallCount == 1)
            {
                // First corridor - SKIP this, arena will replace it
                Mod.Logger.Info($"Skipped first corridor GeneratePyramidPath (arena will be placed here)");
                Mod.Logger.Info($"  Would have been: spawnX={spawnX}, spawnY={spawnY}, endX={endX}, endY={endY}, direction={direction}, max={max}");

                // Place Fargo's Cursed Coffin arena here
                PlaceCoffinArena(spawnX, spawnY, direction);

                return;
            }
            else if (pyramidPathCallCount == 2)
            {
                // Second corridor - Execute original path
                Mod.Logger.Info($"Second corridor: executing original path");
                Mod.Logger.Info($"  spawnX={spawnX}, spawnY={spawnY}, endX={endX}, endY={endY}, direction={direction}, max={max}");
                orig(mod, spawnX, spawnY, endX, endY, direction, max, surround);

                // Before relocating gate, prevent horizontal corridors from spawning at gate level
                // Scan entire horizontal layer at gate level and replace PyramidSlabTile with PyramidBrickTile
                // (only where there's air directly above, so we don't affect solid pyramid structure)
                if (storedArenaBottom != 0)
                {
                    Vector2 pyramidLocation = (Vector2)placementLocationField.GetValue(null);
                    int pyramidX = (int)pyramidLocation.X;

                    int pyramidSlabType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidSlabTile")?.Type ?? -1;
                    int pyramidBrickType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidBrickTile")?.Type ?? -1;

                    if (pyramidSlabType != -1 && pyramidBrickType != -1)
                    {
                        int gateFloorY = storedArenaBottom - 1; // Gate will be placed at this level
                        const int scanWidth = 300; // Scan 300 tiles on each side of pyramid center

                        // Determine starting position (1 tile outside arena entrance)
                        int entranceX;
                        if (storedDirection == -1)
                            entranceX = storedArenaRight; // Arena on left, corridor on right
                        else
                            entranceX = storedArenaLeft + 1; // Arena on right, corridor on left

                        int scanStartX = entranceX + (storedDirection == -1 ? 1 : -1); // 1 tile outside entrance

                        int replacementCount = 0;
                        int firstReplacedX = -1;
                        int lastReplacedX = -1;

                        for (int scanX = pyramidX - scanWidth; scanX <= pyramidX + scanWidth; scanX++)
                        {
                            if (!WorldGen.InWorld(scanX, gateFloorY, 30))
                                continue;

                            Tile floorTile = Main.tile[scanX, gateFloorY];
                            Tile aboveTile = Main.tile[scanX, gateFloorY - 1];

                            // Only replace PyramidSlabTile that has air directly above it
                            if (floorTile.HasTile && floorTile.TileType == pyramidSlabType && !aboveTile.HasTile)
                            {
                                floorTile.TileType = (ushort)pyramidBrickType;
                                replacementCount++;

                                // Track first and last replaced tiles
                                if (firstReplacedX == -1)
                                    firstReplacedX = scanX;
                                lastReplacedX = scanX;
                            }
                        }

                        // Store replacement range for post-generation restoration
                        // Use firstReplacedX (not scanStartX) to capture the actual range of replaced tiles
                        replacedTilesStartX = firstReplacedX;
                        replacedTilesEndX = lastReplacedX;
                        replacedTilesY = gateFloorY;

                        Mod.Logger.Info($"Replaced {replacementCount} PyramidSlabTile → PyramidBrickTile at gate level (Y={gateFloorY}) to prevent horizontal corridors");
                        Mod.Logger.Info($"  Replacement range: X={replacedTilesStartX} to {replacedTilesEndX}");
                    }
                }

                // Relocate pyramid gate after zigzag has been carved
                // At this point, the zigzag corridors exist and we can find air gaps
                if (storedArenaBottom != 0)
                {
                    RelocatePyramidGate(storedMarkerX, storedMarkerY, storedDirection, storedArenaLeft, storedArenaRight, storedArenaBottom);

                    // Place vanilla pyramid chest with loot halfway between gate and arena entrance
                    // Do this right after gate placement, before other SOTS generation
                    if (storedNewGateX != -1 && storedCorridorFloorY != -1)
                    {
                        // Calculate arena entrance X
                        int entranceX;
                        if (storedDirection == -1)
                            entranceX = storedArenaRight; // Arena on left, entrance on right
                        else
                            entranceX = storedArenaLeft + 1; // Arena on right, entrance on left

                        // Calculate midpoint between gate and entrance
                        int chestX = (storedNewGateX + entranceX) / 2;
                        int chestY = storedCorridorFloorY; // Place on corridor floor

                        // Pick a random pyramid primary item (848 = Pharaoh Mask, 857 = Sandstorm in a Bottle, 934 = Flying Carpet)
                        int[] pyramidPrimaries = { 848, 857, 934 };
                        int pyramidItem = pyramidPrimaries[WorldGen.genRand.Next(pyramidPrimaries.Length)];

                        // Use WorldGen.AddBuriedChest to place a Gold Chest with pyramid loot
                        // Passing a pyramid primary (848/857/934) as 'contain' triggers the pyramid loot behavior
                        // This gives the pyramid item + surface-tier filler (ropes, bars, potions, ammo, etc.)
                        bool chestPlaced = WorldGen.AddBuriedChest(
                            i: chestX,                      // x coordinate
                            j: chestY,                      // y coordinate (searches down for floor)
                            contain: pyramidItem,           // pyramid primary item (triggers pyramid loot)
                            notNearOtherChests: false,      // notNearOtherChests
                            Style: 1,                       // gold chest style (ensures num9 != 0 for flag9)
                            trySlope: true,                 // trySlope
                            chestTileType: Terraria.ID.TileID.Containers  // TileID.Containers (21) = regular chest
                        );

                        if (chestPlaced)
                        {
                            Mod.Logger.Info($"Placed vanilla pyramid chest at ({chestX}, {chestY}) between gate and entrance");
                        }
                        else
                        {
                            Mod.Logger.Warn($"Failed to place vanilla pyramid chest at ({chestX}, {chestY})");
                        }
                    }
                }

                // Compute derived values from parameters
                // The original call passes direction = -finalDirection, so:
                int finalDirection = -direction;
                // Original: spawnX = endingTileX + length * finalDirection - 5 * finalDirection
                // Therefore: length = (spawnX - endingTileX) / finalDirection + 5
                int length = (spawnX - storedEndingTileX) / finalDirection + 5;

                // Calculate mirrored spawn position on opposite side of corridor
                int mirroredSpawnX = storedEndingTileX + -length / 5 * finalDirection + 5 * finalDirection;
                int mirroredEndX = mirroredSpawnX - WorldGen.genRand.Next(30, 51) * finalDirection;

                // Execute mirrored path with same direction (not negated)
                Mod.Logger.Info($"Second corridor: executing mirrored path");
                Mod.Logger.Info($"  Computed: finalDirection={finalDirection}, length={length}");
                Mod.Logger.Info($"  mirroredSpawnX={mirroredSpawnX}, spawnY={spawnY}, mirroredEndX={mirroredEndX}, endY={endY}, direction={direction}");
                orig(mod, mirroredSpawnX, spawnY, mirroredEndX, endY, direction, max, surround);
            }
            else if (pyramidPathCallCount == 3)
            {
                // Third call (final path at bottom) - Execute normally
                Mod.Logger.Info($"Third corridor (final path): executing normally");
                Mod.Logger.Info($"  spawnX={spawnX}, spawnY={spawnY}, endX={endX}, endY={endY}, direction={direction}, max={max}");
                orig(mod, spawnX, spawnY, endX, endY, direction, max, surround);
            }
            else
            {
                // Unexpected additional calls - log warning but execute
                Mod.Logger.Warn($"Unexpected GeneratePyramidPath call #{pyramidPathCallCount}");
                orig(mod, spawnX, spawnY, endX, endY, direction, max, surround);
            }
        }

        private void PlaceCoffinArena(int markerX, int markerY, int direction)
        {
            try
            {
                // Get SOTS tile/wall types
                int pyramidSlabType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidSlabTile")?.Type ?? -1;
                int pyramidBrickType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidBrickTile")?.Type ?? -1;
                int pyramidWallType = ModContent.Find<ModWall>(sotsMod.Name, "PyramidWallWall")?.Type ?? -1;
                int pyramidBrickWallType = ModContent.Find<ModWall>(sotsMod.Name, "PyramidBrickWallWall")?.Type ?? -1;

                if (pyramidSlabType == -1 || pyramidBrickType == -1 || pyramidWallType == -1 || pyramidBrickWallType == -1)
                {
                    Mod.Logger.Warn("Could not find required SOTS tile/wall types");
                    return;
                }

                Type coffinArenaType = fargoMod.Code.GetType("FargowiltasSouls.Content.WorldGeneration.CoffinArena");
                if (coffinArenaType == null)
                {
                    Mod.Logger.Warn("Could not find CoffinArena type in FargowiltasSouls");
                    return;
                }

                // Marker is at spawnX, spawnY
                // 2 air blocks below marker to corridor floor = markerY + 3
                // Arena top center Y should be 34 tiles above the bottom of arena
                // Arena is 35 tiles tall, bottom should align near corridor floor
                int corridorFloorY = markerY + 3;
                int arenaTopCenterY = corridorFloorY - 34;

                // Arena is 60 tiles wide
                // Position horizontally based on direction
                // Direction indicates which way the path curves
                int arenaTopCenterX;
                if (direction == -1)
                    arenaTopCenterX = markerX - 30; // Arena to the left
                else
                    arenaTopCenterX = markerX + 30; // Arena to the right

                Point arenaTopCenter = new Point(arenaTopCenterX, arenaTopCenterY);

                Mod.Logger.Info($"Placing Cursed Coffin Arena at ({arenaTopCenterX}, {arenaTopCenterY})");
                Mod.Logger.Info($"  Corridor floor estimated at Y={corridorFloorY}, direction={direction}");

                // Fill solid rectangle for arena shell first (arena will carve out interior)
                // Arena is 60x35, add 5 blocks padding on all sides except corridor side
                const int thickness = 5;
                const int arenaWidth = 60;
                const int arenaHeight = 35;

                int shellX = arenaTopCenterX - arenaWidth / 2 - thickness;
                int shellY = arenaTopCenterY - thickness;
                int shellWidth = arenaWidth + thickness * 2;
                int shellHeight = arenaHeight + thickness * 2;

                // Adjust for corridor side (remove padding on that side)
                if (direction == -1)
                {
                    // Arena on left, corridor on right - don't extend shell to the right
                    shellWidth -= thickness;
                    shellX -= 1; // Push shell 1 unit to the left
                }
                else
                {
                    // Arena on right, corridor on left - don't extend shell to the left
                    shellX += thickness;
                    shellWidth -= thickness;
                    shellX += 1; // Push shell 1 unit to the right
                }

                FillRectangleWithTile(shellX, shellY, shellWidth, shellHeight, pyramidSlabType, pyramidBrickWallType);
                Mod.Logger.Info($"Filled arena shell: X={shellX}, Y={shellY}, Width={shellWidth}, Height={shellHeight}");

                // Call Fargo's arena placement (will carve out the interior)
                MethodInfo placeMethod = coffinArenaType.GetMethod("Place", BindingFlags.Public | BindingFlags.Static);
                if (placeMethod == null)
                {
                    Mod.Logger.Warn("Could not find Place method on CoffinArena");
                    return;
                }

                placeMethod.Invoke(null, new object[] { arenaTopCenter });

                // Calculate arena bounds manually (60 wide, 35 tall)
                int arenaLeft = arenaTopCenterX - 30;
                int arenaRight = arenaTopCenterX + 30;
                int arenaBottom = arenaTopCenterY + 35;

                // Determine entrance X position
                // Note: Width is even (60), so centering is asymmetric
                int entranceX;
                if (direction == -1)
                    entranceX = arenaRight; // Arena on left, corridor on right
                else
                    entranceX = arenaLeft + 1; // Arena on right, corridor on left (adjust for even width)

                Mod.Logger.Info($"Arena placement: direction={direction}, arenaLeft={arenaLeft}, arenaRight={arenaRight}, arenaBottom={arenaBottom}");
                Mod.Logger.Info($"Entrance position calculated: entranceX={entranceX}");

                // Clear entrance passage (using WorldGen.KillTile like PV4)
                for (int yOffset = 2; yOffset <= 6; yOffset++)
                {
                    int openingY = arenaBottom - yOffset;

                    if (WorldGen.InWorld(entranceX, openingY, 30))
                    {
                        Tile tile = Main.tile[entranceX, openingY];
                        if (tile.HasTile)
                        {
                            WorldGen.KillTile(entranceX, openingY, noItem: true);
                        }

                        // Ensure safe pyramid wall is present
                        if (tile.WallType == 0)
                        {
                            WorldGen.PlaceWall(entranceX, openingY, pyramidWallType, mute: true);
                        }
                    }
                }

                // Replace vanilla Sandstone tiles/walls with SOTS pyramid tiles/walls
                Rectangle arenaRect = new Rectangle(arenaLeft, arenaTopCenterY, arenaWidth, arenaHeight);
                ReplaceSandstoneWithPyramid(arenaRect, pyramidBrickType, pyramidWallType, pyramidBrickWallType);

                // Diagnostic: Check pot styles in bottom-left 7x7 of arena
                Mod.Logger.Info("=== Scanning bottom-left 7x7 of Fargo arena for pot styles ===");
                for (int dx = 0; dx < 7; dx++)
                {
                    for (int dy = 0; dy < 7; dy++)
                    {
                        int checkX = arenaLeft + dx;
                        int checkY = arenaBottom - 7 + dy;

                        if (WorldGen.InWorld(checkX, checkY, 30))
                        {
                            Tile tile = Main.tile[checkX, checkY];
                            if (tile.HasTile && tile.TileType == Terraria.ID.TileID.Pots)
                            {
                                // Calculate style from TileFrameX/Y
                                int style = tile.TileFrameX / 36 + tile.TileFrameY / 36 * 100;
                                Mod.Logger.Info($"  Pot at ({checkX}, {checkY}): TileFrameX={tile.TileFrameX}, TileFrameY={tile.TileFrameY}, calculated style={style}");
                            }
                        }
                    }
                }
                Mod.Logger.Info("=== End pot style scan ===");

                // Store arena context for gate relocation (will happen in 2nd PyramidPath call after zigzag is carved)
                storedDirection = direction;
                storedArenaLeft = arenaLeft;
                storedArenaRight = arenaRight;
                storedArenaBottom = arenaBottom;
                storedMarkerX = markerX;
                storedMarkerY = markerY;

                // Set threshold to prevent rooms/shrines/ovals/infection/chests from generating above gate
                // Set threshold 7 tiles below gate for extra clearance
                int gateY = arenaBottom - 1; // Gate will be placed at arenaBottom - 1
                gateThresholdY = gateY + 7; // 7 tiles below gate
                Mod.Logger.Info($"Set gate threshold Y={gateThresholdY} (gate at Y={gateY}, threshold 7 tiles below)");

                // Mark arena as indestructible via Mutant Mod
                PropertyInfo paddedRectangleProperty = coffinArenaType.GetProperty("PaddedRectangle", BindingFlags.Public | BindingFlags.Static);
                if (paddedRectangleProperty == null)
                {
                    Mod.Logger.Warn("Could not find PaddedRectangle property on CoffinArena");
                    return;
                }

                Rectangle paddedRectangle = (Rectangle)paddedRectangleProperty.GetValue(null);

                Rectangle worldRect = new Rectangle(
                    paddedRectangle.X * 16,
                    paddedRectangle.Y * 16,
                    paddedRectangle.Width * 16,
                    paddedRectangle.Height * 16
                );

                Type fargoMainType = fargoMod.Code.GetType("FargowiltasSouls.FargowiltasSouls");
                if (fargoMainType == null)
                {
                    Mod.Logger.Warn("Could not find FargowiltasSouls main type");
                    return;
                }

                PropertyInfo mutantModProperty = fargoMainType.GetProperty("MutantMod", BindingFlags.Public | BindingFlags.Static);
                if (mutantModProperty == null)
                {
                    Mod.Logger.Warn("Could not find MutantMod property");
                    return;
                }

                Mod mutantMod = (Mod)mutantModProperty.GetValue(null);
                if (mutantMod == null)
                {
                    Mod.Logger.Warn("MutantMod is null");
                    return;
                }

                mutantMod.Call("AddIndestructibleRectangle", worldRect);
                Mod.Logger.Info($"Arena marked as indestructible: {paddedRectangle}");
            }
            catch (Exception ex)
            {
                Mod.Logger.Error($"Error placing Cursed Coffin Arena: {ex.Message}");
                Mod.Logger.Error($"Stack trace: {ex.StackTrace}");
            }
        }

        private void FillRectangleWithTile(int x, int y, int width, int height, int tileType, int wallType)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int tileX = x + i;
                    int tileY = y + j;

                    if (WorldGen.InWorld(tileX, tileY, 30))
                    {
                        Tile tile = Main.tile[tileX, tileY];
                        tile.HasTile = true;
                        tile.TileType = (ushort)tileType;

                        // Also place wall behind the tile
                        tile.WallType = (ushort)wallType;
                    }
                }
            }
        }

        private void ReplaceSandstoneWithPyramid(Rectangle arenaRect, int pyramidBrickType, int pyramidWallType, int pyramidBrickWallType)
        {
            // Expand the rectangle slightly to include padded area
            int scanX = arenaRect.X - 2;
            int scanY = arenaRect.Y - 2;
            int scanWidth = arenaRect.Width + 4;
            int scanHeight = arenaRect.Height + 4;

            for (int i = 0; i < scanWidth; i++)
            {
                for (int j = 0; j < scanHeight; j++)
                {
                    int tileX = scanX + i;
                    int tileY = scanY + j;

                    if (WorldGen.InWorld(tileX, tileY, 30))
                    {
                        Tile tile = Main.tile[tileX, tileY];

                        // Replace vanilla SandstoneBrick tile with PyramidBrickTile
                        // PyramidBrickTile prevents SOTS corridor generation from carving through
                        if (tile.HasTile && tile.TileType == Terraria.ID.TileID.SandstoneBrick)
                        {
                            tile.TileType = (ushort)pyramidBrickType;
                        }

                        // Replace walls (preserving paint)
                        byte paintColor = tile.WallColor; // Save paint

                        if (tile.WallType == Terraria.ID.WallID.SandstoneBrick)
                        {
                            // SandstoneBrick wall → PyramidWallWall
                            tile.WallType = (ushort)pyramidWallType;
                            tile.WallColor = paintColor; // Restore paint
                        }
                        else if (tile.WallType == Terraria.ID.WallID.Sandstone ||
                                 tile.WallType == Terraria.ID.WallID.HardenedSand ||
                                 tile.WallType == Terraria.ID.WallID.SandstoneEcho ||
                                 tile.WallType == Terraria.ID.WallID.HardenedSandEcho ||
                                 tile.WallType == Terraria.ID.WallID.SmoothSandstone)
                        {
                            // Sandstone/HardenedSand/Echo/SmoothSandstone walls → PyramidBrickWallWall
                            tile.WallType = (ushort)pyramidBrickWallType;
                            tile.WallColor = paintColor; // Restore paint
                        }

                        // Drain any liquids
                        if (tile.LiquidAmount > 0)
                        {
                            tile.LiquidAmount = 0;
                            tile.LiquidType = 0;
                        }
                    }
                }
            }

            Mod.Logger.Info($"Replaced Sandstone with Pyramid tiles/walls and drained liquids in arena area");
        }

        private void RelocatePyramidGate(int markerX, int markerY, int direction, int arenaLeft, int arenaRight, int arenaBottom)
        {
            try
            {
                // Get pyramid wall types
                int pyramidBrickWallType = ModContent.Find<ModWall>(sotsMod.Name, "PyramidBrickWallWall")?.Type ?? -1;
                int pyramidWallType = ModContent.Find<ModWall>(sotsMod.Name, "PyramidWallWall")?.Type ?? -1;

                if (pyramidBrickWallType == -1 || pyramidWallType == -1)
                {
                    Mod.Logger.Warn("Could not find required SOTS wall types for gate relocation");
                    return;
                }

                // Get pyramid location
                Vector2 pyramidLocation = (Vector2)placementLocationField.GetValue(null);
                int pyramidX = (int)pyramidLocation.X;
                int pyramidY = (int)pyramidLocation.Y;

                // Calculate corridor floor Y at arena floor level (one block above arena bottom)
                int corridorFloorY = arenaBottom - 1;

                // Remove original gate
                RemoveOriginalGate(pyramidX, pyramidY, pyramidBrickWallType, pyramidWallType);

                // Find gate position by scanning from arena entrance outward into corridor
                // Start at entrance side of arena, scan opposite of arena placement direction
                int startX;
                int scanDirection;

                if (direction == -1)
                {
                    // Arena on left, entrance on right side
                    startX = arenaRight;
                    scanDirection = 1; // Scan right (away from arena)
                }
                else
                {
                    // Arena on right, entrance on left side
                    startX = arenaLeft;
                    scanDirection = -1; // Scan left (away from arena)
                }

                // Calculate maximum scan distance based on pyramid width at this Y level
                // Pyramid grows by 1 block on each side per layer down from the top
                int yOffset = corridorFloorY - pyramidY;
                int pyramidHalfWidth = yOffset + 1;
                int maxScanDistance = pyramidHalfWidth * 2; // Scan full pyramid width to be safe

                Mod.Logger.Info($"Gate scanning: startX={startX}, corridorFloorY={corridorFloorY}, scanDirection={scanDirection}, maxScanDistance={maxScanDistance}");

                int newGateX = FindGatePositionFromArena(startX, corridorFloorY, scanDirection, maxScanDistance);
                if (newGateX == -1)
                {
                    Mod.Logger.Warn("Could not find suitable position for pyramid gate");
                    return;
                }

                // Place new gate at arena floor level
                PlaceNewGate(newGateX, corridorFloorY, pyramidBrickWallType);

                // Store gate position for post-generation unsafe wall replacement
                storedNewGateX = newGateX;
                storedCorridorFloorY = corridorFloorY;

                Mod.Logger.Info($"Relocated pyramid gate to ({newGateX}, {corridorFloorY})");
            }
            catch (Exception ex)
            {
                Mod.Logger.Error($"Error relocating pyramid gate: {ex.Message}");
            }
        }

        private void RemoveOriginalGate(int pyramidX, int pyramidY, int pyramidBrickWallType, int pyramidWallType)
        {
            if (pyramidGateTileType == -1 || royalGoldBrickTileType == -1)
                return;

            const int originalGateYOffset = 16;
            const int scanWidth = 300;

            int originalGateY = pyramidY + originalGateYOffset;

            for (int x = pyramidX - scanWidth; x < pyramidX + scanWidth; x++)
            {
                if (!WorldGen.InWorld(x, originalGateY, 30))
                    continue;

                Tile tile = Main.tile[x, originalGateY];

                if (tile.HasTile && tile.TileType == pyramidGateTileType)
                {
                    // Calculate gate origin from tile frame
                    int gateOriginX = x - tile.TileFrameX / 18 + 2;

                    WorldGen.KillTile(gateOriginX, originalGateY, noItem: true);
                    RemoveGoldBricksAndReplaceWalls(gateOriginX, originalGateY, pyramidBrickWallType, pyramidWallType);

                    Mod.Logger.Info($"Removed original gate at ({gateOriginX}, {originalGateY})");
                    return;
                }
            }
        }

        private void RemoveGoldBricksAndReplaceWalls(int gateOriginX, int gateY, int pyramidBrickWallType, int pyramidWallType)
        {
            int leftGoldX = gateOriginX - 3;
            int rightGoldX = gateOriginX + 3;

            Tile leftTile = Main.tile[leftGoldX, gateY];
            if (leftTile.HasTile && leftTile.TileType == royalGoldBrickTileType)
            {
                WorldGen.KillTile(leftGoldX, gateY, noItem: true);
            }

            Tile rightTile = Main.tile[rightGoldX, gateY];
            if (rightTile.HasTile && rightTile.TileType == royalGoldBrickTileType)
            {
                WorldGen.KillTile(rightGoldX, gateY, noItem: true);
            }

            // Replace PyramidBrickWall with PyramidWall in the gate area
            for (int xOffset = -3; xOffset <= 3; xOffset++)
            {
                int checkX = gateOriginX + xOffset;
                if (!WorldGen.InWorld(checkX, gateY, 30))
                    continue;

                Tile tile = Main.tile[checkX, gateY];

                if (tile.WallType == pyramidBrickWallType)
                {
                    tile.WallType = (ushort)pyramidWallType;
                }
            }
        }

        private int FindGatePositionFromArena(int startX, int floorY, int scanDirection, int maxScanDistance)
        {
            const int requiredGapWidth = 7;

            // Look for PyramidBrickTile since we've already replaced slabs with bricks at gate level
            int pyramidBrickType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidBrickTile")?.Type ?? -1;
            if (pyramidBrickType == -1)
                return -1;

            Mod.Logger.Info($"=== Starting gate position scan ===");

            // Scan from arena entrance outward into corridor
            for (int distance = 1; distance <= maxScanDistance; distance++)
            {
                int checkX = startX + distance * scanDirection;

                if (!WorldGen.InWorld(checkX, floorY, 30))
                {
                    Mod.Logger.Info($"  Distance {distance}: X={checkX} - OUT OF BOUNDS");
                    break;
                }

                Tile tile = Main.tile[checkX, floorY];
                string tileInfo = tile.HasTile ? $"{GetTileDebugName(tile.TileType)}" : "Air";

                Mod.Logger.Info($"  Distance {distance}: X={checkX} - {tileInfo} (HasTile={tile.HasTile})");

                // Check if this is the start of a 7-tile air gap
                // SOTS corridors are carved by setting HasTile = false, so look for actual air
                int gapStartX = checkX;
                bool validGap = true;

                // Verify all 7 tiles in the gap are air (HasTile = false)
                for (int i = 0; i < requiredGapWidth; i++)
                {
                    int gapX = gapStartX + i * scanDirection;
                    if (!WorldGen.InWorld(gapX, floorY, 30))
                    {
                        validGap = false;
                        break;
                    }

                    Tile gapTile = Main.tile[gapX, floorY];
                    if (gapTile.HasTile)
                    {
                        validGap = false;
                        break;
                    }
                }

                if (!validGap)
                    continue;

                Mod.Logger.Info($"  -> Found 7-tile air gap starting at {gapStartX}");

                // Check that there's PyramidSlabTile on both sides of the gap
                int leftSideX = gapStartX - scanDirection;
                int rightSideX = gapStartX + requiredGapWidth * scanDirection;

                if (!WorldGen.InWorld(leftSideX, floorY, 30) || !WorldGen.InWorld(rightSideX, floorY, 30))
                {
                    Mod.Logger.Info($"  -> Boundary check failed (out of world)");
                    continue;
                }

                Tile leftTile = Main.tile[leftSideX, floorY];
                Tile rightTile = Main.tile[rightSideX, floorY];

                Mod.Logger.Info($"  -> Left boundary (X={leftSideX}): {GetTileDebugName(leftTile.HasTile ? leftTile.TileType : -1)} (HasTile={leftTile.HasTile})");
                Mod.Logger.Info($"  -> Right boundary (X={rightSideX}): {GetTileDebugName(rightTile.HasTile ? rightTile.TileType : -1)} (HasTile={rightTile.HasTile})");

                if (leftTile.HasTile && leftTile.TileType == pyramidBrickType &&
                    rightTile.HasTile && rightTile.TileType == pyramidBrickType)
                {
                    // Found valid gap with PyramidBrickTile on both sides
                    // Return the center of the gap
                    int gapEndX = gapStartX + (requiredGapWidth - 1) * scanDirection;
                    int gateCenterX = (gapStartX + gapEndX) / 2;

                    Mod.Logger.Info($"  -> VALID! Placing gate at center X={gateCenterX}");
                    return gateCenterX;
                }
                else
                {
                    Mod.Logger.Info($"  -> Boundary check failed (not PyramidBrickTile)");
                }
            }

            Mod.Logger.Info($"=== Gate scan completed - no valid position found ===");
            return -1;
        }

        private string GetTileDebugName(int tileType)
        {
            if (tileType == -1)
                return "Air";

            int pyramidSlabType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidSlabTile")?.Type ?? -1;
            if (tileType == pyramidSlabType)
                return "PyramidSlabTile";

            int pyramidBrickType = ModContent.Find<ModTile>(sotsMod.Name, "PyramidBrickTile")?.Type ?? -1;
            if (tileType == pyramidBrickType)
                return "PyramidBrickTile";

            if (tileType == Terraria.ID.TileID.SandstoneBrick)
                return "SandstoneBrick";

            return $"TileType{tileType}";
        }

        private void PlaceNewGate(int gateX, int gateY, int pyramidBrickWallType)
        {
            if (pyramidGateTileType == -1 || royalGoldBrickTileType == -1)
                return;

            int leftGoldX = gateX - 3;
            int rightGoldX = gateX + 3;

            // Place gold bricks on sides
            Tile leftBrick = Main.tile[leftGoldX, gateY];
            leftBrick.HasTile = true;
            leftBrick.TileType = (ushort)royalGoldBrickTileType;

            Tile rightBrick = Main.tile[rightGoldX, gateY];
            rightBrick.HasTile = true;
            rightBrick.TileType = (ushort)royalGoldBrickTileType;

            // Place pyramid brick walls in the gate area
            for (int xOffset = -3; xOffset <= 3; xOffset++)
            {
                int checkX = gateX + xOffset;
                Tile tile = Main.tile[checkX, gateY];
                tile.WallType = (ushort)pyramidBrickWallType;
            }

            // Place the gate tile
            WorldGen.PlaceTile(gateX, gateY, pyramidGateTileType, mute: true, forced: true);
        }

        private void ReplaceUnsafeWallsAboveGate(int pyramidX, int pyramidY, int gateX, int gateY, int pyramidWallType)
        {
            int unsafePyramidWallType = ModContent.Find<ModWall>(sotsMod.Name, "UnsafePyramidWallWall")?.Type ?? -1;
            if (unsafePyramidWallType == -1 || pyramidWallType == -1)
                return;

            const int scanWidth = 300;

            for (int x = pyramidX - scanWidth; x < pyramidX + scanWidth; x++)
            {
                for (int y = pyramidY; y <= gateY; y++)
                {
                    if (!WorldGen.InWorld(x, y, 30))
                        continue;

                    Tile tile = Main.tile[x, y];
                    if (tile.WallType == unsafePyramidWallType)
                    {
                        tile.WallType = (ushort)pyramidWallType;
                    }
                }
            }

            Mod.Logger.Info("Replaced unsafe pyramid walls above gate");
        }

        // Hooks to prevent generation above gate threshold

        private delegate void orig_GeneratePyramidRoom(int x, int y, int direction, Mod mod);

        private void HookGeneratePyramidRoom(orig_GeneratePyramidRoom orig, int x, int y, int direction, Mod mod)
        {
            // Direction: 0 = left, 1 = right, 2 = up, 3 = down
            //
            // CRITICAL: Before placing the room, GeneratePyramidRoom carves a CORRIDOR in the specified direction.
            // For upward rooms (direction == 2), the corridor carves up to 300 tiles UPWARD from spawn point,
            // clearing ALL tiles except TrueSandstone/CursedTumor (PyramidGateTile is NOT protected!).
            //
            // This corridor WILL destroy the gate if it passes through, so we must block upward rooms
            // from spawning anywhere that could carve into the gate area.
            //
            // Gate protection calculation:
            // - Gate is at gateY (= arenaBottom - 1)
            // - Threshold is at gateThresholdY (= gateY + 7, i.e., 7 tiles below gate)
            // - Upward corridor can carve up to 300 tiles (in practice stops at pyramid edge ~30-50 tiles)
            // - We add extra padding (50 tiles) to ensure no upward corridor can reach the gate

            const int upwardCorridorPadding = 50; // Padding to prevent upward corridors from reaching gate

            if (direction == 2)
            {
                // For upward rooms, check if the corridor's upward extent would reach protected zone
                // If room spawns at y and carves upward, check (y - padding) against threshold
                if (IsAboveGateThreshold(y - upwardCorridorPadding))
                    return;
            }
            else
            {
                // For other directions (left/right/down), only check the spawn point
                if (IsAboveGateThreshold(y))
                    return;
            }

            orig(x, y, direction, mod);
        }

        private delegate void orig_GeneratePyramidCrystalRoom(Mod mod, int spawnX, int spawnY);

        private void HookGeneratePyramidCrystalRoom(orig_GeneratePyramidCrystalRoom orig, Mod mod, int spawnX, int spawnY)
        {
            if (IsAboveGateThreshold(spawnY))
                return;

            orig(mod, spawnX, spawnY);
        }

        private delegate void orig_GenerateShrineRoom(int x, int y, Mod mod, int type);

        private void HookGenerateShrineRoom(orig_GenerateShrineRoom orig, int x, int y, Mod mod, int type)
        {
            if (IsAboveGateThreshold(y))
                return;

            orig(x, y, mod, type);
        }

        private delegate void orig_GeneratePyramidOval(Mod mod, int spawnX, int spawnY, int radius, int radiusY, float thickMult, int radiusConversion, bool extraRestriction);

        private void HookGeneratePyramidOval(orig_GeneratePyramidOval orig, Mod mod, int spawnX, int spawnY, int radius, int radiusY, float thickMult, int radiusConversion, bool extraRestriction)
        {
            if (IsAboveGateThreshold(spawnY))
                return;

            orig(mod, spawnX, spawnY, radius, radiusY, thickMult, radiusConversion, extraRestriction);
        }

        private delegate void orig_GenerateInfection(Vector2 pos, Mod mod, bool item);

        private void HookGenerateInfection(orig_GenerateInfection orig, Vector2 pos, Mod mod, bool item)
        {
            int yTile = (int)(pos.Y / 16f);
            if (IsAboveGateThreshold(yTile))
                return;

            orig(pos, mod, item);
        }
    }
}
