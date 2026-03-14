using System;
using UnityEngine;

namespace Grooming
{
    public class ShaveTool : MonoBehaviour
    {
        [Header("References")]
        public Camera mainCamera;
        public CustomRenderTexture shaveCanvas;
        public Material brushMaterial; // We will slot our M_Brush here

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100f))
                {
                    // Get the UV coordinate of the hit
                    Vector2 hitUV = hit.textureCoord;
                
                    // Tell the brush material where we clicked
                    brushMaterial.SetVector(Global_Data.GlobalData.ShaderProperties.HitUV, hitUV);

                    // Tell the Custom Render Texture to update using our brush
                    shaveCanvas.Update();
                }
            }
        }

        private void OnValidate()
        {
            if(!mainCamera)
                mainCamera = Camera.main;
        }
    }
}