using ExtensionMethods;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TankBattle.InGameGUI
{
    public class CrossHair : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("CrosshairColor")] 
        private Color _crossHairColor;
        
        [SerializeField, FormerlySerializedAs("CrosshairDisabledColor")]
        private Color _crosshairDisabledColor;
        
        private Image _crossHair;
        private Image _energyBar;


        private void Awake()
        {
            _crossHair = transform.FirstOrDefault(t => t.name == "CrosshairBase").GetComponent<Image>();
            _energyBar = transform.FirstOrDefault(t => t.name == "EnergyBar").GetComponent<Image>();
        }

        public void UpdatePosition(Vector3 position)
        {
            transform.position = position;
        }

        public void UpdateEnergy(float energy, float neededEnergy)
        {
            _energyBar.fillAmount = energy < neededEnergy ? 0.0f : energy;

            // Debug.Log($"Energy: {energy}, minimum: {neededEnergy}");
            if (energy < neededEnergy)
            {
                _crossHair.color = _crosshairDisabledColor;
            }
            else
            {
                _crossHair.color = _crossHairColor;
            }
        }
    }
}