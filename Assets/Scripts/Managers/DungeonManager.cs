using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Video;

public class DungeonManager : MonoBehaviour
{
    [SerializeField]
    public GameObject dungeonGeneratorPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            var newDungeonGenerator = PhotonNetwork.Instantiate(dungeonGeneratorPrefab.name, new Vector3(), quaternion.identity);
            newDungeonGenerator.transform.SetParent(transform, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
