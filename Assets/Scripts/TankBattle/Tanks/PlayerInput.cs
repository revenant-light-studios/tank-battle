using Cinemachine;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.InputManagers;
using TankBattle.Tanks.Engines;
using TankBattle.Tanks.Guns;
using TankBattle.Tanks.Turrets;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks
{
    public class PlayerInput : MonoBehaviour
    {
        private PhotonView _photonView;
        private ATankGun _primaryGun;
        private ATankGun _secondaryGun;
        private ATankEngine _engine;
        private ATankTurret _turret;

        [SerializeField, FormerlySerializedAs("AxisStateX")]
        private AxisState _axisStateX;
        [SerializeField, FormerlySerializedAs("AxisStateY")]
        private AxisState _axisStateY;

        private VirtualJoystick _movementJoystick;

        public void InitInput(VirtualJoystick movement, VirtualJoystick aim)
        {
            // Aiming is controlled by cinemachine camera module
            if (aim)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                _axisStateX.m_InputAxisName = "";
                _axisStateY.m_InputAxisName = "";
                _axisStateX.SetInputAxisProvider(0, aim);
                _axisStateY.SetInputAxisProvider(1, aim);
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                _axisStateX.m_InputAxisName = "Mouse X";
                _axisStateY.m_InputAxisName = "Mouse Y";
            }

            _movementJoystick = movement;
        }

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            _engine = GetComponentInChildren<ATankEngine>();
            _turret = GetComponentInChildren<ATankTurret>();

            TankManager tankManager = GetComponent<TankManager>();
            if (tankManager)
            {
                tankManager.OnTankWeaponEnabled += (gun, weapon) =>
                {
                    if (weapon == TankManager.TankWeapon.Primary)
                    {
                        _primaryGun = gun;
                    } else if (weapon == TankManager.TankWeapon.Secondary)
                    {
                        _secondaryGun = gun;
                    }
                };
            }
        }

        private void Update()
        {
            if (!_photonView.IsMine && PhotonNetwork.IsConnected) return;
            EngineInput();
            GunInput();
        }

        private void LateUpdate()
        {
            _turret.UpdateTurret(new Vector3(_axisStateY.Value, _axisStateX.Value, 0));
        }

        private void FixedUpdate()
        {
            TurretInput();
        }

        /// <summary>
        /// Engine input management
        /// </summary>
        private void EngineInput()
        {
            if (_movementJoystick)
            {
                _engine.InputVerticalAxis = _movementJoystick.GetAxisValue(1);
                _engine.InputHorizontalAxis = _movementJoystick.GetAxisValue(0);
            }
            else
            {
                _engine.InputVerticalAxis = Input.GetAxis("Vertical");
                _engine.InputHorizontalAxis = Input.GetAxis("Horizontal");
            }
            _engine.UpdateTank();
        }

        /// <summary>
        /// Turret input management
        /// </summary>
        private void TurretInput()
        {
            _axisStateY.Update(Time.fixedDeltaTime);
            _axisStateX.Update(Time.fixedDeltaTime);
        }

        /// <summary>
        /// Gun input management
        /// </summary>
        private void GunInput()
        {
            if (Input.GetButton("Fire1") && _primaryGun)
            {
                _primaryGun.Fire();
            }

            if (Input.GetButton("Fire2") && _secondaryGun)
            {
                if (_secondaryGun && _secondaryGun.isActiveAndEnabled)
                {
                    _secondaryGun.Fire();
                }
            }
        }
    }
}