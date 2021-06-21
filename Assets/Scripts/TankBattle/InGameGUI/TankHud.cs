using ExtensionMethods;
using TankBattle.Tanks;
using TankBattle.Tanks.Guns;
using UnityEngine;

namespace TankBattle.InGameGUI
{
    public class TankHud : ATankHud
    {
        protected ValueBar _lifeBar;
        protected ValueBar _shieldBar;
        protected CrossHair _crossHair;
        protected HitImage _hitImage;
        protected GameObject _helpPanel;

        
        
        protected virtual void Awake()
        {
            _crossHair = transform.FirstOrDefault(t => t.name == "Crosshair").GetComponent<CrossHair>();
            _hitImage = transform.FirstOrDefault(t => t.name == "HitImage")?.GetComponent<HitImage>();
            _lifeBar = transform.FirstOrDefault(t => t.name == "LifeBar")?.GetComponent<ValueBar>();
            _shieldBar = transform.FirstOrDefault(t => t.name == "ShieldBar")?.GetComponent<ValueBar>();
            _helpPanel = transform.FirstOrDefault(t => t.name == "HelpPanel")?.gameObject;
            _helpPanel.SetActive(false);
        }

        protected override void OnTankWeaponEnabled(ATankGun gun, TankManager.TankWeapon weapon)
        {
            if (gun != null)
            {
                if (weapon == TankManager.TankWeapon.Primary)
                {
                    gun.OnEnergyUpdate += _crossHair.UpdateEnergy;
                } else if (weapon == TankManager.TankWeapon.Secondary)
                {
                    gun.OnNumberOfBulletsChange += NumberOfBulletsChange;
                }
            }
        }

        protected virtual void NumberOfBulletsChange(int numberOfBullets)
        {
            Debug.Log($"Secondary weapon current number of bullets {numberOfBullets}");    
        }

        protected override void OnTurretMove(Vector3 hitPoint)
        {
            Vector3 position = Camera.main.WorldToScreenPoint(hitPoint);
            _crossHair.UpdatePosition(position);
        }

        protected override void OnTankWasHit(TankValues values)
        { 
            _hitImage?.HitFlash();
        }

        protected override void OnTankValuesChanged(TankValues values)
        {
            if(_lifeBar) _lifeBar.CurrentValue = values.ArmorAmount / values.TotalArmor;
            if(_shieldBar) _shieldBar.CurrentValue = values.ShieldAmount / values.TotalShield;
        }

        public override void ShowOrHideHelp()
        {
            _helpPanel.SetActive(!_helpPanel.activeSelf);
        }

    }
}