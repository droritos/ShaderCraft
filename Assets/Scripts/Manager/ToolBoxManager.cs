using System;
using Global_Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Manager
{
    public class ToolBoxManager : MonoSingleton<ToolBoxManager>
    {
        public event UnityAction<ToolType> OnToolSelected;
        public event UnityAction<Color> OnColorSelected;
        
        [Header("Tools Buttons")]
        [SerializeField] private Button cutButton;
        [SerializeField] private Button growButton;
        [SerializeField] private Button paintButton;

        [Header("UI Settings")]
        [SerializeField] private Color defaultButtonColor = Color.purple;
        [SerializeField] private Color selectedButtonColor = Color.yellow;

        private ToolType _activeTool;
        private Color _activeColor;
        
        private void Start()
        {
            cutButton.onClick.AddListener(() => SetTool(0, cutButton));
            growButton.onClick.AddListener(() => SetTool(1, growButton));
            paintButton.onClick.AddListener(() => SetTool(2, paintButton));
            
            // First Invoke - Set cut tool as default
            SetTool(0, cutButton); 
            SetColor(-1);
        }

        public void SetTool(int toolIndex, Button clickedButton) 
        {
            _activeTool = (ToolType)toolIndex;
            OnToolSelected?.Invoke(_activeTool);
            
            cutButton.image.color = defaultButtonColor;
            growButton.image.color = defaultButtonColor;
            paintButton.image.color = defaultButtonColor;

            if (clickedButton != null)
            {
                clickedButton.image.color = selectedButtonColor;
            }
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
                    // FIX: Unity does not have a built-in Color.pink, so we have to mix one manually!
                    _activeColor = new Color(1f, 0.4f, 0.7f); 
                    break;
                default:
                    _activeColor = Color.white;
                    break;

            }
            OnColorSelected?.Invoke(_activeColor);
        }
    }
}