using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TankBattle.Global;
using UnityEngine;
using UnityEngine.UI;

namespace TankBattle.Navigation
{
    public class GameSettingsRoom : MonoBehaviourPunCallbacks
    {
        private int MAX_NUM_PLAYERS = 20;
        private int MAX_NUM_DUMMIES = 10;

        private int _numDummies = 0;
        private int _numSecondaryGuns = 0;

        private Text _numSecondaryGunsText;
        private Text _numDummiesText;
        private Button _sumSecondaryGuns;
        private Button _subtractSecondaryGuns;
        private Button _sumDummies;
        private Button _subtractDummies;

        private GameObject _dummiesBlock;
        private GameObject _numberOfSGBlock;

        private void Awake()
        {
            _dummiesBlock = transform.FirstOrDefault(t => t.name == "Dummies").gameObject;
            _numberOfSGBlock = transform.FirstOrDefault(t => t.name == "NumberOfSG").gameObject;
            
            _numSecondaryGunsText = transform.FirstOrDefault(t => t.name == "SGNumPanel").GetComponentInChildren<Text>();
            _sumSecondaryGuns = transform.FirstOrDefault(t => t.name == "PlusSGNumButton").GetComponent<Button>();
            _subtractSecondaryGuns = transform.FirstOrDefault(t => t.name == "SubtractSGNumButton").GetComponent<Button>();

            _numDummiesText = transform.FirstOrDefault(t => t.name == "DummiesNumPanel").GetComponentInChildren<Text>();
            _sumDummies = transform.FirstOrDefault(t => t.name == "PlusDummiesButton").GetComponent<Button>();
            _subtractDummies = transform.FirstOrDefault(t => t.name == "SubtractDummiesButton").GetComponent<Button>();
            
            _sumSecondaryGuns.onClick.AddListener(() => UpdateNumSecondaryGuns(_numSecondaryGuns + 1));
            _subtractSecondaryGuns.onClick.AddListener(() => UpdateNumSecondaryGuns(_numSecondaryGuns - 1));
            _sumDummies.onClick.AddListener(() => UpdateNumDummies(_numDummies + 1));
            _subtractDummies.onClick.AddListener(() => UpdateNumDummies(_numDummies - 1));

            _numDummies = GlobalMethods.NumberOfDummies;
            _numSecondaryGuns = GlobalMethods.NumberOfSecondaryGuns;

            _numDummiesText.text = $"{_numDummies}";
            _numSecondaryGunsText.text = $"{_numSecondaryGuns}";
        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (GlobalMethods.IsDesktop())
            {
                if (!PhotonNetwork.CurrentRoom.IsVisible && PhotonNetwork.IsMasterClient)
                {
                    SetSettingsVisible(true);
                }
                else
                {
                    SetSettingsVisible(false);
                }
            }
            else
            {
                SetSettingsVisible(true);
            }
        }

        private void UpdateNumSecondaryGuns(int numSecondaryGuns)
        {
            var num = Mathf.Clamp(numSecondaryGuns, 0, MAX_NUM_PLAYERS);
            _numSecondaryGuns = num;
            _numSecondaryGunsText.text = $"{num}";
            GlobalMethods.NumberOfSecondaryGuns = num;
        }

        private void UpdateNumDummies(int numDummies)
        {
            var num = Mathf.Clamp(numDummies, 0, MAX_NUM_DUMMIES);
            _numDummies = num;
            _numDummiesText.text = $"{num}";
            GlobalMethods.NumberOfDummies = num;
        }
        
        public void SetSettingsVisible(bool active)
        {
            Debug.Log($"Setting gameplay settings to {active}");
            _dummiesBlock.SetActive(active);
            _numberOfSGBlock.SetActive(active);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);

            if (!GlobalMethods.IsDesktop()) return;
            
            if (!PhotonNetwork.CurrentRoom.IsVisible && PhotonNetwork.LocalPlayer == newMasterClient)
            {
                SetSettingsVisible(true);
            }
        }
    }
}
