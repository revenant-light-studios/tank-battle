using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TankBattle.InputManagers
{

    public class ShootBtn : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private bool _pointerDown;
        private bool _isConstantPressed;
        private float _pointerDownTimer;

        [SerializeField, FormerlySerializedAs("RequiredHoldTime")]
        private float _requiredHoldTime;

        [SerializeField, FormerlySerializedAs("RequiredHoldTime")]
        private Image _fillImage;

        public delegate void OnShootDelegate();
        public OnShootDelegate onShoot;

        public delegate void OnStopShootDelegate();
        public OnStopShootDelegate onStopShoot;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isConstantPressed)
            {
                _isConstantPressed = false;
                onStopShoot?.Invoke();
                Reset();

            }
            else
            {
                _pointerDown = true;
                onShoot?.Invoke();
            }

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isConstantPressed)
            {
                onStopShoot?.Invoke();
                Reset();
            }
            _pointerDown = false;
        }

        private void Start()
        {
            _pointerDown = false;
            _isConstantPressed = false;
    }

        // Update is called once per frame
        void Update()
        {
            if (!_isConstantPressed && _pointerDown)
            {
                _pointerDownTimer += Time.deltaTime;
                if (_pointerDownTimer > _requiredHoldTime)
                {
                    _isConstantPressed = true;
                }
                _fillImage.fillAmount = _pointerDownTimer / _requiredHoldTime;
            }
        }

        private void Reset()
        {
            _pointerDownTimer = 0;
            _fillImage.fillAmount = 0;
        }
    }
}
