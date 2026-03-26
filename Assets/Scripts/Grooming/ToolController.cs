using UnityEngine;
using Manager;
using Global_Data;

namespace Grooming
{
    public class ToolController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] GroomingRaycaster raycaster;
        [SerializeField] CanvasPainter painter;

        [Header("Tool Settings")]
        [SerializeField] float brushSize = 0.1f; // Keep this matched with your CRT brush size!
        [SerializeField] float cutSpeed = -0.05f;  
        [SerializeField] float growSpeed = 0.05f;  
        [SerializeField] Color currentColor = Color.red;

        private ToolType currentTool;

        void OnEnable()
        {
            if (raycaster != null)
            {
                raycaster.OnFurHit += HandleFurHit;
                raycaster.OnFurHover += HandleFurHover; // Subscribe to hover
                raycaster.OnInteractionStopped += HandleInteractionStopped;
            }
            if (ToolBoxManager.Instance != null) ToolBoxManager.Instance.OnToolSelected += ChangeTool;
            if (ToolBoxManager.Instance != null) ToolBoxManager.Instance.OnColorSelected += SetSprayColor;
        }

        void OnDisable()
        {
            if (raycaster != null)
            {
                raycaster.OnFurHit -= HandleFurHit;
                raycaster.OnFurHover -= HandleFurHover; // Unsubscribe
                raycaster.OnInteractionStopped -= HandleInteractionStopped;
            }
            if (ToolBoxManager.Instance != null) ToolBoxManager.Instance.OnToolSelected -= ChangeTool;
            if (ToolBoxManager.Instance != null) ToolBoxManager.Instance.OnColorSelected -= SetSprayColor;
        }
        private void HandleFurHit(Vector2 uv)
        {
            switch (currentTool)
            {
                case ToolType.Shave: painter.PaintLength(uv, cutSpeed,brushSize); break;
                case ToolType.Grow:  painter.PaintLength(uv, growSpeed,brushSize); break;
                case ToolType.Color: painter.PaintColor(uv, currentColor,brushSize); break;
            }
        }
        private void HandleFurHover(Vector2 uv)
        {
            // Instead of telling one material, we broadcast this globally!
            Shader.SetGlobalVector(GlobalMembers.ShaderIDs.HitUV, new Vector4(uv.x, uv.y, 0, 0));
            Shader.SetGlobalFloat(GlobalMembers.ShaderIDs.BrushSize, brushSize);
        }

        private void HandleInteractionStopped()
        {
            painter.StopAllPainting();
            
            // Move the global cursor off-screen so the ring disappears
            Shader.SetGlobalVector(GlobalMembers.ShaderIDs.HitUV, new Vector4(-1, -1, 0, 0));
        }

        private void ChangeTool(ToolType newTool) { currentTool = newTool; }
        
        public void SetSprayColor(Color newColor)
        {
            currentColor = newColor;
            ChangeTool(ToolType.Color);
        }
    }
}