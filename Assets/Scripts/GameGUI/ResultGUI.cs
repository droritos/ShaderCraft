using System.Collections.Generic;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameGUI
{
    public class ResultGUI : MonoBehaviour
    {
        [Header("Star UI")]
        [SerializeField] private List<Image> starImagesList; 
        [SerializeField] private Sprite starFullSprite;
        [SerializeField] private Sprite starEmptySprite;
        
        [Header("Text UI")]
        [SerializeField] TextMeshProUGUI matchValueText; 
        
        [Header("Buttons")]
        [SerializeField] Button ReplayButton;
        [SerializeField] Button NextLevelButton;

        private const string Percent = "%";

        private void Start()
        {
            ReplayButton.onClick.AddListener(EventManager.ButtonsOnClickEvent.RaiseReplay);
            NextLevelButton.onClick.AddListener(EventManager.ButtonsOnClickEvent.RaiseNextLevel);
        }

        public void DisplayResults(float matchValue) 
        {
            ChangeText(matchValue);

            int starsEarned = 1; // Default to 1 star for trying!
            
            if (matchValue >= 90f) {
                starsEarned = 3; 
            } 
            else if (matchValue >= 50f) {
                starsEarned = 2;
            }

            // 2. Loop through the list and swap the sprites
            for (int i = 0; i < starImagesList.Count; i++)
            {
                if (i < starsEarned)
                {
                    starImagesList[i].sprite = starFullSprite;
                }
                else
                {
                    starImagesList[i].sprite = starEmptySprite;
                }
            }
        }

        private void ChangeText(float matchValue)
        {
            matchValueText.text = "Match: " + matchValue.ToString("F1") + Percent; 
        }
    }
}