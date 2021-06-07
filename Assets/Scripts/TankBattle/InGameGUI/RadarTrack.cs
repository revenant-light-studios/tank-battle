using ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.InGameGUI
{
    public class RadarTrack : MonoBehaviour
    {
        public enum LockedState
        {
            Locked,
            None
        }

        public Color LockedColor;
        public Color TrackerColor;
        
        private Canvas _parentCanvas;
        private Image _trackingImage;
        private Image _lifeBar;
        private Image _shieldBar;
        private Text _name;
        private RectTransform _rectTransform;
        private Vector2 _maxLife;
        private Vector2 _maxShield;

        private void Awake()
        {
            _parentCanvas = transform.parent.GetComponent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            _trackingImage = transform.FirstOrDefault(t => t.name == "TrackingImage")?.GetComponent<Image>();
            if (_trackingImage) _trackingImage.color = TrackerColor;
            
            _lifeBar = transform.FirstOrDefault(t => t.name == "LifeBar")?.GetComponent<Image>();
            _maxLife = _lifeBar.GetComponent<RectTransform>().sizeDelta;
            
            _shieldBar = transform.FirstOrDefault(t => t.name == "ShieldBar")?.GetComponent<Image>();
            _maxShield = _shieldBar.GetComponent<RectTransform>().sizeDelta;

            _name = transform.FirstOrDefault(t => t.name == "Name")?.GetComponent<Text>();
        }

        public void ShowTankBars(bool show)
        {
            _lifeBar.gameObject.SetActive(show);
            _shieldBar.gameObject.SetActive(show);
        }

        public void UpdateTrackingImage(Camera currentCamera, Bounds objectBounds)
        {
            Vector3[] corners = new Vector3[]
            {
                currentCamera.WorldToScreenPoint(new Vector3(objectBounds.center.x + objectBounds.extents.x, objectBounds.center.y + objectBounds.extents.y, objectBounds.center.z + objectBounds.extents.z)), 
                currentCamera.WorldToScreenPoint(new Vector3(objectBounds.center.x + objectBounds.extents.x, objectBounds.center.y - objectBounds.extents.y, objectBounds.center.z + objectBounds.extents.z)),
                currentCamera.WorldToScreenPoint(new Vector3(objectBounds.center.x + objectBounds.extents.x, objectBounds.center.y + objectBounds.extents.y, objectBounds.center.z - objectBounds.extents.z)), 
                currentCamera.WorldToScreenPoint(new Vector3(objectBounds.center.x + objectBounds.extents.x, objectBounds.center.y - objectBounds.extents.y, objectBounds.center.z - objectBounds.extents.z)),
                currentCamera.WorldToScreenPoint(new Vector3(objectBounds.center.x - objectBounds.extents.x, objectBounds.center.y + objectBounds.extents.y, objectBounds.center.z + objectBounds.extents.z)), 
                currentCamera.WorldToScreenPoint(new Vector3(objectBounds.center.x - objectBounds.extents.x, objectBounds.center.y - objectBounds.extents.y, objectBounds.center.z + objectBounds.extents.z)),
                currentCamera.WorldToScreenPoint(new Vector3(objectBounds.center.x - objectBounds.extents.x, objectBounds.center.y + objectBounds.extents.y, objectBounds.center.z - objectBounds.extents.z)), 
                currentCamera.WorldToScreenPoint(new Vector3(objectBounds.center.x - objectBounds.extents.x, objectBounds.center.y - objectBounds.extents.y, objectBounds.center.z - objectBounds.extents.z))
            }; 

            Vector3 min = Vector3.Min(corners[0], Vector3.Min(corners[1], Vector3.Min(corners[2], Vector3.Min(corners[3], Vector3.Min(corners[4], Vector3.Min(corners[5], Vector3.Min(corners[6], corners[7])))))));
            Vector3 max = Vector3.Max(corners[0], Vector3.Max(corners[1], Vector3.Max(corners[2], Vector3.Max(corners[3], Vector3.Max(corners[4], Vector3.Max(corners[5], Vector3.Max(corners[6], corners[7])))))));

            if (_rectTransform)
            {
                _rectTransform.position = min;
                _rectTransform.sizeDelta = Vector2.Max(new Vector2(32f, 32f), (max - min)) / _parentCanvas.scaleFactor;
            }
        }

        public void SetLifeValue(float value)
        {
            value = Mathf.Clamp(value, 0f, 1f);
            if(_lifeBar) _lifeBar.GetComponent<RectTransform>().sizeDelta = _maxLife * new Vector2(1f, value);
        }
        
        public void SetShieldValue(float value)
        {
            value = Mathf.Clamp(value, 0f, 1f);
            if(_shieldBar) _shieldBar.GetComponent<RectTransform>().sizeDelta = _maxShield * new Vector2(1f, value);
        }

        public void SetName(string objName)
        {
            if (_name) _name.text = objName;
        }

        public void SetTrankingState(LockedState state)
        {
            if (!_trackingImage) return;
            
            if (state == LockedState.None)
            {
                _trackingImage.color = TrackerColor;
            }
            else
            {
                _trackingImage.color = LockedColor;
            }
        }
    }
}