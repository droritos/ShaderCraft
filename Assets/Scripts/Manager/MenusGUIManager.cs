using Global_Data;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public class MenusGUIManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainMenuPanel; 
        [SerializeField] private GameObject inGamePanel; 
        
        [Header("Input")]
        [SerializeField] private InputReader inputReader;
        
        [Header("Main Menu Buttons")] 
        [SerializeField] private Button easyModeButton;
        [SerializeField] private Button mediumModeButton;
        [SerializeField] private Button hardModeButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            EventManager.ButtonsOnClickEvent.NextLevel += () => ChangeCanvas(false); 
            EventManager.ButtonsOnClickEvent.Quit += Application.Quit;  // In Future change to Game Manager
            
            easyModeButton.onClick.AddListener(() => SwitchDifficulty(DifficultyType.Easy));
            mediumModeButton.onClick.AddListener(() => SwitchDifficulty(DifficultyType.Medium));
            hardModeButton.onClick.AddListener(() => SwitchDifficulty(DifficultyType.Hard));
            quitButton.onClick.AddListener(QuitGame);

            ChangeCanvas(true);
        }

        void Update()
        {
            if (inputReader == null) return;

            if (inputReader.PausePressed)
            {
                mainMenuPanel.SetActive(!mainMenuPanel.activeSelf);
                EventManager.PauseSystem.RaisePause(mainMenuPanel.activeSelf);
                ChangeCanvas(mainMenuPanel.activeSelf);
            }
        }

        private void OnValidate()
        {
            if(!inputReader)
                inputReader = FindAnyObjectByType<InputReader>();
        }

        private void SwitchDifficulty(DifficultyType difficulty)
        {
            EventManager.ButtonsOnClickEvent.RaiseChangeDifficulty(difficulty);
            mainMenuPanel.SetActive(false);
            EventManager.PauseSystem.RaisePause(false);
        }

        private void QuitGame()
        {
            EventManager.ButtonsOnClickEvent.RaiseQuit();
        }

        private void ChangeCanvas(bool state) 
        {
            mainMenuPanel.SetActive(state);
            inGamePanel.SetActive(!state);
        }
    }
}