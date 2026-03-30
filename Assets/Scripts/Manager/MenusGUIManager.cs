using Global_Data;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class MenusGUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject optionPanel;
        [SerializeField] private Button easyModeButton;
        [SerializeField] private Button mediumModeButton;
        [SerializeField] Button hardModeButton;

        private void Start()
        {
            // Added the () => to create the delegate, and fixed the button names!
            easyModeButton.onClick.AddListener(() => SwitchDifficulty(DifficultyType.Easy));
            mediumModeButton.onClick.AddListener(() => SwitchDifficulty(DifficultyType.Medium));
            hardModeButton.onClick.AddListener(() => SwitchDifficulty(DifficultyType.Hard));
        }

        private void SwitchDifficulty(DifficultyType difficulty)
        {
            EventManager.ButtonsOnClickEvent.RaiseChangeDifficulty(difficulty);
            optionPanel.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                optionPanel.SetActive(!optionPanel.activeSelf);
            }
        }
    }
}