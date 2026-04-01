using UnityEngine;
using UnityEngine.InputSystem;

namespace Manager 
{
    public class InputReader : MonoBehaviour
    {
        public Vector2 RotationInput { get; private set; }
        public bool PausePressed { get; private set; } 

        private InputSystem_Actions _inputActions;
        private void Awake()
        {
            _inputActions = new InputSystem_Actions();
        }

        private void OnEnable() => _inputActions.Player.Enable();
        private void OnDisable() => _inputActions.Player.Disable();

        private void Update()
        {
            RotationInput = _inputActions.Player.Move.ReadValue<Vector2>();
            PausePressed = _inputActions.Player.Pause.WasPressedThisFrame(); 
        }
    }
}