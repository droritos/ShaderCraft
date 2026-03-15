using Groomable_Object_Scripts;
using UnityEngine;

namespace Statue
{
    public class ModelController : MonoBehaviour
    {
        [Header("References")]
        [field:SerializeField] public GroomableObject GroomableObject {get; private set;}
        [SerializeField] private Transform modelSlotTransfrom; 
    
        [Header("Settings")]
        [SerializeField] float rotationSpeed = 150f;
    
        [Header("Members")]
        private Transform _modelTransfrom; // AKA the Head of the Hair that will do a haircut.
        // AKA the Head of the Hair that will do a haircut.
        // This is the "Member" you asked for
        private RotationHandler _rotationHandler;

        void Start()
        {
            // We "Initialize" our pure C# class here
            _rotationHandler = new RotationHandler(rotationSpeed);
        
            //  _modelTransfrom = modelSlotTransfrom.GetChild(0).transform; // The child is the Model/State/Head
        }

        void Update()
        {
            // 1. Get Input
            float input = Input.GetAxis("Horizontal"); // Only Keyboard 'A' , 'D'

            // 2. Ask the Handler to do the math
            float rotationAmount = _rotationHandler.CalculateRotation(input, Time.deltaTime);

            // 3. Apply the result to the Transform
            _modelTransfrom?.Rotate(Vector3.up, -rotationAmount);
        }
    }
}