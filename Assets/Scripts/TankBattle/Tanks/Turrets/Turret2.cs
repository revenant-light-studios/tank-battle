using UnityEngine;

namespace TankBattle.Tanks.Turrets
{
    public class Turret2 : ATankTurret
    {
        private Vector3 _turretRotation;
        
        private void Awake()
        {
            _turretRotation = Vector3.zero;
        }
        public override void UpdateTurret(Vector3 angle)
        {
            transform.eulerAngles = angle;
        }
    }
}