using TankBattle.InputManagers;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.InGameGUI
{
    public class TankHudMobile : TankHud
    {
        [SerializeField, FormerlySerializedAs("MovementJoystick")]
        private VirtualJoystick _movementJoystick;
        
        [SerializeField, FormerlySerializedAs("AimJoystick")]
        private VirtualJoystick _aimJoystick;

        [SerializeField, FormerlySerializedAs("Shoot")]
        private VirtualButton _shootBtn;

        [SerializeField, FormerlySerializedAs("SpecialShoot")]
        private VirtualButton _specialShootBtn;

        public VirtualJoystick MovementJoystick { get => _movementJoystick; }
        
        public VirtualJoystick AimJoystick { get => _aimJoystick; }
        
        public VirtualButton ShootBtn { get => _shootBtn; }

        public VirtualButton SpecialShootBtn { get => _specialShootBtn; }
        
        public override void RegisterTank(GameObject tank)
        {
            base.RegisterTank(tank);
        }
    }
}