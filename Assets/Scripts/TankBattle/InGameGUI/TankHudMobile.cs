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
        
        public VirtualJoystick MovementJoystick { get => _movementJoystick; }
        
        public VirtualJoystick AimJoystick { get => _aimJoystick; }
        
        public override void RegisterTank(GameObject tank)
        {
            base.RegisterTank(tank);
            
        }
    }
}