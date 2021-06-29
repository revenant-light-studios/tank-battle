using System;
using TankBattle.Navigation;
using UnityEngine;
using TankBattle.Global;

namespace TankBattle.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public enum SoundTypes
        {
            effect,
            music
        }

        public SoundTypes SoundType = SoundTypes.effect;

        private float _volume;

        private void Awake()
        {
            if (SoundType == SoundTypes.effect)
            {
                _volume = GlobalMethods.EffectsVolume;
            }
            else
            {
                _volume = GlobalMethods.MusicVolume;
            }
            Debug.Log($"V: {_volume} G: {GlobalMethods.GeneralVolume}");
            var audioSource = transform.GetComponent<AudioSource>();
            audioSource.volume = _volume * GlobalMethods.GeneralVolume;
        }
    }   
}
