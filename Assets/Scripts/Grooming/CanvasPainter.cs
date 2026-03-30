using System;
using Global_Data;
using Statue;
using UnityEngine;

namespace Grooming
{
    public class CanvasPainter : MonoBehaviour
    {
        [SerializeField] CustomerModelController _customerModelController;

        private CustomRenderTexture _furCanvas => _customerModelController.CustomerFurTexture;
        private Material _shaveMaterial;

        private CustomRenderTexture _colorCanvas => _customerModelController.CustomerColorCanvas;
        private Material _colorMaterial;

        void Start()
        {
            if (_furCanvas != null)
            {
                _furCanvas.Initialize();
                _shaveMaterial = _furCanvas.material;
            }
            
            if (_colorCanvas != null)
            {
                _colorCanvas.Initialize();
                _colorMaterial = _colorCanvas.material;
            }
        }

        private void OnValidate()
        {
            if(!_customerModelController)
                _customerModelController = FindAnyObjectByType<CustomerModelController>();
        }

        public void PaintLength(Vector2 uv, float value, float brushSize)
        {
            if (_shaveMaterial == null) return;
            
            Vector4 brushUV = new Vector4(uv.x, uv.y, 0, 0);
            _shaveMaterial.SetVector(GlobalMembers.ShaderIDs.HitUV, brushUV);
            _shaveMaterial.SetFloat(GlobalMembers.ShaderIDs.PaintValue, value);
            _shaveMaterial.SetFloat(GlobalMembers.ShaderIDs.BrushSize, brushSize); 
            
            _furCanvas.Update();
        }

        public void PaintColor(Vector2 uv, Color color, float brushSize)
        {
            if (_colorMaterial == null) return;

            Vector4 brushUV = new Vector4(uv.x, uv.y, 0, 0);
            _colorMaterial.SetVector(GlobalMembers.ShaderIDs.HitUV, brushUV);
            _colorMaterial.SetColor(GlobalMembers.ShaderIDs.PaintColor, color);
            _colorMaterial.SetFloat(GlobalMembers.ShaderIDs.BrushSize, brushSize);
            
            _colorCanvas.Update();
        }

        public void StopAllPainting()
        {
            Vector4 offScreen = new Vector4(-1, -1, 0, 0);
            
            if (_shaveMaterial != null)
            {
                _shaveMaterial.SetVector(GlobalMembers.ShaderIDs.HitUV, offScreen);
                _furCanvas.Update();
            }

            if (_colorMaterial != null)
            {
                _colorMaterial.SetVector(GlobalMembers.ShaderIDs.HitUV, offScreen);
                _colorCanvas.Update();
            }
        }
    }
}