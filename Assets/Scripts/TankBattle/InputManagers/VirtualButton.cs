using System;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TankBattle.InputManagers
{
    public class VirtualButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler
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

        private Text _text;
        public string Text
        {
            get => _text.text;
            set => _text.text = value;
        }

        public Sprite Icon
        {
            get => GetComponent<Image>()?.sprite;
            set => GetComponent<Image>().sprite = value;
        }

        private void Awake()
        {
            _text = transform.FirstOrDefault(t => t.name == "Text")?.GetComponent<Text>();
        }

        public bool IsPressed()
        {
            return _isPressed;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Debug.Log($"Pointer down");
            _pointerDown = true;
            _pointerDownTimer = 0f;
            _fillImage.fillAmount = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Debug.Log($"Pointer up");
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
            
            SendValueToControl(_isPressed ? 1.0f : 0.0f);
        }
        
        [InputControl(layout = "Button")]
        [SerializeField]
        private string m_ControlPath;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
    }
}