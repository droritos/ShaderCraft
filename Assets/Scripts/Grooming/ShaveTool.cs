using UnityEngine;

namespace Grooming
{
    public class ShaveTool : MonoBehaviour
    {
        [Header("References")]
        public Camera mainCamera;
        public CustomRenderTexture shaveCanvas;

        Material runtimeMaterial;

        void Start()
        {
            if (shaveCanvas != null)
            {
                shaveCanvas.Initialize();
                runtimeMaterial = shaveCanvas.material;
            }
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100f))
                {
                    Vector2 hitUV = hit.textureCoord;

                    Debug.Log("I hit the object at UV: " + hitUV);

                    if (runtimeMaterial != null)
                    {
                        Vector4 brushUV = new Vector4(hitUV.x, hitUV.y, 0, 0);
                        runtimeMaterial.SetVector(Global_Data.GlobalData.ShaderProperties.HitUV, brushUV);
                    }

                    shaveCanvas.Update();
                }
            }
        }

        private void OnValidate()
        {
            if (!mainCamera)
                mainCamera = Camera.main;
        }
    }
}