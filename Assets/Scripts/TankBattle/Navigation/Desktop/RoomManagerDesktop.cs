using ExtensionMethods;
using Photon.Pun;
using UnityEngine.UI;

namespace TankBattle.Navigation.Desktop
{
    public class RoomManagerDesktop : RoomManager
    {
        
        protected override void SetRoomMode()
        {
            Text startButtonText = _startBtn.transform.FirstOrDefault(t => t.name == "Text").GetComponent<Text>();

            if (PhotonNetwork.IsMasterClient)
            {
                _loadBtn.gameObject.SetActive(true);
                _roomName.gameObject.SetActive(true);

                _privateRoom.interactable = true;
                _roomName.interactable = true;
                _loadBtn.interactable = true;
                startButtonText.text = "Comenzar";
            }
            else
            {
                _loadBtn.gameObject.SetActive(false);
                _roomName.gameObject.SetActive(false);
                _privateRoom.interactable = false;

                startButtonText.text = "Estoy listo";
            }
        }

    }
}
