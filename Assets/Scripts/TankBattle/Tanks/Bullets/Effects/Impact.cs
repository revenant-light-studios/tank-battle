using UnityEngine;

namespace TankBattle.Tanks.Bullets.Effects
{
    [RequireComponent(typeof(ParticleSystem), typeof(AudioSource))]
    public class Impact : MonoBehaviour
    {
        public enum ImpactType
        {
            Hull,
            ForceField,
            Default
        }
        
        public AudioClip HullImpact;
        public AudioClip ForceFieldImpact;

        public float Duration
        {
            get => ((_particleSystem) ? _particleSystem.main.duration : 0f);
        }
        
        private ParticleSystem _particleSystem;
        private AudioSource _audioSource;
        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _audioSource = GetComponent<AudioSource>();
        }
        
        public void Play(ImpactType impactType = ImpactType.Default)
        {
            _particleSystem.Play();
            switch (impactType)
            {
                case ImpactType.Hull:
                    _audioSource.PlayOneShot(HullImpact);
                    break;
                case ImpactType.ForceField:
                    _audioSource.PlayOneShot(ForceFieldImpact);
                    break;
                default:
                    _audioSource.PlayOneShot(HullImpact);
                    break;
            }
        }
    }
}