using ExtensionMethods;
using HightTide.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class CreditsManager : MonoBehaviour
    {
        private Button _returnBtn;
        //private DisplayCredits _creditsMove;

        public delegate void OnReturnMainMenuDelegate();
        public OnReturnMainMenuDelegate OnReturnMainMenu;

        private void Awake()
        {
           // _creditsMove = transform.FirstOrDefault(t => t.name == "DisplayCredits").GetComponent<DisplayCredits>();
            _returnBtn = transform.FirstOrDefault(t => t.name == "ReturnBtn").GetComponent<Button>();

            _returnBtn.onClick.AddListener(ReturnMainMenu);
        }
        public void ReturnMainMenu()
        {
            OnReturnMainMenu?.Invoke();
        }

        private void OnEnable()
        {
            Debug.Log("Start Credits");
            //_creditsMove?.StartCredits();
        }

        private void OnDisable()
        {
            Debug.Log("Finish Credits");
            //_creditsMove?.RestartPositions();
        }
    }
}