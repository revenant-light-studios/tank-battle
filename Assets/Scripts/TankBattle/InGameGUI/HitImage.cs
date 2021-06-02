using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TankBattle.InGameGUI
{
    [ExecuteInEditMode]
    public class HitImage : MonoBehaviour
    {
        private Coroutine _hitCoroutine;
        private Image _image;
        private Camera _camera;

        [SerializeField, FormerlySerializedAs("FlashSeconds"), InspectorName("Flash Seconds")] 
        private float _flashSeconds;

        [SerializeField, FormerlySerializedAs("FlashAlpha"), InspectorName("Alpha")]
        private float _alpha;

        [SerializeField, FormerlySerializedAs("FlashColor"), InspectorName("Color")]
        private Color _color;
        
        [SerializeField, FormerlySerializedAs("CameraShakeMagnitude"), InspectorName("Camera shake magnitude")]
        float _cameraShakeMagnitude = 2f;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _camera = Camera.main;
        }

        public void HitFlash()
        {
            HitFlash(_flashSeconds, _alpha, _color);
        }
        
        public void HitFlash(float seconds, float alpha, Color color)
        {
            if (_image)
            {
                Color startColor = _color;
                startColor.a = 0f;
                _image.color = startColor;

                alpha = Mathf.Clamp(alpha, 0f, 1f);

                if (_hitCoroutine != null)
                {
                    StopCoroutine(_hitCoroutine);
                }

                _hitCoroutine = StartCoroutine(FlashCoroutine(seconds, alpha));
            }            
        }

        private IEnumerator FlashCoroutine(float seconds, float alpha)
        {
            Vector3 cameraStart = _camera.transform.localPosition;

            float inDuration = seconds * 0.5f;
            Color frameColor = _image.color;
            
            for (float i = 0; i < inDuration; i += Time.deltaTime)
            {
                frameColor.a = Mathf.Lerp(0f, alpha, i / inDuration);
                _image.color = frameColor;

                float x = Random.Range(-1f, 1f) * _cameraShakeMagnitude;
                float y = Random.Range(-1f, 1f) * _cameraShakeMagnitude;
                _camera.transform.localPosition = new Vector3(x, y, 0);
                yield return null;
            }

            for (float i = 0; i < inDuration; i += Time.deltaTime)
            {
                frameColor.a = Mathf.Lerp(alpha, 0f, i / inDuration);
                _image.color = frameColor;
                
                float x = Random.Range(-1f, 1f) * _cameraShakeMagnitude;
                float y = Random.Range(-1f, 1f) * _cameraShakeMagnitude;
                _camera.transform.localPosition = new Vector3(x, y, 0);
                yield return null;
            }

            frameColor.a = 0f;
            _image.color = frameColor;
            _camera.transform.localPosition = cameraStart;
        }
    }
}