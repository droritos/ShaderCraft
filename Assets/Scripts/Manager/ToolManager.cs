using UnityEngine;

namespace Manager
{
    public class ToolManager : MonoBehaviour
    {
        // These match the requirements in your brief
        public enum ToolType { Shave, Grow, Color }
    
        [Header("Current Settings")]
        public ToolType activeTool;
        public Color activeColor = Color.red; // Default starting color

        // This will be called by your UI Buttons
        public void SetTool(int toolIndex)
        {
            activeTool = (ToolType)toolIndex;
            Debug.Log("Active Tool: " + activeTool);
        }
    }
}