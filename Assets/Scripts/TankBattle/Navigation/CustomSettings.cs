using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankBattle.Navigation
{
    public class CustomSettings : MonoBehaviour
    {
        private KeyCode _shootBtn = KeyCode.Mouse0;

        public KeyCode shootBtn
        {
            get { return _shootBtn; }
            set { _shootBtn = value; }
        }

        private float _globalVolume = 1f;
        public float globalVolume
        {
            get { return _globalVolume; }
            set
            {
                _globalVolume = value;
                OnChangeVolume?.Invoke();
            }
        }

        private float _musicVolume = 1f;
        public float musicVolume
        {
            get { return _musicVolume; }
            set
            {
                _musicVolume = value;
                OnChangeVolume?.Invoke();
            }
        }

        private float _effectsVolume = 1f;
        public float effectsVolume
        {
            get { return _effectsVolume; }
            set
            {
                _effectsVolume = value;
                OnChangeVolume?.Invoke();
            }
        }

        public delegate void OnChangeVolumeDelegate();
        public OnChangeVolumeDelegate OnChangeVolume;
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}