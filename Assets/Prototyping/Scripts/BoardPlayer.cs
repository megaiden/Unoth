using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
public class BoardPlayer : MonoBehaviour
{
    public Sprite avatar;
    [SerializeField] private Route currentRoute;
    [SerializeField] private int routePosition;
    public int steps;
    
    [NonSerialized]
    public bool isMoving;
    [SerializeField] private float secondsInSpace;
    [SerializeField] private float movementSpeed;
    [SerializeField] private TMP_Text instructiontext;
    [SerializeField] private Route routeComponent;
    [SerializeField] private DiceManager _diceManager;
    [SerializeField] private GameObject playerCamera;
    public int initialDiceRoll;

    [SerializeField] 
    private SpriteRenderer _playerSprite;
    [SerializeField] 
    private SkinsCharacterScriptableObject _skinsCharacterScriptableObject;
    [SerializeField] 
    [Dropdown("_playerIndexList")]
    private int PlayerIndexSelector; 
            
    private List<int> _playerIndexList = new(){0,1,2,3};
    public bool isCPU;
    private void OnEnable()
    {
        playerCamera.SetActive(false);
     /*   if (_skinsCharacterScriptableObject.playerSkinSelectedList.Any())
        {
            _playerSprite.sprite = _skinsCharacterScriptableObject.playerSkinSelectedList[PlayerIndexSelector];   
            avatar = _skinsCharacterScriptableObject.playerSkinSelectedList[PlayerIndexSelector];
            //isCPU = _skinsCharacterScriptableObject.isCpu[PlayerIndexSelector];
        }*/
    }

    public void StartMoving()
    {
        steps = _diceManager.dieFaceValue;
        StartCoroutine(Move());
        _diceManager.generalCamera.SetActive(false);
        playerCamera.SetActive(true);
    }

    IEnumerator Move()
    {

        if (isMoving)
        {
            yield break;
        }
        isMoving = true;
        while (steps > 0)
        {

            routePosition++;
            routePosition %= currentRoute.childNodeList.Count;
            
            Vector3 nextPos = currentRoute.childNodeList[routePosition].position;
            while (MoveToNextNode(nextPos))
            {
                yield return null;
            }

            yield return new WaitForSeconds(secondsInSpace);
            steps --;
           // routePosition ++;
        }
        isMoving = false;
       
    }

    IEnumerator MoveBackwards()
    {
        if (isMoving)
        {
            yield break;
        }
        isMoving = true;
        while (steps < 0)
        {

            routePosition--;
            routePosition %= currentRoute.childNodeList.Count;
            
            Vector3 nextPos = currentRoute.childNodeList[routePosition].position;
            while (MoveToNextNode(nextPos))
            {
                yield return null;
            }

            yield return new WaitForSeconds(secondsInSpace);
            steps ++;
            // routePosition ++;
        }
        isMoving = false;
    }
    public void postMovementActions(bool isTrapped)
    {
        if (isTrapped)
        {
            Debug.Log("landed on trap");
            steps = -3;
            StartCoroutine(MoveBackwards());
            _diceManager.generalCamera.SetActive(false);
            playerCamera.SetActive(true);
        }
        else
        {
            //when player stops, we activate the trigger of colliders in board to see where the player landed
            instructiontext.text = "Press spacebar to roll dice";
            _diceManager.SwitchTurn();
            playerCamera.SetActive(false);
            _diceManager.generalCamera.SetActive(true);  
        }


    }

    bool MoveToNextNode(Vector3 goal)
    {
        Debug.Log("position is " + goal);
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, movementSpeed * Time.deltaTime));
        
    }

}
