using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PlayersToLoadScriptableObject", menuName = "ScriptableObject/PlayersToLoad", order = 0)]
public class SkinsCharacterScriptableObject : ScriptableObject
{
    [Header("PlayersToLoadSelected")] 
    [SerializeField]
    public List<GameObject> playerToload 
        = new();
    [SerializeField]
    public int selectedPlayer = 0;

}
