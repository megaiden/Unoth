using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    [SerializeField] 
    public InputField createInput;
    [SerializeField] 
    public InputField joinInput;


    public void CreateRoom()
    {
        if(createInput.text.IsNullOrEmpty())
            return;
        
        PhotonNetwork.CreateRoom(createInput.text, new RoomOptions()
        {
            IsVisible = true, 
            IsOpen = true, 
            MaxPlayers = 8, 
            PublishUserId = true
        } );
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Orgate");
    }
}
