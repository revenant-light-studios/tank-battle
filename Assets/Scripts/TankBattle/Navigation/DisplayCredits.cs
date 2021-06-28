using System;
using System.Collections;
using ExtensionMethods;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class DisplayCredits : MonoBehaviour
    {
        private Transform _creditsContainer;
        private Vector2 _startPos;
        private Vector2 _endPos;
        private RectTransform _logo;
        private RectTransform _transform;
        private float _time = 15f;
        bool load = false;

        private IEnumerator _coroutine;

        private void Awake()
        {
            _creditsContainer = transform.FirstOrDefault(t => t.name == "CreditsContainer");
            _logo = transform.FirstOrDefault(t => t.name == "LogoSection").GetComponent<RectTransform>();
        }

        private void Start()
        {
            Invoke("InitCredits", 0.2f);
        }

        private void InitCredits()
        {
            _transform = _creditsContainer.GetComponent<RectTransform>();
            RectTransform _creditsTransform = GetComponent<RectTransform>();
            
            Vector2 canvasSize = _creditsTransform.rect.size;
            _logo.sizeDelta = canvasSize * transform.localScale;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_transform);
            
            VerticalLayoutGroup layout = _transform.GetComponent<VerticalLayoutGroup>();
            float height = layout.preferredHeight * _transform.localScale.y;
            _endPos = new Vector2(_startPos.x, height);
            
            

            // VerticalLayoutGroup layout = _transform.GetComponent<VerticalLayoutGroup>();
            // _startPos = _transform.anchoredPosition;
            // float height = layout.preferredHeight * _transform.localScale.y;
            // float windowHeight = GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect.height;
            // _endPos = new Vector2(_startPos.x, _startPos.y + height + (windowHeight * 0.5f - _logo.rect.height * 0.5f * transform.localScale.y));
            
            load = true;
            
            StartCredits();
        }

        IEnumerator Display(float time, Vector2 startPos, Vector2 endPos)
        {
            float elapsedTime = 0;
            while (elapsedTime < time)
            {
                _transform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _transform.anchoredPosition = endPos;
        }

        public void StartCredits()
        {
            if (load)
            {
                // Debug.Log($"Going from {_startPos} to {_endPos}");
                _coroutine = Display(_time, _startPos, _endPos);
                StartCoroutine(_coroutine);
            }
        }

        public void RestartPositions()
        {
            // Debug.Log($"Going from {_endPos} to {_startPos}");
            _transform.anchoredPosition = _startPos;
        }
    }
}
