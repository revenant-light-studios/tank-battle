using TankBattle.InputManagers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TankBattle.InGameGUI
{
    public class TankHudMobile : TankHud
    {
        [SerializeField, FormerlySerializedAs("MovementJoystick")]
        private VirtualJoystick _movementJoystick;
        
        [SerializeField, FormerlySerializedAs("AimJoystick")]
        private VirtualJoystick _aimJoystick;

        [SerializeField, FormerlySerializedAs("Shoot")]
        private ShootBtn _shootBtn;

        [SerializeField, FormerlySerializedAs("SpecialShoot")]
        private Button _specialShootBtn;

        public VirtualJoystick MovementJoystick { get => _movementJoystick; }
        
        public VirtualJoystick AimJoystick { get => _aimJoystick; }
        
        public ShootBtn ShootBtn { get => _shootBtn; }

        public Button SpecialShootBtn { get => _specialShootBtn; }
        
        public override void RegisterTank(GameObject tank)
        {
            base.RegisterTank(tank);
            
        }
    }
}