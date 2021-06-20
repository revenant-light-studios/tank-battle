using System;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace TankBattle.InGameGUI
{
    public class SecondaryWeaponIcon : MonoBehaviour
    {
        private Image _image;
        private Text _text;

        private void Awake()
        {
            _image = transform.FirstOrDefault(t => t.name == "Image").GetComponent<Image>();
            _text = transform.FirstOrDefault(t => t.name == "Text").GetComponent<Text>();
        }

        public string Text
        {
            get => _text.text;
            set => _text.text = value;
        }

        public Sprite Icon
        {
            get => _image.sprite;
            set => _image.sprite = value;
        }
    }
}