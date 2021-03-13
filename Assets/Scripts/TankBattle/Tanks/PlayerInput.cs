using Photon.Pun;
using TankBattle.Tanks.Engines;
using TankBattle.Tanks.Guns;
using TankBattle.Tanks.Turrets;
using UnityEngine;

namespace TankBattle.Tanks
{
    public class PlayerInput : MonoBehaviour
    {
        private PhotonView _photonView;
        private ATankEngine _engine;
        private ATankGun _gun;
        private ATankTurret _turret;

        private float _lastFired = 0f;
        private bool _fired = false;

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            _gun = GetComponentInChildren<ATankGun>();
            _engine = GetComponentInChildren<ATankEngine>();
            _turret = GetComponentInChildren<ATankTurret>();
        }

        private void Update()
        {
            if (!_photonView.IsMine && PhotonNetwork.IsConnected) return;
            
            EngineInput();
            TurretInput();
            GunInput();
        }

        /// <summary>
        /// Engine input management
        /// </summary>
        private void EngineInput()
        {
            _engine.InputVerticalAxis = Input.GetAxis("Vertical");
            _engine.InputHorizontalAxis = Input.GetAxis("Horizontal");
            _engine.UpdateTank();
        }

        /// <summary>
        /// Turret input management
        /// </summary>
        private void TurretInput()
        {
            _turret.MousePosition = Input.mousePosition;
            _turret.UpdateTurret();
        }

        /// <summary>
        /// Gun input management
        /// </summary>
        private void GunInput()
        {
            if (Input.GetButton("Fire1"))
            {
                if(!_fired)
                {
                    _lastFired = _gun.FiringRate;
                    _fired = true;
                    _gun.Fire();
                }
            }
            
            if (_fired)
            {
                _lastFired -= Time.deltaTime;
                
                if (_lastFired <= 0f)
                {
                    _fired = false;
                }
            }
        }
    }
}