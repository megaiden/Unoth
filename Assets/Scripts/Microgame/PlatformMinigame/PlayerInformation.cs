using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    [SerializeField] 
    public string playerNickName;
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
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
