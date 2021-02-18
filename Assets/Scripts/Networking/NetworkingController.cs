using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Networking
{
    public class NetworkingController : MonoBehaviourPunCallbacks
    {
        public void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        
        public override void OnConnectedToMaster()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Disconnected from server");
        }

        public override void OnCreatedRoom()
        {
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
        }
        
        public override void OnJoinedRoom()
        {
        }

        public override void OnLeftRoom()
        {
            Debug.Log("Local player left room");
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
        }
    }
}
