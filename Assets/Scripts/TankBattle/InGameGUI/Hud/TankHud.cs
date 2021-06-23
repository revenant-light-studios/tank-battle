using ExtensionMethods;
using TankBattle.Global;
using TankBattle.InGameGUI.LockedTank;
using TankBattle.Tanks;
using TankBattle.Tanks.Guns;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.InGameGUI.Hud
{
    public class TankHud : ATankHud
    {
        protected ValueBar _lifeBar;
        protected ValueBar _shieldBar;
        protected CrossHair _crossHair;
        protected HitImage _hitImage;
        protected LockedTankUI _lockedTankUI;
        private Text _livingPlayersText;
        private FinishGame _endPanel;
        private GameObject _pauseMenu;

        
        protected virtual void Awake()
        {
            _crossHair = transform.FirstOrDefault(t => t.name == "Crosshair").GetComponent<CrossHair>();
            _hitImage = transform.FirstOrDefault(t => t.name == "HitImage")?.GetComponent<HitImage>();
            _lifeBar = transform.FirstOrDefault(t => t.name == "LifeBar")?.GetComponent<ValueBar>();
            _shieldBar = transform.FirstOrDefault(t => t.name == "ShieldBar")?.GetComponent<ValueBar>();
            _lockedTankUI = transform.FirstOrDefault(t => t.name == "LockedTankUI")?.GetComponent<LockedTankUI>();
            if (_lockedTankUI)
            {
                _lockedTankUI.SetActive(false);
            }
            
            _livingPlayersText = transform.FirstOrDefault(t => t.name == "LivingPlayersText").GetComponent<Text>();
            _endPanel = transform.FirstOrDefault(t => t.name == "EndGamePanel").GetComponent<FinishGame>();
            _pauseMenu = transform.FirstOrDefault(t => t.name == "PauseMenu").gameObject;
            _endPanel.gameObject.SetActive(false);
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
        public override void UpdateLivingPlayersText(int LivingPlayers)
        {
            _livingPlayersText.text = $"{LivingPlayers}";
        }
        public override void ShowEndPanel(TankManager tankManager)
        {
            _endPanel.gameObject.SetActive(true);
            _endPanel.InitEndPanel(tankManager);
        }
        public override void StartViewerMode()
        {
            // Debug.Log("Viewer");
            GetComponentInChildren<TankViewerManager>().enabled = true;
        }
        public override void OpenPauseMenu()
        {
            _pauseMenu.SetActive(!_pauseMenu.activeSelf);
            if (_pauseMenu.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        protected virtual void NumberOfBulletsChange(int numberOfBullets)
        {
            // Debug.Log($"Secondary weapon current number of bullets {numberOfBullets}");    
        }

        protected override void OnTurretMove(Vector3 hitPoint)
        {
            Vector3 position = Camera.main.WorldToScreenPoint(hitPoint);
            _crossHair.UpdatePosition(position);
        }

        protected override void OnLockedTankChange(DetectableObject trackedtank)
        {
            if (!_lockedTankUI) return;

            if (trackedtank != null)
            {
                _lockedTankUI.TankValues = trackedtank.GetComponent<TankValues>();
            }
            else
            {
                _lockedTankUI.TankValues = null;
            }
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
    }
}