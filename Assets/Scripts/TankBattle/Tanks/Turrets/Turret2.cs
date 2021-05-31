using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Turrets
{
    public class Turret2 : ATankTurret
    {
        public CrossHair Crosshair;
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