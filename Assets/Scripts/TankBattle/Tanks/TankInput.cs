using System;
using Cinemachine;
using Photon.Pun;
using TankBattle.Global;
using TankBattle.InputManagers;
using TankBattle.Navigation;
using TankBattle.Tanks.Engines;
using TankBattle.Tanks.Turrets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.Serialization;

namespace TankBattle.Tanks
{
    public class TankInput : MonoBehaviour
    {
        public enum TankInputMaps
        {
            Player,
            PauseSystem,
            DeadPlayer
        }
        
        private PhotonView _photonView;
        private ATankEngine _engine;
        private ATankTurret _turret;
        
        // Triggers
        public Trigger Trigger1 = new Trigger();
        public Trigger Trigger2 = new Trigger();
        public Trigger LockTrigger = new Trigger();
        public Trigger PauseTrigger = new Trigger();
        public Trigger HelpTrigger = new Trigger();
        public Trigger SwitchTankTrigger = new Trigger();

        [SerializeField, FormerlySerializedAs("AxisStateX")]
        private AxisState _axisStateX;
        [SerializeField, FormerlySerializedAs("AxisStateY")]
        private AxisState _axisStateY;
        
        private PlayerInput _playerInput;

        public void InitInput()
        {
            PlayerInputManager.instance.JoinPlayer();
            
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.enabled = true;
            SwitchActionMap(TankInputMaps.Player);
            
            #if UNITY_EDITOR
            if (!GlobalMethods.IsDesktop())
            {
                _playerInput.user.UnpairDevices();
                InputUser.PerformPairingWithDevice(Gamepad.current, _playerInput.user);
                TouchSimulation.Enable();

                if (GlobalMethods.IsForceMobile())
                {
                    InputSystemUIInputModule inputModule = FindObjectOfType<InputSystemUIInputModule>();
                    if (inputModule)
                    {
                        inputModule.pointerBehavior = UIPointerBehavior.AllPointersAsIs;
                    }
                }
            }
            #endif
        }

        public void SwitchActionMap(TankInputMaps map)
        {
            _playerInput.SwitchCurrentActionMap(map.ToString());
            // Debug.Log($"Action map switched to {map.ToString()}");
            switch (map)
            {
                case TankInputMaps.Player:
                    Cursor.lockState = GlobalMethods.IsDesktop() ? CursorLockMode.Locked : CursorLockMode.None;
                    break;
                case TankInputMaps.PauseSystem:
                default:
                    Cursor.lockState = CursorLockMode.None;
                    break;
            }
        }

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            _engine = GetComponentInChildren<ATankEngine>();
            _turret = GetComponentInChildren<ATankTurret>();
        }

        public void OnMove(InputValue inputValue)
        {
            Vector2 inputVector = inputValue.Get<Vector2>();
            
            //Debug.Log($"OnMove called {inputVector}");
            if (_engine)
            {
                _engine.InputHorizontalAxis = inputVector.x;
                _engine.InputVerticalAxis = inputVector.y;
            }
        }

        public void OnTurretMove(InputValue inputValue)
        {
            Vector2 inputVector = inputValue.Get<Vector2>();
            // Debug.Log($"OnTurretMove called {inputVector}");
            _axisStateX.m_InputAxisValue = inputVector.x;
            _axisStateY.m_InputAxisValue = inputVector.y;
        }

        public void OnTrigger1(InputValue inputValue)
        {
            // Debug.Log($"Trigger1 pressed: {inputValue.isPressed}");
            Trigger1.IsPressed = inputValue.isPressed;
        }

        public void OnTrigger2(InputValue inputValue)
        {
            Trigger2.IsPressed = inputValue.isPressed;
        }

        public void OnLock(InputValue inputValue)
        {
            LockTrigger.IsPressed = inputValue.isPressed;
        }

        public void OnOpenPause(InputValue inputValue)
        {
            // For single press capture
            PauseTrigger.IsPressed = true;
            PauseTrigger.IsPressed = false;
        }

        public void OnOpenHelp(InputValue inputValue)
        {
            HelpTrigger.IsPressed = inputValue.isPressed;
        }

        public void OnSwitchTank(InputValue inputValue)
        {
            // For single press capture
            SwitchTankTrigger.IsPressed = true;
            SwitchTankTrigger.IsPressed = false;
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
        /// Turret input management
        /// </summary>
        private void TurretInput()
        {
            _axisStateY.Update(Time.fixedDeltaTime);
            _axisStateX.Update(Time.fixedDeltaTime);
        }
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