using UnityEngine;

namespace TankBattle.Tanks
{
    public class CrossHair : MonoBehaviour
    {
        public Sprite SearchingSprite;
        public Sprite AimSprite; 
            
        private Material _crossHairMaterial;
        private bool _targetReached;

        public bool TargetReached
        {
            get => _targetReached;
            set
            {
                _targetReached = value;

                if (_targetReached)
                {
                    if (AimSprite)
                    {
                        _crossHairMaterial.SetTexture("_MainTex", AimSprite.texture);
                    }
                }
                else
                {
                    if (SearchingSprite)
                    {
                        _crossHairMaterial.SetTexture("_MainTex", SearchingSprite.texture);
                    }
                }
            }
        }
        
        private void Start()
        {
            _crossHairMaterial = GetComponent<MeshRenderer>().material;
            
            if (SearchingSprite == null)
            {
                SearchingSprite = Resources.Load<Sprite>("Sprites/CrosshairSearch");
            }

            if (AimSprite == null)
            {
                AimSprite = Resources.Load<Sprite>("Sprites/CrosshairAim");
            }
        }
    }
}