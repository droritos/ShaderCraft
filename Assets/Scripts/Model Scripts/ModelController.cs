using Global_Data;
using Manager;
using UnityEngine;

namespace Statue
{
    public class ModelController : MonoBehaviour
    {
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
            HandleModelEnable(false); // make sure its false
            EventManager.ScoreSystem.MatchValue += (float _) => HandleModelEnable(false); // when finish disable it
            EventManager.ButtonsOnClickEvent.ChangeDifficulty += (DifficultyType _) => HandleModelEnable(true); // after selection enable it
            
            // We "Initialize" our pure C# class here
            _rotationHandler = new RotationHandler(rotationSpeed);
            
        
            //_modelTransfrom = _modelTransfrom.transform; // The child is the Model/State/Head
        }
        
        void Update()
        {
            // 1. Get Input
            float input = Input.GetAxis("Horizontal"); // Only Keyboard 'A' , 'D'

            // 2. Ask the Handler to do the math
            float rotationAmount = _rotationHandler.CalculateRotation(input, Time.deltaTime);

            // 3. Apply the result to the Transform
            modelSlotTransfrom?.Rotate(Vector3.up, -rotationAmount);
        }

        private void HandleModelEnable(bool enable)
        {
            modelSlotTransfrom.gameObject.SetActive(enable);
        }
    }
}