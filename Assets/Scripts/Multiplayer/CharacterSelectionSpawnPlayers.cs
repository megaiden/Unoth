using Behaviors;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace Multiplayer
{
    public class CharacterSelectionSpawnPlayers: MonoBehaviour
    {
        [SerializeField] 
        public GameObject playerPrefab;
        [SerializeField] 
        public GameObject characterSelectManager;

        public static CharacterSelectionSpawnPlayers instance;
        private CharacterSelectManager _componentCharacterSelectManager;
        private GameObject newPlayer;
        private void Awake()
        {
            instance = this;
        }

        public void Start()
        {

            if (PhotonNetwork.IsConnected)
            {
                SpawnPlayer();
            }
        }


        private void SpawnPlayer()
        {
            _componentCharacterSelectManager = characterSelectManager.GetComponent<CharacterSelectManager>();
            var childCount = transform.childCount;
            
            newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(), quaternion.identity);
            var characterManagerBehaviorComponent = newPlayer.GetComponent<CharacterManagerBehavior>();
           // characterManagerBehaviorComponent.PlayerIndexSelector = childCount;
        }
        
    }
}