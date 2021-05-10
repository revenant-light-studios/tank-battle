using System.Collections.Generic;
using TankBattle.InGameGUI;
using TankBattle.Tanks;
using UnityEngine;

namespace TankBattle.Global
{
    public class DetectableObject : MonoBehaviour
    {
        private const string TANK_HULL_TAG = "TankHULL";
        private List<Renderer> _visibleRenderers = new List<Renderer>();
        private RadarTrack _radarTrack;

        public Bounds Bounds
        {
            get
            {
                if (_visibleRenderers.Count == 0) return new Bounds();
                
                Bounds bounds = _visibleRenderers[0].bounds;
            
                for (int i = 1; i < _visibleRenderers.Count; i++)
                {
                    bounds.Encapsulate(_visibleRenderers[i].bounds);
                }

                return bounds;
            }
        }
        
        public bool IsVisibleInCamera = false;
        
        private void Start()
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                if (renderer.CompareTag(TANK_HULL_TAG))
                {
                    _visibleRenderers.Add(renderer);    
                }
            }
        }

        private void OnDestroy()
        {
            Radar.Instance.RemoveDetectableObject(this);
            if (_radarTrack != null)
            {
                Destroy(_radarTrack);
            }
        }

        public void ShowTrackerImage(Transform canvas)
        {
            if (_radarTrack == null)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Tanks/Hud/RadarTracker"), canvas);
                _radarTrack = go.GetComponent<RadarTrack>();
                _radarTrack.SetName(gameObject.name);
            }
            
            _radarTrack.gameObject.SetActive(true);
        }

        public void HideTrackerImage()
        {
            _radarTrack?.gameObject.SetActive(false);    
        }
        
        public void UpdateTrackerImage(Camera currentCamera)
        {
            if (_radarTrack != null && _radarTrack.gameObject.activeSelf)
            {
                _radarTrack.UpdateTrackingImage(currentCamera, Bounds);
                TankValues _values = GetComponent<TankValues>();
                if (_values)
                {
                    _radarTrack.SetLifeValue(_values.ArmorAmount / _values.TotalArmor);
                    _radarTrack.SetShieldValue(_values.ShieldAmount / _values.TotalShield);
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);
        }
    }
}