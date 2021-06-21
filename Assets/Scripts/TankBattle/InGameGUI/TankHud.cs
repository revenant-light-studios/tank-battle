using ExtensionMethods;
using TankBattle.Tanks;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.InGameGUI
{
    public class TankHud : ATankHud
    {
        private ValueBar _lifeBar;
        private ValueBar _shieldBar;
        private CrossHair _crossHair;
        private Text _livingPlayersText;
        private FinishGame _endPanel;
        
        private TankValues _tankValues;
        private HitImage _hitImage;

        private void Awake()
        {
            _crossHair = transform.FirstOrDefault(t => t.name == "Crosshair").GetComponent<CrossHair>();
            _hitImage = transform.FirstOrDefault(t => t.name == "HitImage")?.GetComponent<HitImage>();
            _lifeBar = transform.FirstOrDefault(t => t.name == "LifeBar")?.GetComponent<ValueBar>();
            _shieldBar = transform.FirstOrDefault(t => t.name == "ShieldBar")?.GetComponent<ValueBar>();
            _livingPlayersText = transform.FirstOrDefault(t => t.name == "LivingPlayersText").GetComponent<Text>();
            _endPanel = transform.FirstOrDefault(t => t.name == "EndGamePanel").GetComponent<FinishGame>();
            _endPanel.gameObject.SetActive(false);

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

        public override void UpdateLivingPlayersText(int livingPlayers)
        {
            _livingPlayersText.text = $"{livingPlayers}";
        }


        public override void StartViewerMode()
        {
            Debug.Log("Viewer");
            GetComponent<TankViewerManager>().enabled = true;
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

        public override void ShowEndPanel(TankManager tankManager)
        {
            _endPanel.gameObject.SetActive(true);
            _endPanel.InitEndPanel(tankManager);
        }
    }
}