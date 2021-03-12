using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.Turrets
{
    public class Turret2 : ATankTurret
    {
        public CrossHair Crosshair;
        
        [SerializeField, FormerlySerializedAs("TurretRotationSpeed")] private float _turretRotationSpeed = 30f;
        [SerializeField, FormerlySerializedAs("DeadZone")] private float _deadZone = 10f;
        
        private Vector3 _turretRotation;
        
        public override Vector3 MousePosition
        {
            set => _mousePosition = value;
        }

        private Vector3 _mousePosition;


        private void Start()
        {
            _turretRotation = Vector3.zero;
        }
        private void Update()
        {
            if (_mousePosition.x < 0 || _mousePosition.x > Screen.width) 
                return;
         
            float _halfScreenWidth = Screen.width * 0.5f;
            float displacement = (_mousePosition.x - _halfScreenWidth) / _halfScreenWidth;
            // Debug.Log($"Mouse position: {_mousePosition}, displacement {displacement}");

            if (Mathf.Abs(displacement) > _deadZone)
            {
                _turretRotation.y += displacement * Time.deltaTime * _turretRotationSpeed;
                transform.localRotation = Quaternion.Euler(_turretRotation);
            }
        }
    }
}