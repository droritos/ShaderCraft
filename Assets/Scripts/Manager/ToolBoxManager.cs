using System;
using Global_Data;
using UnityEngine;
using UnityEngine.Events;

namespace Manager
{
    public class ToolBoxManager : MonoSingleton<ToolBoxManager>
    {
        public event UnityAction<ToolType> OnToolSelected;
        public event UnityAction<Color> OnColorSelected;

        private ToolType _activeTool;
        private Color _activeColor;
        
        private void Start()
        {
            SetTool(0); 
            SetColor(-1);
        }

        public void SetTool(int toolIndex)
        {
            _activeTool = (ToolType)toolIndex;
            OnToolSelected?.Invoke(_activeTool);
            //Debug.Log("Active Tool: " + activeTool);
        }

        public void SetColor(int color)
        {
            ColorType colorType = (ColorType)color;
            switch (colorType)
            {
                case ColorType.Red:
                    _activeColor = Color.red;
                    break;
                case ColorType.Blue:
                    _activeColor = Color.cyan;
                    break;
                case ColorType.Green:
                    _activeColor = Color.green;
                    break;
                case ColorType.Yellow:
                    _activeColor = Color.yellow;
                    break;
                case ColorType.Purple:
                    _activeColor = Color.magenta;
                    break;
                case ColorType.Pink:
                    _activeColor = Color.pink;
                    break;
                default:
                    _activeColor = Color.white;
                    break;

            }
            OnColorSelected?.Invoke(_activeColor);
            //Debug.Log("Active Color: " + colorType);
        }
    }
}

