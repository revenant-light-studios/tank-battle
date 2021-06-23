using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TankBattle.InputManagers
{
    public class VirtualJoystick : OnScreenControl, IDragHandler, IPointerUpHandler, IPointerDownHandler, AxisState.IInputAxisProvider
    {

        [SerializeField, FormerlySerializedAs("Container")]
        private Image _container;
        [SerializeField, FormerlySerializedAs("Joystick")]
        private Image _joystick;

        private float _radious = 0;
        private Vector2 _inputDirection = Vector2.zero;

        private void Start()
        {
            _radious = _container.rectTransform.rect.width * 0.5f;
            _container.gameObject.SetActive(false);
            _joystick.gameObject.SetActive(false);
        }

        public void OnDrag(PointerEventData ped)
        {
            var containerPos = new Vector2(_container.transform.position.x, _container.transform.position.y);

            var position = ped.position - containerPos;

            var posX = Mathf.Clamp(position.x, -_radious, _radious);
            var posY = Mathf.Clamp(position.y, -_radious, _radious);

            position = new Vector2(posX, posY);
            position = (position.magnitude > _radious) ? position.normalized * _radious : position;

            _joystick.transform.position = position + containerPos;
            _inputDirection = position/_radious;
        }

        private void Update()
        {
            SendValueToControl(_inputDirection);
        }

        public void OnPointerDown(PointerEventData ped)
        {
            _container.transform.position = ped.position;
            _container.gameObject.SetActive(true);
            _joystick.transform.position = ped.position;
            _joystick.gameObject.SetActive(true);
        }

        public void OnPointerUp(PointerEventData ped)
        {
            _inputDirection = Vector2.zero;
            _container.gameObject.SetActive(false);
            _joystick.gameObject.SetActive(false);
        }

        public float GetAxisValue(int axis)
        {
            if (axis == 0)
            {
                return _inputDirection.x;
            }

            if (axis == 1)
            {
                return _inputDirection.y;
            }

            return 0;
        }
        
        #region OnScreenControl
        [InputControl(layout = "Vector2")]
        [SerializeField]
        private string m_ControlPath;
        
        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
        #endregion
    }
}