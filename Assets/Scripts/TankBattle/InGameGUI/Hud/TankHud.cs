using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using ExtensionMethods;
using Networking.Utilities;
using Photon.Pun;
using Photon.Realtime;
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
        private FinishGamePanel _endPanel;
        private PauseMenu _pauseMenu;
        private TankInput _tankInput;        

        protected override void Awake()
        {
            base.Awake();
            
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
            _endPanel = transform.FirstOrDefault(t => t.name == "EndGamePanel").GetComponent<FinishGamePanel>();
            _pauseMenu = transform.FirstOrDefault(t => t.name == "PauseMenu").GetComponent<PauseMenu>();
            _endPanel.gameObject.SetActive(false);
        }

        public override void RegisterTank(TankManager tankManager)
        {
            base.RegisterTank(tankManager);
            _tankInput = tankManager.GetComponent<TankInput>();
        }

        public override void SetDeadHudState()
        {
            base.SetDeadHudState();
            _crossHair.gameObject.SetActive(false);
            _hitImage.gameObject.SetActive(false);
            _pauseMenu.gameObject.SetActive(false);
            _helpPanel.gameObject.SetActive(false);
            _lockedTankUI?.SetActive(false);
            transform.FirstOrDefault(t => t.name == "TankHud")?.gameObject.SetActive(false);
            transform.FirstOrDefault(t => t.name == "RadarTracks")?.gameObject.SetActive(false);
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

        public override void OnEnable()
        {
            base.OnEnable();
            UpdateLivingPlayersText(PhotonNetwork.CurrentRoom.GetAlivePlayersCount());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            
            int numberOfPlayersAlive = PhotonNetwork.CurrentRoom.GetAlivePlayersCount();
            UpdateLivingPlayersText(PhotonNetwork.CurrentRoom.GetAlivePlayersCount());

            if (numberOfPlayersAlive <= 1)
            {
                ShowEndPanel();
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.TryGetValue(PlayerExtensions.PlayerAlive, out object isAlive))
            {
                int numberOfPlayersAlive = PhotonNetwork.CurrentRoom.GetAlivePlayersCount(); 
                UpdateLivingPlayersText(numberOfPlayersAlive);

                if (!(bool)isAlive)
                {
                    if (numberOfPlayersAlive <= 1)
                    {
                        ShowEndPanel();
                    }
                }
            }
        }

        public override void ShowEndPanel()
        {
            SetDeadHudState();
            _tankValues.GetComponent<PhotonView>().RPC("DeactivateTank", RpcTarget.All);
            _tankInput.SwitchActionMap(TankInput.TankInputMaps.None);
            _endPanel.Show();
        }
        public override void StartViewerMode()
        {
            // Debug.Log("Viewer");
            GetComponentInChildren<TankFollowerManager>().enabled = true;
        }
        public override void TogglePauseMenu(Action CloseAction)
        {
            PauseMenu.OnResumeDelegate _closeDelegate = () => CloseAction?.Invoke();
            _pauseMenu.OnResume -= _closeDelegate;
            _pauseMenu.OnResume += _closeDelegate;
            _pauseMenu.Toggle();            
        }
        
        protected virtual void NumberOfBulletsChange(int numberOfBullets)
        {
            //Debug.Log($"Secondary weapon current number of bullets {numberOfBullets}");    
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