using Cinemachine;
using Photon.Pun;
using TankBattle.Global;
using TankBattle.InputManagers;
using TankBattle.Tanks.Engines;
using TankBattle.Tanks.Turrets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Users;
using UnityEngine.Serialization;

namespace TankBattle.Tanks
{
    public class TankInput : MonoBehaviour
    {
        private PhotonView _photonView;
        private ATankEngine _engine;
        private ATankTurret _turret;
        
        // Triggers
        public Trigger Trigger1 = new Trigger();
        public Trigger Trigger2 = new Trigger();
        public Trigger LockTrigger = new Trigger();

        [SerializeField, FormerlySerializedAs("AxisStateX")]
        private AxisState _axisStateX;
        [SerializeField, FormerlySerializedAs("AxisStateY")]
        private AxisState _axisStateY;
        
        public void InitInput()
        {
            PlayerInput playerInput = GetComponent<PlayerInput>();
            playerInput.enabled = true;
            PlayerInputManager.instance.JoinPlayer();
            Cursor.lockState = GlobalMethods.IsDesktop() ? CursorLockMode.Locked : CursorLockMode.None;
            
            #if UNITY_EDITOR
            if (!GlobalMethods.IsDesktop())
            {
                playerInput.user.UnpairDevices();
                InputUser.PerformPairingWithDevice(Gamepad.current, playerInput.user);
                TouchSimulation.Enable();
            }
            #endif
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