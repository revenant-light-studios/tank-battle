using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.ForceFields
{
    public class ForceField : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("Color"), InspectorName("Color")]
        private Color _color;

        [SerializeField, FormerlySerializedAs("FlashTime"), InspectorName("Flash time")]
        private float _flashTime;

        [SerializeField, FormerlySerializedAs("FlashAlpha"), InspectorName("Flash Alpha"), Range(0f, 1f)]
        private float _flashAlpha;

        private Material _material;
        private Coroutine _materialFlashCoroutine;

        public TankManager ParentTank;

        private void Awake()
        {
            _material = GetComponent<MeshRenderer>().material;
        }

        public void ForceFieldHit()
        {
            _color.a = 0.0f;
            _material.color = _color;
            _flashAlpha = Mathf.Clamp(_flashAlpha, 0f, 1f);
            
            if(_materialFlashCoroutine != null) StopCoroutine(_materialFlashCoroutine);
            _materialFlashCoroutine = StartCoroutine(FlashEnumerator());
        }

        private IEnumerator FlashEnumerator()
        {
            Color frameColor = _material.color;
            float flashInTime = _flashTime * 0.5f;

            for (float i = 0; i < flashInTime; i += Time.deltaTime)
            {
                frameColor.a = Mathf.Lerp(0, _flashAlpha, i / flashInTime);
                _material.color = frameColor;
                yield return null;
            }
            
            for (float i = 0; i < flashInTime; i += Time.deltaTime)
            {
                frameColor.a = Mathf.Lerp(_flashAlpha, 0, i / flashInTime);
                _material.color = frameColor;
                yield return null;
            }

            _material.color = _color;
        }
    }
}