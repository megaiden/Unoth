using Photon.Pun;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemsGenerator : MonoBehaviourPunCallbacks
{

    [SerializeField]
    public List<GameObject> itemPrefab;

    [SerializeField]
    public List<Vector3> positionToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient) 
        {
            // i would like to make this better, but unity is not accepting class types as serialize fields
            for (int i = 0; i < itemPrefab.Count; i++)
            {
                var item = itemPrefab[i];
                var position = positionToSpawn[i];
                var itemNetwork = PhotonNetwork.Instantiate(item.name, position, quaternion.RotateX(120));
                photonView.RPC("RPC_CreationItemsBroadCast", RpcTarget.AllBufferedViaServer, itemNetwork.name, position);
            }
           
        }
    }

    #region PUN FUNCTIONS
    [PunRPC]
    private void RPC_CreationItemsBroadCast(string itemNetworkName, Vector3 position)
    {
        var itemNetwork = GameObject.Find(itemNetworkName);
        itemNetwork.transform.localPosition = new Vector3(position.x, position.y, position.z);
        itemNetwork.transform.SetParent(transform, true);
    }
    #endregion

}
