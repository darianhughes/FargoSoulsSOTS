using Terraria.ModLoader;

namespace FargoSoulsSOTS.Core.Systems
{
    public class ModIntegrationsSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            MusicDisplaySetup();
        }

        private void MusicDisplaySetup()
        {
            if (!FargoSOTSCrossmod.MusicDisplay.Loaded)
                return;

            Mod musicDisplay = FargoSOTSCrossmod.MusicDisplay.Mod;

            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("FargoSoulsSOTS/Assets/Sounds/Music/ItsTVTime"), "It's TV Time!", "Toby Fox", "Secrets of the Souls");
        }
    }
}
