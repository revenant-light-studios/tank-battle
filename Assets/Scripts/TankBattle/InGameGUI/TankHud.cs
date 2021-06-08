using ExtensionMethods;
using TankBattle.Tanks;
using UnityEngine;

namespace TankBattle.InGameGUI
{
    public class TankHud : ATankHud
    {
        private ValueBar _lifeBar;
        private ValueBar _shieldBar;
        
        private TankValues _tankValues;
        private HitImage _hitImage;

        private void Awake()
        {
            _hitImage = transform.FirstOrDefault(t => t.name == "HitImage")?.GetComponent<HitImage>();
            _lifeBar = transform.FirstOrDefault(t => t.name == "LifeBar")?.GetComponent<ValueBar>();
            _shieldBar = transform.FirstOrDefault(t => t.name == "ShieldBar")?.GetComponent<ValueBar>();
        }
        
        public override void RegisterTank(GameObject tank)
        {
            _tankValues = tank.GetComponent<TankValues>();
            if (_tankValues != null)
            {
                _tankValues.OnValuesChanged = OnTankValuesChanged;
                _tankValues.OnTankWasHit = OnTankWasHit;
                // Debug.LogFormat("Tank {0} registered with hud", tank.name);
            }
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