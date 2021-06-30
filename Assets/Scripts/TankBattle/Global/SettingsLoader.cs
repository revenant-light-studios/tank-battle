using System;
using TankBattle.InputManagers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankBattle.Global
{
    public class SettingsLoader : MonoBehaviour
    {
        private void Start()
        {
            LoadInputSystemSettings();
        }
        private void LoadInputSystemSettings()
        {
            string savedBindings = GlobalMethods.KeyboardBindings;

            if (!String.IsNullOrEmpty(savedBindings))
            {
                InputActionAsset asset = Resources.Load<InputActionAsset>("InputSystem/UIInputActions");
                asset.LoadOverridesFromJSON(savedBindings);
            }
        }
    }
}