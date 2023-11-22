using Photon.Pun;
using UnityEngine;

namespace Microgame.PlatformMinigame
{
    public class MinigameManager : MonoBehaviour
    {
        [SerializeField] 
        public Transform endPosition;
        [SerializeField] 
        public GameObject camera;
        [SerializeField] 
        public float cameraSpeed = 1f;
        [SerializeField] 
        public GameObject[] platformsSpawners;

        public PhotonView viewPlayer;

        private void Start()
        {                
            if (viewPlayer.Controller.IsMasterClient) // if the player is host
            {
                foreach (var platformsSpawner in platformsSpawners)
                {
                    platformsSpawner.gameObject.GetComponent<PlatformSpawner>().viewPlayer = viewPlayer;
                    platformsSpawner.SetActive(true); // we activate the spawners. 
                }
            }
        }

        private void Update()
        {

            if (camera.transform.position.Equals(endPosition.transform.position)) //Camera arrived to spot so we stop moving it
            {
                foreach (var platformsSpawner in platformsSpawners)
                {
                    platformsSpawner.SetActive(false); // we deactivate the spawners. we stop spawning more platforms
                }
            }
            else
            {
                camera.transform.position = Vector3.MoveTowards(camera.transform.position, endPosition.position,
                    cameraSpeed * Time.deltaTime);
            }

        }
    }
}
