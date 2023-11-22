using Photon.Pun;
using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnPlayersManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public GameObject playerPrefab;
    [SerializeField]
    public GameObject parentPlayersPrefab;
    [SerializeField]
    public float minX;
    [SerializeField]
    public float maxX;
    [SerializeField]
    public float minZ;
    [SerializeField]
    public float maxZ;
    [SerializeField]
    public int playersToWait = 0;
    [SerializeField]
    private SkinsCharacterScriptableObject _skinsCharacterScriptableObject;

    private PhotonView viewPlayer;
    private bool isAllPlayersJoined;
    private GameObject parentPlayers;

    void Start()
    {
        var randomPosition = new Vector3(Random.Range(minX, maxX), 1f, Random.Range(minZ, maxZ));
        var newPlayer = PhotonNetwork.Instantiate(_skinsCharacterScriptableObject.playerToload[_skinsCharacterScriptableObject.selectedPlayer].name, randomPosition, quaternion.identity);
        newPlayer.transform.Rotate(60, 0, 0);
        viewPlayer = newPlayer.GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient) 
        {
            parentPlayers = PhotonNetwork.Instantiate(parentPlayersPrefab.name, new Vector3(), quaternion.identity);
            newPlayer.transform.SetParent(parentPlayers.transform, true);
        }
        else
        {
            StartCoroutine(ConvertPlayersToParent(newPlayer));
        }

    }

    IEnumerator ConvertPlayersToParent(GameObject newPlayer)
    {
        yield return new WaitForSeconds(.3f);
        parentPlayers = GameObject.Find("playerParent(Clone)");
        if (parentPlayers)
        {
            newPlayer.transform.SetParent(parentPlayers.transform, true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //if (!isAllPlayersJoined && gameObject.transform.childCount >= playersToWait)
        //{
        //    ActivatePlayers();
        //}
    }


    private void ActivatePlayers()
    {
        foreach (Transform childPlayer in gameObject.transform)
        {
            childPlayer.gameObject.SetActive(true);
        }
        isAllPlayersJoined = true;
    }
}
