using System;
using TankBattle.InGameGUI;
using TankBattle.Tanks.Bullets;
using UnityEditor;
using UnityEngine;

namespace TankBattle.Tanks.Guns
{
    public class MissileLauncher : ATankGun
    {
        private ATankBullet _missile;
        private GameObject _trackedTank;


        private void Awake()
        {
            if (!_missile)
            {
                _missile = Resources.Load<Missile>("Bullets/Missile");    
            }
        }

        public override float FiringRate
        {
            get;
        }
        
        public GameObject TrackedTank
        {
            get => _trackedTank;
            
            set
            {
                if(_trackedTank != null) _trackedTank.GetComponent<RadarTrack>().SetTrankingState(RadarTrack.LockedState.None);
                
                _trackedTank.GetComponent<RadarTrack>()?.SetTrankingState(RadarTrack.LockedState.Locked);
                _trackedTank = value;
                
            }
        }

        public override void Fire()
        {
            if (!_missile) return;
            
            ATankBullet missileInstance = Instantiate(_missile);
            missileInstance.OnBulletHit = OnBulletHit;
            missileInstance.transform.position = transform.position;
            missileInstance.transform.rotation = transform.rotation;
            missileInstance.Fire(transform);
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(MissileLauncher))]
    public class MissileLauncherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Fire"))
            {
                MissileLauncher missile = (MissileLauncher)target;
                missile.Fire();
            }
        }
    }
#endif
}