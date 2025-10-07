using Terraria.ModLoader;

namespace SecretsOfTheSouls.Core.Systems
{
    public class ModIntegrationsSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            MusicDisplaySetup();
        }

        private void MusicDisplaySetup()
        {
            if (!SecretsOfTheSoulsCrossmod.MusicDisplay.Loaded)
                return;

            Mod musicDisplay = SecretsOfTheSoulsCrossmod.MusicDisplay.Mod;

            musicDisplay.Call("AddMusic", (short)MusicLoader.GetMusicSlot("FargoSoulsSOTS/Assets/Sounds/Music/ItsTVTime"), "It's TV Time!", "Toby Fox", "Secrets of the Souls");
        }
    }
}
