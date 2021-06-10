using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
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
        public RadarTrack RadarTrack { get => _radarTrack; }

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

        #region Visibility, tracking and locking states
        private bool _visible = false;
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                
                if (!_visible)
                {
                    Tracked = false;
                }
            } 
        }
        
        private bool _tracked = false;
        public bool Tracked
        {
            get => _tracked;
            set
            {
                if (value)
                {
                    ShowTrackerImage();
                }
                else
                {
                    HideTrackerImage();
                }
                
                _tracked = value;  
            } 
        }

        private bool _locked = false;
        public bool Locked
        {
            get => _locked;

            set
            {
                _locked = value;
                if(_radarTrack) _radarTrack.SetTrackingState(_locked ? RadarTrack.LockedState.Locked : RadarTrack.LockedState.None);
            }
        }
        #endregion
        
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

        private Camera _currentCamera;
        private TankValues _tankValues;

        public void InitTrackerImage(Transform canvas, Camera _camera)
        {
            _currentCamera = _camera;
            _tankValues = gameObject.GetComponent<TankValues>();
            
            if (_radarTrack == null)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Tanks/Hud/RadarTracker"), canvas);
                go.name = $"RadarTrack-{gameObject.name}";
                _radarTrack = go.GetComponent<RadarTrack>();
                _radarTrack.SetName(gameObject.name);
            }
        }
        
        public void UpdateTrackerImage()
        {
            if (_radarTrack != null && _radarTrack.gameObject.activeSelf)
            {
                _radarTrack.UpdateTrackingImage(_currentCamera, Bounds);
                
                if (_tankValues)
                {
                    _radarTrack.SetLifeValue(_tankValues.ArmorAmount / _tankValues.TotalArmor);
                    _radarTrack.SetShieldValue(_tankValues.ShieldAmount / _tankValues.TotalShield);
                }
            }
        }

        private void Update()
        {
            UpdateTrackerImage();
        }

        public void ShowTrackerImage()
        {
            _radarTrack?.gameObject.SetActive(true);
        }

        public void HideTrackerImage()
        {
            _radarTrack?.gameObject.SetActive(false);    
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);
        }
    }
}