using System;
using TankBattle.Navigation;
using UnityEngine;
using TankBattle.Global;

namespace TankBattle.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public enum SoundTypes
        {
            Effects,
            Music,
            Global
        }

        public SoundTypes SoundType = SoundTypes.Effects;
        private AudioSource _audioSource;

        private float _volume;

        private void Awake()
        {
            _audioSource = transform.GetComponent<AudioSource>();
            SetVolume();
            
            GlobalMethods.OnVolumeChanged -= OnVolumeChanged;
            GlobalMethods.OnVolumeChanged += OnVolumeChanged;
        }
        
        private void OnDestroy()
        {
            GlobalMethods.OnVolumeChanged -= OnVolumeChanged;
        }
        private void OnVolumeChanged(SoundTypes type)
        {
            if (type == SoundType || type == SoundTypes.Global)
            {
                SetVolume();
            }
        }

        private void SetVolume()
        {
            float volume = SoundType == SoundTypes.Effects ? GlobalMethods.EffectsVolume : GlobalMethods.MusicVolume;
            _audioSource.volume = volume * GlobalMethods.GeneralVolume;
        }
    }   
}
