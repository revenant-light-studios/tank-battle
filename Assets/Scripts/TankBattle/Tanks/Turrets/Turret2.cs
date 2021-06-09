using ExtensionMethods;
using UnityEngine;

namespace TankBattle.Tanks.Turrets
{
    public class Turret2 : ATankTurret
    {
        private Vector3 _turretRotation;
        private Transform _firePoint;

        private void Awake()
        {
            _turretRotation = Vector3.zero;
            _firePoint = transform.FirstOrDefault(t => t.name == "FirePoint");
        }
        public override void UpdateTurret(Vector3 angle)
        {
            transform.eulerAngles = angle;
            
            if (Physics.Raycast(_firePoint.transform.position, _firePoint.transform.forward, out RaycastHit hit))
            {
                _onTurretMove?.Invoke(hit.point);
            }
        }
    }
}