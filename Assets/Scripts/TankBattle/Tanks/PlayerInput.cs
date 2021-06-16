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

        /// <summary>
        /// Gun input management
        /// </summary>
        // private void GunInput()
        // {
        //     if (GlobalMethods.IsDesktop())
        //     {
        //         if (Input.GetButton("Fire1") && _primaryGun)
        //         {
        //             _primaryGun.Fire();
        //         }
        //
        //         if (Input.GetButton("Fire2") && _secondaryGun)
        //         {
        //             if (_secondaryGun && _secondaryGun.isActiveAndEnabled)
        //             {
        //                 _secondaryGun.Fire();
        //             }
        //         }
        //         
        //     }
        //     else
        //     {
        //         if (_shootButton.IsPressed())
        //         {
        //             _primaryGun.Fire();
        //         }
        //
        //         if (_secondaryShootButton.IsPressed())
        //         {
        //             _secondaryGun.Fire();
        //         }
        //     }
        // }

        #region Weapon triggers management
        private bool _primaryTriggerPressed;
        private bool _secondaryTriggerPressed;

        public delegate void OnTrigger1PressedDelegate();
        public event OnTrigger1PressedDelegate OnTrigger1Pressed;

        public delegate void OnTrigger1ReleasedDelegate();
        public event OnTrigger1ReleasedDelegate OnTrigger1Released;

        public bool Trigger1
        {
            get => _primaryTriggerPressed;
            set
            {
                if (value && !_primaryTriggerPressed)
                {
                    _primaryTriggerPressed = value;
                    OnTrigger1Pressed?.Invoke();
                } else if (!value && _primaryTriggerPressed)
                {
                    _primaryTriggerPressed = value;
                    OnTrigger1Released?.Invoke();
                }
                
            }
        }

        public delegate void OnTrigger2PressedDelegate();
        public event OnTrigger2PressedDelegate OnTrigger2Pressed;

        public delegate void OnTrigger2ReleasedDelegate();
        public event OnTrigger2ReleasedDelegate OnTrigger2Released;

        public bool Trigger2
        {
            get => _secondaryTriggerPressed;

            set
            {
                if (value && !_secondaryTriggerPressed)
                {
                    _secondaryTriggerPressed = value;
                    OnTrigger2Pressed?.Invoke();
                } else if (!value && _secondaryTriggerPressed)
                {
                    _secondaryTriggerPressed = value;
                    OnTrigger2Released?.Invoke();
                }
                
            }
        }

        private void GunInput()
        {
            if (GlobalMethods.IsDesktop())
            {
                Trigger1 = Input.GetButton("Fire1");
                Trigger2 = Input.GetButton("Fire2");
            }
            else
            {
                Trigger1 = _shootButton.IsPressed();
                Trigger2 = _secondaryShootButton.IsPressed();
            }
        }
        #endregion
    }
}