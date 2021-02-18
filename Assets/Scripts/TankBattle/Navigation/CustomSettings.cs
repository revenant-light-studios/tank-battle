using UnityEngine;

namespace TankBattle.Navigation
{
    public class CustomSettings : MonoBehaviour
    {
        private float _globalVolume = 1;
        public float globalVolume 
        {
            get { return _globalVolume; }
            set
            {
                _globalVolume = value;
                OnChangeVolume?.Invoke();
            }
        }
        private float _musicVolume = 0.3f;
        public float musicVolume
        {
            get { return _musicVolume; }
            set
            {
                _musicVolume = value;
                OnChangeVolume?.Invoke();
            }
        }
        private float _effectsVolume = 1;
        public float effectsVolume
        {
            get { return _effectsVolume; }
            set
            {
                _effectsVolume = value;
                OnChangeVolume?.Invoke();
            }
        }

        public string nickname = "User";
        public KeyCode stats = KeyCode.E;
        public KeyCode photo = KeyCode.R;
        public KeyCode finishTurn = KeyCode.T;
        public bool isDaltonic = false;

        public delegate void OnChangeVolumeDelegate();
        public OnChangeVolumeDelegate OnChangeVolume;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }


    }
}

