using System;
using Global_Data;
using Manager;
using UnityEngine;

namespace Grooming
{
    public class ShaveTool : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Camera mainCamera;
        [SerializeField] CustomRenderTexture shaveCanvas;

        [Header("Tool Settings")]
        [SerializeField] float cutSpeed = -0.05f;  // Negative subtracts hair
        [SerializeField] float growSpeed = 0.05f;  // Positive adds hair
        [SerializeField] ToolType currentTool;

        Material runtimeMaterial;
        private const string PaintValue = "_PaintValue";

        void Start()
        {
            ToolBoxManager.Instance.OnToolSelected += ChangeTool;
            
            if (shaveCanvas != null)
            {
                shaveCanvas.Initialize();
                runtimeMaterial = shaveCanvas.material;
            }
        }

        void OnDestroy()
        {
            if (ToolBoxManager.Instance != null)
                ToolBoxManager.Instance.OnToolSelected -= ChangeTool;
        }

        void Update()
        {
            HandleTool();
        }

        private void HandleTool()
        {
            if (Input.GetMouseButton(0)) // Can be changed to new input system
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100f))
                {
                    Vector2 hitUV = hit.textureCoord;

                    if (runtimeMaterial != null)
                    {
                        // 1. Send the UV Position (Shared across all tools)
                        Vector4 brushUV = new Vector4(hitUV.x, hitUV.y, 0, 0);
                        runtimeMaterial.SetVector(Global_Data.GlobalData.ShaderProperties.HitUV, brushUV);

                        // 2. Decide what the brush does based on the current tool
                        switch (currentTool)
                        {
                            case ToolType.Shave:
                                runtimeMaterial.SetFloat(PaintValue, cutSpeed);
                                break;
                            
                            case ToolType.Grow:
                                runtimeMaterial.SetFloat(PaintValue, growSpeed);
                                break;
                            
                            case ToolType.Color:
                                // Ready for Phase 2! 
                                // Example: runtimeMaterial.SetColor("_PaintColor", selectedColor);
                                break;
                            
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    // 3. Force the CRT to draw one frame
                    shaveCanvas.Update();
                }
                else
                {
                    // If we clicked, but missed the character, move the brush away
                    StopPainting();
                }
            }
            else
            {
                // If we aren't clicking at all, move the brush away
                StopPainting();
            }
        }

        private void StopPainting()
        {
            if (runtimeMaterial != null)
            {
                // Send an invalid UV (-1, -1) so the brush is hidden off-screen
                runtimeMaterial.SetVector(Global_Data.GlobalData.ShaderProperties.HitUV, new Vector4(-1, -1, 0, 0));
                shaveCanvas.Update();
            }
        }

        private void ChangeTool(ToolType newTool)
        {
            currentTool = newTool;
        }

        private void OnValidate()
        {
            if (!mainCamera)
                mainCamera = Camera.main;
        }
    }
}