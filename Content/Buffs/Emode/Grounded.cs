using FargowiltasSouls.Content.Buffs.Eternity;
using Terraria;

namespace SecretsOfTheSouls.Content.Buffs.Emode
{
    public class Grounded : ClippedWingsBuff
    {
        public override string Texture => "FargowiltasSouls/Assets/Textures/Content/Buffs/Eternity/ClippedWingsBuff";
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);

            player.empressBrooch = false;
        }
    }
}
