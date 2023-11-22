using System.Collections;
using System.Globalization;
using Microgame.PlatformMinigame;
using Photon.Pun;
using Rewired;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Multiplayer
{
    public class ManagerSpawnPlayers : MonoBehaviour
    {
        [SerializeField] 
        public GameObject playerPrefab;
        [SerializeField] 
        public float minX;
        [SerializeField] 
        public float maxX;
        [SerializeField] 
        public float minY;
        [SerializeField] 
        public float maxY;
        [SerializeField] 
        public GameObject minigameManagerToStart;
        [SerializeField] 
        public GameObject timerText;
        [SerializeField]
        public float timeToStartMinigame = 3;
        [SerializeField]
        public int playersToWait = 0;
        
        
        private IEnumerator _coroutineSpawner;
        private Text _textComponent;
        private float _timeToRemoveText = 1.5f;
        private Player player;
        private bool _shouldStartMinigame;
        private PhotonView viewPlayer;
        private bool _alreadyPlayed;

        void Start()
        {
            if (!_alreadyPlayed) // if this is the first time players play
            {
                player = ReInput.players.GetPlayer(0);
                _textComponent = timerText.GetComponent<Text>();
                if (_textComponent)
                {
                    _textComponent.text = "Waiting for players to join...";    
                }
            
    
                var randomPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
                var newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, quaternion.identity);
                viewPlayer = newPlayer.GetComponent<PhotonView>();
                newPlayer.SetActive(false);
                newPlayer.transform.SetParent(gameObject.transform,false);  
            }

        }
        
        // Update is called once per frame
        private void Update()
        {

            if (gameObject.transform.childCount >= playersToWait)
            {
                _shouldStartMinigame = true;
                ActivatePlayers();
            }

            if (_shouldStartMinigame)
            {
                if (gameObject.transform.childCount <= 0)
                {
                    _textComponent.text = "GAME OVER!";

                    _coroutineSpawner = WaitAndReloadMiniGame(3f);
                    StartCoroutine(_coroutineSpawner); 
                }
                else
                {
                    if (!minigameManagerToStart.activeInHierarchy)
                    {
                        _textComponent.text = timeToStartMinigame.ToString(CultureInfo.InvariantCulture);
                        timeToStartMinigame -= Time.deltaTime;
                        if (timeToStartMinigame <= 0)
                        {
                            if (minigameManagerToStart)
                            {
                                minigameManagerToStart.SetActive(true);
                                minigameManagerToStart.GetComponent<MinigameManager>().viewPlayer = viewPlayer;
                            }

                            _textComponent.text = "START!";
                            
                            _coroutineSpawner = WaitToDeleteText(_timeToRemoveText);
                            StartCoroutine(_coroutineSpawner);    
                        }
                    }
                }
            }
        }

        private void ActivatePlayers()
        {
            foreach (Transform childPlayer in gameObject.transform)
            {
                childPlayer.gameObject.SetActive(true);
            }
        }

        private IEnumerator WaitToDeleteText(float waitTime) // we wait for a given time and remove the screen text
        {
            yield return new WaitForSeconds(waitTime);
            _textComponent.text = string.Empty;
        }
        
        private IEnumerator WaitAndReloadMiniGame(float waitTime) // we wait for a given time and reload the minigame
        {
            
            yield return new WaitForSeconds(waitTime);
            _alreadyPlayed = true;
            PhotonNetwork.LoadLevel("PlatformMinigame");
        }
    }
}
