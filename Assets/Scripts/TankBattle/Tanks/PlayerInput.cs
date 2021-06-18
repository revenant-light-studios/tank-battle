using Cinemachine;
using ExtensionMethods;
using Photon.Pun;
using TankBattle.Global;
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
        private VirtualButton _shootButton;
        private VirtualButton _secondaryShootButton;
        private VirtualButton _secondaryTrackButton;

        public void InitInput(VirtualJoystick movement, VirtualJoystick aim, VirtualButton shoot, VirtualButton specialShoot)
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

            if (shoot != null)
            {
                _shootButton = shoot;
            }

            if (specialShoot != null)
            {
                _secondaryShootButton = specialShoot;
            }
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

        #region Weapon triggers management
        public Trigger Trigger1 = new Trigger();
        public Trigger Trigger2 = new Trigger();
        public Trigger Trigger3 = new Trigger();

        private void GunInput()
        {
            if (GlobalMethods.IsDesktop())
            {
                Trigger1.IsPressed = Input.GetButton("Fire1");
                Trigger2.IsPressed = Input.GetButton("Fire2");
                Trigger3.IsPressed = Input.GetButton("Track");
            }
            else
            {
                Trigger1.IsPressed = _shootButton.IsPressed();
                Trigger2.IsPressed = _secondaryShootButton.IsPressed();
            }
        }
        #endregion
    }

    public class Trigger
    {
        private bool _triggerPressed;
        
        public bool IsPressed
        {
            get => _triggerPressed;
            set
            {
                if (value && !_triggerPressed)
                {
                    _triggerPressed = true;
                    OnTriggerPressed?.Invoke();    
                } else if (!value && _triggerPressed)
                {
                    _triggerPressed = false;
                    OnTriggerReleased?.Invoke();
                }
            }
        }
        
        public delegate void OnTriggerPressedDelegate();
        public event OnTriggerPressedDelegate OnTriggerPressed;

        public delegate void OnTriggerReleasedDelegate();
        public event OnTriggerReleasedDelegate OnTriggerReleased;

    }
}