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
            easyModeButton.onClick.AddListener(() => EventManager.ButtonsOnClickEvent.RaiseChangeDifficulty(DifficultyType.Easy));
            mediumModeButton.onClick.AddListener(() => EventManager.ButtonsOnClickEvent.RaiseChangeDifficulty(DifficultyType.Medium));
            hardModeButton.onClick.AddListener(() => EventManager.ButtonsOnClickEvent.RaiseChangeDifficulty(DifficultyType.Hard));
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