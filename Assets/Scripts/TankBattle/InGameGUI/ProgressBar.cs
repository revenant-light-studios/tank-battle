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
        private int _minValue;
        
        [SerializeField, FormerlySerializedAs("MaxValue"), InspectorName("Maximum value")]
        private int _maxValue;
        
        [SerializeField, FormerlySerializedAs("Value"), InspectorName("Current value")]
        private int _value;

        [SerializeField, FormerlySerializedAs("ShowValue"), InspectorName("Show value")]
        private bool _showValue;

        private Text _valueText;
        private Image _mask;
        
        public int MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        public int MaxValue
        {
            get => _maxValue;
            set => _minValue = value;
        }

        public int Value
        {
            get => _value;
            set => _value = Math.Min(Math.Max(value, _minValue), _maxValue);
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
            _value = Math.Min(Math.Max(_value, _minValue), _maxValue);
            
            float current = _value - _minValue;
            float range = _maxValue - _minValue;
            float fillAmount = current / range;
            _mask.fillAmount = fillAmount;

            if (_valueText)
            {
                _valueText.text = _showValue ? $"{fillAmount * 100:##0}%" : "";
            }
        }
    }
}
