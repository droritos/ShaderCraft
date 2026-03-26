using Global_Data;
using UnityEngine;

namespace Grooming
{
    public class CanvasPainter : MonoBehaviour
    {
        [Header("Length Canvas")]
        [SerializeField] CustomRenderTexture shaveCanvas;
        Material shaveMaterial;

        [Header("Color Canvas")]
        [SerializeField] CustomRenderTexture colorCanvas;
        Material colorMaterial;

        void Start()
        {
            if (shaveCanvas != null)
            {
                shaveCanvas.Initialize();
                shaveMaterial = shaveCanvas.material;
            }
            
            if (colorCanvas != null)
            {
                colorCanvas.Initialize();
                colorMaterial = colorCanvas.material;
            }
        }

        public void PaintLength(Vector2 uv, float value, float brushSize)
        {
            if (shaveMaterial == null) return;
            
            Vector4 brushUV = new Vector4(uv.x, uv.y, 0, 0);
            shaveMaterial.SetVector(GlobalMembers.ShaderIDs.HitUV, brushUV);
            shaveMaterial.SetFloat(GlobalMembers.ShaderIDs.PaintValue, value);
            shaveMaterial.SetFloat(GlobalMembers.ShaderIDs.BrushSize, brushSize); 
            
            shaveCanvas.Update();
        }

        public void PaintColor(Vector2 uv, Color color, float brushSize)
        {
            if (colorMaterial == null) return;

            Vector4 brushUV = new Vector4(uv.x, uv.y, 0, 0);
            colorMaterial.SetVector(GlobalMembers.ShaderIDs.HitUV, brushUV);
            colorMaterial.SetColor(GlobalMembers.ShaderIDs.PaintColor, color);
            colorMaterial.SetFloat(GlobalMembers.ShaderIDs.BrushSize, brushSize);
            
            colorCanvas.Update();
        }

        public void StopAllPainting()
        {
            Vector4 offScreen = new Vector4(-1, -1, 0, 0);
            
            if (shaveMaterial != null)
            {
                shaveMaterial.SetVector(GlobalMembers.ShaderIDs.HitUV, offScreen);
                shaveCanvas.Update();
            }

            if (colorMaterial != null)
            {
                colorMaterial.SetVector(GlobalMembers.ShaderIDs.HitUV, offScreen);
                colorCanvas.Update();
            }
        }
    }
}