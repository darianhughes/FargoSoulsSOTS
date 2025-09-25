using FargowiltasSouls.Content.Buffs.Masomode;
using Terraria;

namespace FargoSoulsSOTS.Content.Buffs.Emode
{
    public class Grounded : ClippedWingsBuff
    {
        public override string Texture => "FargowiltasSouls/Content/Buffs/Masomode/ClippedWingsBuff";
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);

            player.empressBrooch = false;
        }
    }
}
