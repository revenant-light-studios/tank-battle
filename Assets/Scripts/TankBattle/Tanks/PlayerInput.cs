using Photon.Pun;
using TankBattle.Tanks.Engines;
using TankBattle.Tanks.Guns;
using TankBattle.Tanks.Turrets;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks
{
    public class PlayerInput : MonoBehaviour
    {
        protected PhotonView _photonView;
        protected ATankEngine _engine;
        protected ATankGun _gun;
        //private ATankTurret _turret;
        protected TurretMovement _turret;

        protected float _lastFired = 0f;
        protected bool _fired = false;

        protected void Start()
        {
            _photonView = GetComponent<PhotonView>();
            _gun = GetComponentInChildren<ATankGun>();
            _engine = GetComponentInChildren<ATankEngine>();
            _turret = GetComponentInChildren<TurretMovement>();
        }

        protected void Update()
        {
            if (!_photonView.IsMine && PhotonNetwork.IsConnected) return;
            
            EngineInput();
            //TurretInput();
            GunInput();
        }

        /// <summary>
        /// Engine input management
        /// </summary>
        protected virtual void EngineInput()
        {

        }

        /// <summary>
        /// Turret input management
        /// </summary>
        /*private void TurretInput()
        {
            _turret.MousePosition = Input.mousePosition;
            _turret.UpdateTurret();
        }*/

        /// <summary>
        /// Gun input management
        /// </summary>
        protected virtual void GunInput()
        {
            
        }
    }
}