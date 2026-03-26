using System;
using Global_Data;
using UnityEngine;
using UnityEngine.Events;

namespace Manager
{
    public class ToolBoxManager : MonoSingleton<ToolBoxManager>
    {
        // These match the requirements in your brief
        public event UnityAction<ToolType> OnToolSelected;
        public event UnityAction<Color> OnColorSelected;

        [Header("Current Settings")]
        public ToolType activeTool;
        public Color activeColor = Color.red; // Default starting color

        // This will be called by your UI Buttons
        private void Start()
        {
            SetTool(0); 
            SetColor(0);
        }

        public void SetTool(int toolIndex)
        {
            activeTool = (ToolType)toolIndex;
            OnToolSelected?.Invoke(activeTool);
            //Debug.Log("Active Tool: " + activeTool);
        }

        public void SetColor(int color)
        {
            ColorType colorType = (ColorType)color;
            switch (colorType)
            {
                case ColorType.Red:
                    activeColor = Color.red;
                    break;
                case ColorType.Blue:
                    activeColor = Color.cyan;
                    break;
                case ColorType.Green:
                    activeColor = Color.green;
                    break;
                case ColorType.Yellow:
                    activeColor = Color.yellow;
                    break;
                case ColorType.Purple:
                    activeColor = Color.magenta;
                    break;
                case ColorType.Pink:
                    activeColor = Color.pink;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            OnColorSelected?.Invoke(activeColor);
            //Debug.Log("Active Color: " + colorType);
        }
    }
}

