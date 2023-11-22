using System.Collections.Generic;
using UnityEngine;

namespace Behaviors
{
    public class CharacterManagerBehavior : MonoBehaviour
    {
        [SerializeField] 
        public Animator selectedCharacter;

        [SerializeField] 
        public List<RuntimeAnimatorController> characterSkinsList = new();
        
        [SerializeField] 
        public GameObject backButton;
        
        [SerializeField] 
        public GameObject nextButton;

        public CharacterSelectManager componentCharacterSelectManager;
        
        [SerializeField] 
        private SkinsCharacterScriptableObject _skinsCharacterScriptableObject;
       
        private int _selectedSkinIndex;

        private void Start()
        {
            selectedCharacter.runtimeAnimatorController = characterSkinsList[0];
        }

        public void NextSkin()
        {  
            _selectedSkinIndex++;
            if (_selectedSkinIndex == characterSkinsList.Count)
            {
                _selectedSkinIndex = 0;
            }   
            selectedCharacter.runtimeAnimatorController = characterSkinsList[_selectedSkinIndex];
            _skinsCharacterScriptableObject.selectedPlayer = _selectedSkinIndex;
        }

        public void BackSkin()
        {
            _selectedSkinIndex--;
            if (_selectedSkinIndex < 0)
            {
                _selectedSkinIndex = characterSkinsList.Count - 1;
            }
            selectedCharacter.runtimeAnimatorController = characterSkinsList[_selectedSkinIndex];
            _skinsCharacterScriptableObject.selectedPlayer = _selectedSkinIndex;
        }

        public void RandomSkin()
        {
            var rand = Random.Range(0, characterSkinsList.Count);
            selectedCharacter.runtimeAnimatorController = characterSkinsList[rand];

        }

    }
}
