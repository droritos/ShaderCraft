using System;
using Global_Data;
using Manager;
using UnityEngine;

namespace Statue
{
    public class CustomerModelController : MonoBehaviour
    {
        #region << Sharable Members >>
        [field:SerializeField] public CustomRenderTexture CustomerFurTexture {get; private set;}
        [field:SerializeField] public CustomRenderTexture CustomerColorCanvas{ get; private set;}
        #endregion
        
        [Header("References")]
        [SerializeField] private Transform modelSlotTransform; 
        
        [SerializeField] private InputReader inputReader;
 
        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 150f;
        
        [SerializeField] private float minVerticalTilt = -45f; 
        [SerializeField] private float maxVerticalTilt = 45f;  

        private float _currentYaw = 0f;   // Horizontal spin
        private float _currentPitch = 0f; // Vertical tilt

        void Start()
        {
            EventManager.ButtonsOnClickEvent.NextLevel += LoadTextures; 
            EventManager.ButtonsOnClickEvent.Replay += LoadTextures; 
            
            EventManager.ScoreSystem.MatchValue += (float _) => HandleModelEnable(false); 
            EventManager.ButtonsOnClickEvent.ChangeDifficulty += (DifficultyType _) => HandleModelEnable(true); 
            
            HandleModelEnable(false); 

            if (modelSlotTransform != null)
            {
                Vector3 startAngles = modelSlotTransform.localEulerAngles;
                
                _currentPitch = (startAngles.x > 180) ? startAngles.x - 360 : startAngles.x;
                _currentYaw = startAngles.y;
            }
        }

        private void OnDestroy()
        {
            EventManager.ButtonsOnClickEvent.NextLevel -= LoadTextures; 
            EventManager.ButtonsOnClickEvent.Replay -= LoadTextures; 
        }

        void Update()
        {
            if (inputReader == null || modelSlotTransform == null) return;

            Vector2 input = inputReader.RotationInput; 

            _currentYaw -= input.x * rotationSpeed * Time.deltaTime; 
            
            _currentPitch += input.y * rotationSpeed * Time.deltaTime;

            _currentPitch = Mathf.Clamp(_currentPitch, minVerticalTilt, maxVerticalTilt);

            modelSlotTransform.localRotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
        }

        public void LoadTextures()
        {
            CustomerFurTexture.Release();
            CustomerColorCanvas.Release();

            CustomerFurTexture.Create();
            CustomerColorCanvas.Create();

            CustomerFurTexture.Initialize();
            CustomerColorCanvas.Initialize();

            HandleModelEnable(true);
        }

        public void UpdateResolution(int newResolution)
        {
            // 1. Tell the GPU to let go of the texture first!
            CustomerFurTexture.Release();
            CustomerColorCanvas.Release();

            // 2. Now we are allowed to change the size
            CustomerFurTexture.width = newResolution;
            CustomerFurTexture.height = newResolution;
            CustomerColorCanvas.width = newResolution;
            CustomerColorCanvas.height = newResolution;

            // 3. Rebuild the texture and wipe it clean
            CustomerFurTexture.Create();
            CustomerColorCanvas.Create();

            CustomerFurTexture.Initialize();
            CustomerColorCanvas.Initialize();
        }

        private void HandleModelEnable(bool enable)
        {
            if (modelSlotTransform != null)
            {
                modelSlotTransform.gameObject.SetActive(enable);
            }
        }
    }
}