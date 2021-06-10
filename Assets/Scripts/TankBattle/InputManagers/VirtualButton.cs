using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TankBattle.InputManagers
{
    public class VirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private bool _pointerDown;
        private bool _isPressed;
        private float _pointerDownTimer;

        [SerializeField, FormerlySerializedAs("CanHold")]
        private bool _canHold;

        [SerializeField, FormerlySerializedAs("RequiredHoldTime")]
        private float _requiredHoldTime;

        [SerializeField, FormerlySerializedAs("FillImage")]
        private Image _fillImage;

        public bool IsPressed()
        {
            return _isPressed;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDown = true;
            _pointerDownTimer = 0f;
            _fillImage.fillAmount = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pointerDown = false;
        }

        private void Start()
        {
            _pointerDown = false;
       }

        // Update is called once per frame
        void Update()
        {
            if (_canHold)
            {
                if (_pointerDown)
                {
                    _pointerDownTimer += Time.deltaTime;
                    _fillImage.fillAmount = Mathf.Clamp(_pointerDownTimer / _requiredHoldTime, 0f, 1f);
                }

                if (_pointerDownTimer >= _requiredHoldTime)
                {
                    _isPressed = true;
                }
                else
                {
                    _isPressed = _pointerDown;
                    if (!_isPressed) _fillImage.fillAmount = 0f;
                }
            } else
            {
                _isPressed = _pointerDown;
                _fillImage.fillAmount = _isPressed ? 1f : 0f;
            }
            
            // Debug.Log($"{gameObject.name} -> Pressed: {_isPressed}");
        } 
    }
}