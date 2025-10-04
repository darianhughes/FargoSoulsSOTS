using System;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Terraria.ModLoader;
using Terraria;

namespace FargoSoulsSOTS.Core.Systems.Hooks.Fargowiltas
{
    public class SquirrelSupportPatch : ModSystem
    {
        private static Hook _isFargoSoulsItemHook;

        public override void Load()
        {
            try
            {
                if (!ModLoader.TryGetMod("Fargowiltas", out var fargo))
                    return;

                // Get the Squirrel type from the Fargowiltas assembly
                var squirrelType = fargo.Code?.GetType("Fargowiltas.NPCs.Squirrel");
                if (squirrelType == null)
                    return;

                var mi = squirrelType.GetMethod(
                    "IsFargoSoulsItem",
                    BindingFlags.Public | BindingFlags.Static);

                if (mi == null)
                    return;

                _isFargoSoulsItemHook = new Hook(mi, (Func<Item, bool>)Patched_IsFargoSoulsItem);
            }
            catch
            {
                // Swallow: if something goes wrong, just don't patch
            }
        }

        public override void Unload()
        {
            _isFargoSoulsItemHook?.Dispose();
            _isFargoSoulsItemHook = null;
        }

        private static bool Patched_IsFargoSoulsItem(Item item)
        {
            if (item?.ModItem is null)
                return false;

            string modName = item.ModItem.Mod?.Name;
            if (modName is null)
                return false;

            return modName.Equals("FargowiltasSouls")
                || modName.Equals("FargowiltasSoulsDLC")
                || modName.Equals("FargoSoulsSOTS");
        }
    }
}
