using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Microgame.PlatformMinigame
{
    public class PlatformSpawner : MonoBehaviour
    {
        [SerializeField] 
        public GameObject[] platformsArray;
        [SerializeField] 
        public float minimumTimeToPlatformAppear =2.5f;
        [SerializeField] 
        public float maxTimeToPlatformAppear=6f;
        [SerializeField]
        private float secondsToChangeDirection;
        [SerializeField]
        private float platformMovementSpeed = 2f;
        [SerializeField]
        private Vector2 _directionToMove;
        [NonSerialized]
        public PhotonView viewPlayer;
        
        private IEnumerator _coroutineSpawner;
        private bool _isSpawing;
        private int? _lastRandomPlatformPick;
        private float _secondsToChangeDirectionStart;



        private void Start()
        {
            _secondsToChangeDirectionStart = secondsToChangeDirection;
        }

// once per frame
        private void Update()
        {
            if (viewPlayer.Controller.IsMasterClient) // if player is host
            {
                MovePlatform();
                
                if (!_isSpawing)  
                {

                    var randomPlatformPick = Random.Range(0, 3);
                    
                    _coroutineSpawner = SpawnPlatformAfterGivenTime(
                        Random.Range(minimumTimeToPlatformAppear, maxTimeToPlatformAppear),
                        randomPlatformPick);
                    
                    StartCoroutine(_coroutineSpawner);
                }  
            }
        }

        private void MovePlatform()
        {
            secondsToChangeDirection -= Time.deltaTime;
            if (secondsToChangeDirection >= 0) // if there is time
            {
                transform.Translate(_directionToMove * platformMovementSpeed * Time.deltaTime);
            }
            else
            {
                _directionToMove = _directionToMove * -1;
                secondsToChangeDirection = _secondsToChangeDirectionStart;
            }
        }
        
        private IEnumerator SpawnPlatformAfterGivenTime(float waitTime, int randomPlatformPick) // we wait for a given time and spawn a random platform
        {
            _isSpawing = true;
            
            if (_lastRandomPlatformPick == randomPlatformPick) // to not send the same platform again
            {
                var newPick = Random.Range(0,6);
                while (newPick == _lastRandomPlatformPick)
                {
                    newPick = Random.Range(0,6);
                }

                _lastRandomPlatformPick = newPick;
                randomPlatformPick = newPick;

            }
            else
            {
                _lastRandomPlatformPick = randomPlatformPick;
            }
            
            yield return new WaitForSeconds(waitTime);
            
            _isSpawing = false;
            var positionX = transform.position.x /*+ randomPositionX*/;
            var positionY = transform.position.y /*+ randomPositionY*/;
            
            var newPlatform =  PhotonNetwork.Instantiate(platformsArray[randomPlatformPick].name, new Vector3(positionX, positionY, 0), Quaternion.identity);
                //Instantiate(platformsArray[randomPlatformPick], new Vector3(positionX, positionY, 0),Quaternion.identity);
                
        }
        
    }
}
