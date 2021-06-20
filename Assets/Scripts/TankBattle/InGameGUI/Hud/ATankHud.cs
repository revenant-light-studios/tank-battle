using TankBattle.Tanks;
using TankBattle.Tanks.Guns;
using UnityEngine;

namespace TankBattle.InGameGUI.Hud
{
    public abstract class ATankHud : MonoBehaviour
    {
        protected TankValues _tankValues;
        
        public virtual void RegisterTank(TankManager tankManager)
        {
            _tankValues = tankManager.GetComponent<TankValues>();
            if (_tankValues != null)
            {
                _tankValues.OnValuesChanged = OnTankValuesChanged;
                _tankValues.OnTankWasHit = OnTankWasHit;
            }

            if (tankManager.Turret)
            {
                tankManager.Turret.OnTurretMove += OnTurretMove;
            }

            tankManager.OnTankWeaponEnabled += OnTankWeaponEnabled;
        }

        protected abstract void OnTankWasHit(TankValues values);
        protected abstract void OnTankValuesChanged(TankValues values);
        protected abstract void OnTurretMove(Vector3 position);
        protected abstract void OnTankWeaponEnabled(ATankGun gun, TankManager.TankWeapon weapon);
    }
}