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
        [SerializeField] private Transform modelSlotTransfrom; 
 
    
        [Header("Settings")]
        [SerializeField] float rotationSpeed = 150f;
    
        [Header("Members")]
        //private Transform _modelTransfrom; // AKA the Head of the Hair that will do a haircut.
        // AKA the Head of the Hair that will do a haircut.
        // This is the "Member" you asked for
        private RotationHandler _rotationHandler;

        void Start()
        {
            //Events Sub
            EventManager.ButtonsOnClickEvent.NextLevel += LoadTextures; 
            EventManager.ButtonsOnClickEvent.Replay += LoadTextures; // Recreate / Reset
            
            EventManager.ScoreSystem.MatchValue += (float _) => HandleModelEnable(false); // when finish disable it
            EventManager.ButtonsOnClickEvent.ChangeDifficulty += (DifficultyType _) => HandleModelEnable(true); // after selection enable it
            
            HandleModelEnable(false); // make sure its false
            
            _rotationHandler = new RotationHandler(rotationSpeed);
        }

        private void OnDestroy()
        {
            EventManager.ButtonsOnClickEvent.NextLevel -= LoadTextures; 
            EventManager.ButtonsOnClickEvent.Replay -= LoadTextures; // Recreate / Reset
        }

        void Update()
        {
            float input = Input.GetAxis("Horizontal"); // Only Keyboard 'A' , 'D'

            float rotationAmount = _rotationHandler.CalculateRotation(input, Time.deltaTime);

            modelSlotTransfrom?.Rotate(Vector3.up, -rotationAmount);
        }

        public void LoadTextures()
        {
            CustomerFurTexture.Release();
            CustomerColorCanvas.Release();

            CustomerFurTexture.Create();
            CustomerColorCanvas.Create();

            CustomerFurTexture.Initialize();
            CustomerColorCanvas.Initialize();
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
            modelSlotTransfrom.gameObject.SetActive(enable);
        }
    }
}