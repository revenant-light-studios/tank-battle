using System;
using ExtensionMethods;
using TankBattle.Tanks;
using TankBattle.Tanks.Turrets;
using UnityEngine;

namespace TankBattle.InGameGUI
{
    public class TankHud : ATankHud
    {
        private ValueBar _lifeBar;
        private ValueBar _shieldBar;
        private CrossHair _crossHair;
        
        private TankValues _tankValues;
        private HitImage _hitImage;

        private void Awake()
        {
            _crossHair = transform.FirstOrDefault(t => t.name == "Crosshair").GetComponent<CrossHair>();
            _hitImage = transform.FirstOrDefault(t => t.name == "HitImage")?.GetComponent<HitImage>();
            _lifeBar = transform.FirstOrDefault(t => t.name == "LifeBar")?.GetComponent<ValueBar>();
            _shieldBar = transform.FirstOrDefault(t => t.name == "ShieldBar")?.GetComponent<ValueBar>();
        }
        
        public override void RegisterTank(TankManager tankManager)
        {
            _tankValues = tankManager.GetComponent<TankValues>();
            if (_tankValues != null)
            {
                _tankValues.OnValuesChanged = OnTankValuesChanged;
                _tankValues.OnTankWasHit = OnTankWasHit;
            }

            if (tankManager.Turret && _crossHair)
            {
                tankManager.Turret.OnTurretMove += OnTurretMove;
            }

            tankManager.OnTankWeaponEnabled += (gun, weapon) =>
            {
                if (weapon == TankManager.TankWeapon.Primary)
                {
                    gun.OnEnergyUpdate += _crossHair.UpdateEnergy;
                }
            };

            // Debug.LogFormat("Tank {0} registered with hud", tankManager.name);
        }
        private void OnTurretMove(Vector3 hitPoint)
        {
            Vector3 position = Camera.main.WorldToScreenPoint(hitPoint);
            _crossHair.UpdatePosition(position);
        }

        private void OnTankWasHit(TankValues values)
        { 
            _hitImage?.HitFlash();
        }
        
        private void OnTankValuesChanged(TankValues values)
        {
            if(_lifeBar) _lifeBar.CurrentValue = values.ArmorAmount / values.TotalArmor;
            if(_shieldBar) _shieldBar.CurrentValue = values.ShieldAmount / values.TotalShield;
        }
    }
}