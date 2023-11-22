using System.Collections;
using System.Linq;
using Behaviors;
using Photon.Pun;
using UnityEngine;

public class CharacterSelectManagerOld : MonoBehaviour
{

   [SerializeField] public GameObject[] playerSelectors;
   [SerializeField] private GameObject continueBtn;
   [SerializeField] public bool[] isPlayer;
   [SerializeField] bool[] isCpu;
   private int totalPlayers;
   private void Start()
   {
      PhotonNetwork.AutomaticallySyncScene = true;
      totalPlayers = 0;
   }

   
   public void SelectPlayer(int index)
   {      
      isPlayer = new bool[playerSelectors.Length];
      for (var i = 0; i < playerSelectors.Length; i++)
      {
         isPlayer.Append(true);
         isPlayer[i] = true;
      }
      isCpu = new bool[playerSelectors.Length];

      isCpu[index] = false;
      playerSelectors[index].SetActive(true);
      totalPlayers++;
      continueBtn.SetActive(true);

   }
   

   public void CompletePlayers()
   {
     StartCoroutine(SelectRandomCpuPlayers());
     
   }

   IEnumerator SelectRandomCpuPlayers()
   {
      for (var i = 0; i < isPlayer.Length; i++)
      {
         if (isPlayer[i])
         {
            continue;
         }

         isCpu[i] = true;
      }

      yield return new WaitForSeconds(.3f);
      
      for (var i = 0; i < isCpu.Length; i++)
      {
         if (!isCpu[i])
         {
            continue;
         }

         SelectComputerPlayer(i);
         playerSelectors[i].GetComponent<CharacterManagerBehavior>().RandomSkin();

         yield return new WaitForSeconds(.5f);
      }
      yield return new WaitForSeconds(2);

      if (PhotonNetwork.IsMasterClient)
      {
         PhotonNetwork.LoadLevel("BoardMovement");
      }

   }
   
   private void SelectComputerPlayer(int index)
   {
      if (!isPlayer[index])
      {
         playerSelectors[index].SetActive(true);

      }

      totalPlayers++;
      continueBtn.SetActive(true);
   }
}
