using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
   public Image avatarImage;
   public TMP_Text turnTag;
   public TMP_Text playerName;
   [SerializeField] Sprite[] backgrounds;
   [SerializeField] Image background;

   public void selectBackground(int index)
   {
      background.sprite = backgrounds[index];
   }
}
