using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class DiceManager : MonoBehaviour
{
    public bool isRolling;
    [SerializeField] private int dieFaces;
    public  int dieFaceValue;
    [SerializeField] private TMP_Text dieFaceText;
    [SerializeField] private BoardPlayer boardPlayer;
    [SerializeField] public GameObject[] players;
    [SerializeField] public int currentPlayer;
    public GameObject generalCamera;
    [SerializeField] private GameObject rollingText;
    [SerializeField] private TMP_Text currentTurnText;
    public bool isPlaying;
    public void InitializeGame()
    {
        boardPlayer = players[currentPlayer].GetComponent<BoardPlayer>();
        currentTurnText.text = "Current turn: Player " + (currentPlayer + 1).ToString();
        rollingText.SetActive(false);
        if (players[currentPlayer].GetComponent<BoardPlayer>().isCPU)
        {
            StartCoroutine(CPURoll());
        }
    }
    
    public void AssignPlayerOrder()
    {
        Debug.Log("Assigning order based on initial dice Roll");
        players.OrderBy(x => x.GetComponent<BoardPlayer>().initialDiceRoll).ToArray();
    }


    public void RollDice()
    {
        StartCoroutine(DiceTime());
        Invoke("StopRolling", Random.Range(1,4));
        rollingText.SetActive(true);
    }

    void StopRolling()
    {
        isRolling = false;
    }

    IEnumerator DiceTime()
    {
        isRolling = true;
        while (isRolling)
        {
            dieFaceValue = Random.Range(1, dieFaces + 1);
            dieFaceText.text = dieFaceValue.ToString();
            Debug.Log("value is " + dieFaceValue);
            yield return new WaitForSeconds(.1f);
        }
        dieFaceValue = Random.Range(1, dieFaces);
        dieFaceText.text = dieFaceValue.ToString();
        yield return new WaitForSeconds(2);
        rollingText.SetActive(false);
        boardPlayer.GetComponent<BoardPlayer>().StartMoving();

    }

    private void Update()
    {
        if (isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isRolling && !boardPlayer.isMoving)
            {
                RollDice();
            }    
        }
    }


    public void SwitchTurn()
    {
        if (currentPlayer >= players.Length -1)
        {
            currentPlayer = 0;
        }
        else
        {
            currentPlayer ++;  
        }
        boardPlayer = players[currentPlayer].GetComponent<BoardPlayer>();
        currentTurnText.text = "Current turn: Player " + (currentPlayer + 1).ToString();
        if (players[currentPlayer].GetComponent<BoardPlayer>().isCPU)
        {
            StartCoroutine(CPURoll());
        }
    }

     IEnumerator CPURoll()
    {
        yield return new WaitForSeconds(2);
        RollDice();
    }
     
     public void enablePlaying()
    {
        isPlaying = true;
    }
    
}

