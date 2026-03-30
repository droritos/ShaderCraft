using System;
using Global_Data;
using UnityEngine;

namespace Manager
{
    public class DifficultyManager : MonoBehaviour
    {
        [Header("Render Textures")]
        [SerializeField] CustomRenderTexture playerShaveCanvas;
        [SerializeField] CustomRenderTexture playerColorCanvas;

        [Header("Answer Key Size")]
        [SerializeField] int targetResolution = 1024; // Your target images are 1024

        // A public static variable so the ScoreManager can easily read it!
        public static int CurrentScaleFactor = 1; 
        public static float CurrentTolerance = 0.1f;
        private void Awake()
        {
            EventManager.ButtonsOnClickEvent.ChangeDifficulty += ChangeDifficulty;
            //ChangeDifficulty(DifficultyType.Easy); // Start with Easy mode
        }

        private void OnDestroy()
        {
            EventManager.ButtonsOnClickEvent.ChangeDifficulty -= ChangeDifficulty;
        }

        private void ChangeDifficulty(DifficultyType difficulty)
        {
            switch (difficulty)
            {
                case DifficultyType.Easy:
                    CurrentTolerance = 0.4f;
            ChangeResolution(256); 
                    
                    break;
                case DifficultyType.Medium:
                    CurrentTolerance = 0.2f;
            ChangeResolution(512); 
                    
                    break;
                case DifficultyType.Hard:
                    CurrentTolerance = 0.05f; 
            ChangeResolution(1024); 
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null);
            }
        }
        private void ChangeResolution(int newResolution)
        {
            // 1. Tell the GPU to let go of the texture first!
            playerShaveCanvas.Release();
            playerColorCanvas.Release();

            // 2. Now we are allowed to change the size
            playerShaveCanvas.width = newResolution;
            playerShaveCanvas.height = newResolution;
            playerColorCanvas.width = newResolution;
            playerColorCanvas.height = newResolution;

            // 3. Rebuild the texture and wipe it clean
            playerShaveCanvas.Create();
            playerColorCanvas.Create();
    
            playerShaveCanvas.Initialize();
            playerColorCanvas.Initialize();

            // 4. Calculate the scale factor (e.g. 4096 / 1024 = 4)
            CurrentScaleFactor = targetResolution / newResolution;

            Debug.Log($"<color=cyan>Difficulty Set! Canvas: {newResolution}x{newResolution} | Scale Factor: {CurrentScaleFactor}</color>");
        }
    }
}