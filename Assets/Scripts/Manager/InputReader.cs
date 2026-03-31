using UnityEngine;
using UnityEngine.InputSystem;

namespace Manager 
{
    public class InputReader : MonoBehaviour
    {
        // This is the C# class Unity just generated for you!
        private InputSystem_Actions _inputActions;

        public Vector2 RotationInput { get; private set; }

        private void Awake()
        {
            _inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();
        }

        private void Update()
        {
            RotationInput = _inputActions.Player.Move.ReadValue<Vector2>();
        }
    }
}