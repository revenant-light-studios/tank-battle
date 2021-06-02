using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.InGameGUI
{
    public class ValueBar : MonoBehaviour
    {
        private Vector2 _startSize;
        private RectTransform _rectTransform;
    
        [SerializeField,
         FormerlySerializedAs("CurrentValue"),
         Range(0f, 1f)]
        private float _currentValue;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _startSize = _rectTransform.sizeDelta;
        }

        private void Reset()
        {
            _rectTransform = GetComponent<RectTransform>();
            _startSize = _rectTransform.sizeDelta;
            _currentValue = 1f;
        }

        public float CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                UpdateBar();
            }
        }
        
        // Start is called before the first frame update
        void Start()
        {
            _currentValue = 1f;
        }

        /// <summary>
        /// To make it work on editor
        /// </summary>
        private void OnValidate()
        {
            UpdateBar();
        }

        private void UpdateBar()
        {
            if (_rectTransform != null)
            {
                // Debug.LogFormat("Value bar {0} value set to {1}", name, _currentValue);
                _rectTransform.sizeDelta = _startSize * new Vector2(1f, _currentValue);    
            }
        }
    }
}
