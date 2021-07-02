using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankBattle.Tanks.ForceFields
{
    public class ForceField : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("FlashTime"), InspectorName("Flash time")]
        private float _flashTime;

        private Material _material;
        private Coroutine _materialFlashCoroutine;

        public TankManager ParentTank;

        private void Awake()
        {
            _material = GetComponent<MeshRenderer>().material;
        }

        public void ForceFieldHit()
        {
            if(_materialFlashCoroutine != null) StopCoroutine(_materialFlashCoroutine);
            _materialFlashCoroutine = StartCoroutine(FlashEnumerator());
        }

        private IEnumerator FlashEnumerator()
        {
            float _currentFill = _material.GetFloat("Vector1_63935E4E");
            float totalFill = 1.0f;
            float flashInTime = _flashTime * 0.5f;

            for (float i = 0; i < flashInTime; i += Time.deltaTime)
            {
                _material.SetFloat("Vector1_63935E4E", Mathf.Lerp(_currentFill, totalFill, i / flashInTime));
                yield return null;
            }
            
            for (float i = 0; i < flashInTime; i += Time.deltaTime)
            {
                _material.SetFloat("Vector1_63935E4E", Mathf.Lerp(totalFill, _currentFill, i / flashInTime));
                yield return null;
            }
        }
    }
}