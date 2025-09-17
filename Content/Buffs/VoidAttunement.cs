using Terraria.ID;
using Terraria.ModLoader;

namespace FargoSoulsSOTS.Content.Buffs
{
    public class VoidAttunement : ModBuff
    {
        public override string Texture => "SOTS/Buffs/VoidAccess";

        public override void SetStaticDefaults()
        {
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
    }
}
