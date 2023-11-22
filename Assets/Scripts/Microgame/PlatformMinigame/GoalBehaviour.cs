using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Microgame.PlatformMinigame
{
    
    public class GoalBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] 
        public GameObject miniGameText;
        private bool _alreadyCheck;
        private IEnumerator _coroutineSpawner;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_alreadyCheck && other.CompareTag("Player"))
            {
                _alreadyCheck = true;
                miniGameText.GetComponent<Text>().text = $"WINNER IS {other.gameObject.name}";
                _coroutineSpawner = WaitAndReloadMiniGame(3f);
                StartCoroutine(_coroutineSpawner);  
            }
            
        }
        
        private IEnumerator WaitAndReloadMiniGame(float waitTime) // we wait for a given time and reload the minigame
        {
            yield return new WaitForSeconds(waitTime);
            photonView.RPC("ReloadMinigame", RpcTarget.AllViaServer);
        }
        
        [PunRPC]
        public void ReloadMinigame()
        {
            PhotonNetwork.LoadLevel("PlatformMinigame");
        }
        
    }
}