using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace Multiplayer
{
    public class ConnectToServer : MonoBehaviourPunCallbacks
    {
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            if(!PhotonNetwork.InLobby)
                PhotonNetwork.JoinLobby();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            PhotonNetwork.Reconnect();
        }

        public override void OnJoinedLobby()
        {
            SceneManager.LoadScene("Lobby");
        }
        
    }
}
