using System;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TankBattle.InGameGUI
{
    [ExecuteInEditMode]
    public class ProgressBar : MonoBehaviour
    {
        #if UNITY_EDITOR
        [MenuItem("GameObject/UI/Linear Progress Bar")]
        public static void AddLinearProgressBar()
        {
            GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Components/Linear Progress Bar"), Selection.activeGameObject.transform);
        }
        #endif
        
        [SerializeField, FormerlySerializedAs("MinValue"), InspectorName("Minimum value")]
        private float _minValue = 0;
        
        [SerializeField, FormerlySerializedAs("MaxValue"), InspectorName("Maximum value")]
        private float _maxValue = 1;
        
        [SerializeField, FormerlySerializedAs("Value"), InspectorName("Current value")]
        private float _value = 0;

        [SerializeField, FormerlySerializedAs("ShowValue"), InspectorName("Show value")]
        private bool _showValue;

        private Text _valueText;
        private Image _mask;
        
        public float MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        public float MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public float Value
        {
            get => _value;
            set => _value = Mathf.Clamp(value, _minValue, _maxValue);
        }

        public bool ShowValue
        {
            get => _showValue;
            set => _showValue = value;
        }

        private void Update()
        {
            CalculateFill();
        }

        private void OnEnable()
        {
            _mask = transform.FirstOrDefault(t => t.name == "Mask").GetComponent<Image>();
            _valueText = transform.FirstOrDefault(t => t.name == "Text").GetComponent<Text>();
        }

        private void CalculateFill()
        {
            _value = Mathf.Clamp(_value, _minValue, _maxValue);
            
            float current = _value - _minValue;
            float range = Mathf.Max(0.00001f ,_maxValue - _minValue);
            float fillAmount = current / range;
            
            _mask.fillAmount = fillAmount;

            if (_valueText)
            {
                _valueText.text = (_showValue ? $"{fillAmount * 100,3:##0}%" : "").Replace(' ', (char)160);
            }
        }
    }
}
