using System;
using System.Collections;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TurnPicker : MonoBehaviourPun
{
    [SerializeField] private int numberOfFaces;
    [SerializeField] private TMP_Text[] diceNumbers;
    [SerializeField] private int[] diceValues;
    [SerializeField] private int[] sortedDiceValues;
    [SerializeField] private TMP_Text[] diceResults;
    [SerializeField] private DiceManager manager;
    [SerializeField] private bool[] isRolling;
    [SerializeField] private bool[] hasRolled;
    [SerializeField] private GameObject[] players;
    [SerializeField] private GameObject[] panels;
    [SerializeField] private GameObject playerIdPrefab;
    [SerializeField] private GameObject playersLayout;
    private int numberOfRolledDice;
    private bool isLastPanel;
    private void Start()
    {
        InitializeData();
        OpenPanel(0);
    }

    void ClosePanels()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
    }

    void OpenPanel(int panel)
    {
        ClosePanels();
        panels[panel].SetActive(true);
    }

    void InitializeData()
    {
        isRolling = new Boolean[diceNumbers.Length];
        hasRolled = new Boolean[diceNumbers.Length];
        diceValues = new int[diceNumbers.Length];
        sortedDiceValues = new int[diceNumbers.Length];
        manager.isPlaying = false;
        for (int i = 0; i < hasRolled.Length; i++)
        {
            hasRolled[i] = false;
        }
        
        Invoke("RollCPUDice", 2);
    }

    void RollCPUDice()
    {
        for (int p = 0; p < players.Length; p++)
        {
            if (players[p].GetComponent<BoardPlayer>().isCPU)
            {
                RollDice(p);
            }
        }
    }
    
    void RollDice(int player)
    {
        StartCoroutine(RollingDiceRoutine(player));
    }

    IEnumerator RollingDiceRoutine(int player)
    {
        isRolling[player] = true;
        StartCoroutine(StopDiceRolling(player));
        while (isRolling[player])
        {
            diceValues[player] = Random.Range(1, (numberOfFaces + 1));
            diceNumbers[player].text = diceValues[player].ToString();
            yield return new WaitForSeconds(.1f);
        }
        var rndNumber = Random.Range(1, 7);
        while (diceValues.Contains(rndNumber))
        {
            rndNumber = Random.Range(1, 7);
        }

        diceValues[player] = rndNumber;
        //diceNumbers[player].text = diceValues[player].ToString();
        var photonViewDice = diceNumbers[player].GetComponent<PhotonView>();
        photonViewDice.RPC("UpdateText", RpcTarget.All, player, diceValues[player].ToString());

        hasRolled[player] = true;
        players[player].GetComponent<BoardPlayer>().initialDiceRoll = diceValues[player];
        numberOfRolledDice++;
        sortedDiceValues[player] = diceValues[player];
      
        if (numberOfRolledDice > 3)
        {
            yield return new WaitForSeconds(.5f);
            sortedDiceValues = sortedDiceValues.OrderByDescending(c => c).ToArray();
            manager.players = new GameObject[players.Length];
            yield return new WaitForSeconds(.5f);
            for (int i = 0; i < sortedDiceValues.Length; i++)
            {
                for (int j = 0; j < players.Length; j++)
                {
                    if (players[j].GetComponent<BoardPlayer>().initialDiceRoll == sortedDiceValues[i])
                    {
                        manager.players[i] = players[j];
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            yield return new WaitForSeconds(2);
            OpenPanel(1);
            StartCoroutine(PopulateTurnPanel());
        }
    }

    IEnumerator PopulateTurnPanel()
    {
        for (int i = 0; i < manager.players.Length; i++)
        {
            GameObject card = Instantiate(playerIdPrefab, playersLayout.transform);
            PlayerCard playercard = card.GetComponent<PlayerCard>();
            card.transform.localScale = new Vector3(1, 1, 1);
            playercard.turnTag.text = "# " + (i + 1).ToString();
            playercard.playerName.text = manager.players[i].gameObject.name;
            playercard.avatarImage.sprite = manager.players[i].GetComponent<BoardPlayer>().avatar;
            playercard.selectBackground(i);
            yield return new WaitForSeconds(2);
        }

        isLastPanel = true;
    }

    IEnumerator StopDiceRolling(int player)
    {
        yield return new WaitForSeconds(3);
        isRolling[player] = false;
    }

    void StopRolling(int currentPlayer)
    {
        isRolling[currentPlayer] = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!hasRolled[0])
            {
                if (players[0].GetComponent<BoardPlayer>().isCPU)
                {
                    return;
                }
                RollDice(0);
            }
            
        }        
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!hasRolled[1])
            {
                if (players[1].GetComponent<BoardPlayer>().isCPU)
                {
                    return;
                }
                RollDice(1);
            }
        }        
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!hasRolled[2])
            {
                if (players[2].GetComponent<BoardPlayer>().isCPU)
                {
                    return;
                }
                RollDice(2);      
            }
        }        
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!hasRolled[3])
            {
                if (players[3].GetComponent<BoardPlayer>().isCPU)
                {
                    return;
                }
                RollDice(3);         
            }
        }

        if (isLastPanel)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(closeTime());
                isLastPanel = false;
            }
        }
        
    }

    IEnumerator closeTime()
    {
        ClosePanels();
        yield return new WaitForSeconds(2);
        manager.isPlaying = true;
        manager.InitializeGame();
        gameObject.SetActive(false);
    }
}

