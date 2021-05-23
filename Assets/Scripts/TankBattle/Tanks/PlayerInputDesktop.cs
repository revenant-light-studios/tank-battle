using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;


namespace TankBattle.Tanks
{
    public class PlayerInputDesktop : PlayerInput
    {
        [SerializeField, FormerlySerializedAs("XAxis")]
        Cinemachine.AxisState _xAxis;

        private void FixedUpdate()
        {
            if (!_photonView.IsMine && PhotonNetwork.IsConnected) return;

            _xAxis.Update(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            _turret.UpdateTurret(_xAxis.Value);
        }

        protected override void EngineInput()
        {
            base.EngineInput();
            _engine.InputVerticalAxis = Input.GetAxis("Vertical");
            _engine.InputHorizontalAxis = Input.GetAxis("Horizontal");
            _engine.UpdateTank();
        }

        protected override void GunInput()
        {
            base.GunInput();
            if (Input.GetButton("Fire1"))
            {
                if (!_fired)
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
