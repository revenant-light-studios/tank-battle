using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using TankBattle.Global;
using TankBattle.InputManagers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class RebindingDisplay : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference _action;

        [SerializeField]
        private int bindIndex = 0;

        private Button _startRebindingButton;

        private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;

        private void Awake()
        {
            _startRebindingButton = transform.FirstOrDefault(t => t.name == "ActionButton").GetComponent<Button>();

            _startRebindingButton.GetComponentInChildren<Text>().text = InputControlPath.ToHumanReadableString(
            _action.action.bindings[bindIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
            _startRebindingButton.onClick.AddListener(StartRebinding);
        }


        public void StartRebinding()
        {
            // Debug.Log("Rebind");
            _startRebindingButton.interactable = false;

            _rebindingOperation = _action.action.PerformInteractiveRebinding(bindIndex)
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete())
                .Start();
        }

        private void RebindComplete()
        {
            _startRebindingButton.interactable = true;

            _startRebindingButton.GetComponentInChildren<Text>().text = InputControlPath.ToHumanReadableString(
            _action.action.bindings[bindIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
            _rebindingOperation.Dispose();

            string bindingsJSON = _action.asset.SaveOverridesToJSON(); 
            // Debug.Log(bindingsJSON);
            GlobalMethods.KeyboardBindings = bindingsJSON;
        }
    }
}
