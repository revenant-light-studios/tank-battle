using ExtensionMethods;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TankBattle.Tanks
{
    public class PlayerInputMobile : PlayerInput
    {
        private VirtualJoystick _movementJoystick;
        private VirtualJoystick _cameraJoystick;

        private Button _shootBtn;

        public float _yAngle = 0f;
        private float _rotVel = 60f;

        private void Awake()
        {
            GameObject UI = GameObject.Find("UserUIMobile");
            _cameraJoystick = UI.transform.FirstOrDefault(t => t.name == "CameraJoystick").GetComponentInChildren<VirtualJoystick>();
            _movementJoystick = UI.transform.FirstOrDefault(t => t.name == "MovementJoystick").GetComponentInChildren<VirtualJoystick>();
            _shootBtn = UI.transform.FirstOrDefault(t => t.name == "ShootBtn").GetComponent<Button>();

            _shootBtn.onClick.AddListener(Fire);
        }

        private void FixedUpdate()
        {
            if (!_photonView.IsMine && PhotonNetwork.IsConnected) return;

            _yAngle += _cameraJoystick.InputDirection.x * _rotVel * Time.fixedDeltaTime;

            if(_yAngle < -180)
            {
                _yAngle += 360;
            }
            else if(_yAngle > 180)
            {
                _yAngle -= 360;
            }
        }

        private void LateUpdate()
        {
            _turret.UpdateTurret(_yAngle);
        }

        protected override void EngineInput()
        {
            base.EngineInput();
            _engine.InputVerticalAxis = _movementJoystick.InputDirection.y;
            _engine.InputHorizontalAxis = _movementJoystick.InputDirection.x;
            _engine.UpdateTank();
        }

        private void Fire()
        {
            if (!_fired)
            {
                _lastFired = _gun.FiringRate;
                _fired = true;
                _gun.Fire();
            }
        }

        protected override void GunInput()
        {
            base.GunInput();
            
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
