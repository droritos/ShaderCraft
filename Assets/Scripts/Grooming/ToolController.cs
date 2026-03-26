using Global_Data;
using UnityEngine;
using Manager;

namespace Grooming
{
    public class ToolController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] GroomingRaycaster raycaster;
        [SerializeField] CanvasPainter painter;

        [Header("Tool Settings")]
        [SerializeField] float cutSpeed = -0.05f;  
        [SerializeField] float growSpeed = 0.05f;  
        [SerializeField] Color currentColor = Color.red;

        private ToolType _currentTool;

        void OnEnable()
        {
            // Subscribe to the Raycaster's shouts
            if (raycaster != null)
            {
                raycaster.OnFurHit += HandleFurHit;
                raycaster.OnInteractionStopped += HandleInteractionStopped;
            }

            // Subscribe to your UI Manager
            if (ToolBoxManager.Instance != null)
            {
                ToolBoxManager.Instance.OnToolSelected += ChangeTool;
                ToolBoxManager.Instance.OnColorSelected += SetSprayColor;
            }
        }

        void OnDisable()
        {
            // ALWAYS Unsubscribe to prevent memory leaks!
            if (raycaster != null)
            {
                raycaster.OnFurHit -= HandleFurHit;
                raycaster.OnInteractionStopped -= HandleInteractionStopped;
            }

            if (ToolBoxManager.Instance != null)
            {
                ToolBoxManager.Instance.OnToolSelected -= ChangeTool;
                ToolBoxManager.Instance.OnColorSelected -= SetSprayColor;
            }
        }

        // The Raycaster shouted that we hit the fur!
        private void HandleFurHit(Vector2 uv)
        {
            switch (_currentTool)
            {
                case ToolType.Shave:
                    painter.PaintLength(uv, cutSpeed);
                    break;
                case ToolType.Grow:
                    painter.PaintLength(uv, growSpeed);
                    break;
                case ToolType.Color:
                    painter.PaintColor(uv, currentColor);
                    break;
            }
        }

        // The Raycaster shouted that the player stopped clicking
        private void HandleInteractionStopped()
        {
            painter.StopAllPainting();
        }

        private void ChangeTool(ToolType newTool)
        {
            _currentTool = newTool;
        }

        // Connect this to your UI buttons!
        public void SetSprayColor(Color newColor)
        {
            currentColor = newColor;
            ChangeTool(ToolType.Color);
        }
    }
}