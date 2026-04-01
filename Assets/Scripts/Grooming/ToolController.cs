using UnityEngine;
using Manager;
using Global_Data;
using Statue;

namespace Grooming
{
    public class ToolController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] GroomingRaycaster raycaster;
        [SerializeField] CanvasPainter painter;
        
        [Header("VFX Dependencies")] 
        [SerializeField] VFXManager vfxManager;
        [SerializeField] CustomerModelController _customerModelController;

        [Header("Tool Settings")]
        [SerializeField] float brushSize = 0.1f; 
        [SerializeField] float cutSpeed = -0.05f;  
        [SerializeField] float growSpeed = 0.05f;  
        [SerializeField] Color currentColor = Color.red;

        private ToolType _currentTool;
        private Texture2D _pixelReader;

        #region << Unity Functions >>
        private void Start()
        {
            _pixelReader = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        }

        void OnEnable()
        {
            if (raycaster != null)
            {
                raycaster.OnFurHit += HandleFurHit;
                raycaster.OnFurHover += HandleFurHover; 
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
                raycaster.OnFurHover -= HandleFurHover; 
                raycaster.OnInteractionStopped -= HandleInteractionStopped;
            }
            if (ToolBoxManager.Instance != null) ToolBoxManager.Instance.OnToolSelected -= ChangeTool;
            if (ToolBoxManager.Instance != null) ToolBoxManager.Instance.OnColorSelected -= SetSprayColor;
        }
        private void OnValidate()
        {
            if(!_customerModelController)
                _customerModelController = FindAnyObjectByType<CustomerModelController>();
        }
        #endregion

        // UPDATED: Now accepts the 3D hitPoint!
        private void HandleFurHit(Vector2 uv, Vector3 hitPoint)
        {
            switch (_currentTool)
            {
                case ToolType.Shave: 
                    painter.PaintLength(uv, cutSpeed, brushSize); 
                    Color cutColor = GetColorFromCRT(uv); 
                    vfxManager.PlayCutVFX(hitPoint, cutColor); // Spawn colored hair
                    break;
                
                case ToolType.Grow:  
                    painter.PaintLength(uv, growSpeed, brushSize); 
                    vfxManager.PlayRegrowVFX(hitPoint); // Spawn magical sparkles
                    break;
                
                case ToolType.Color: 
                    painter.PaintColor(uv, currentColor, brushSize); 
                    vfxManager.PlaySprayVFX(hitPoint); // Spawn spray paint burst
                    break;
            }
        }


        #region << Handle Tools >>
        private void ChangeTool(ToolType newTool) { _currentTool = newTool; }
        
        private void SetSprayColor(Color newColor)
        {
            currentColor = newColor;
            ChangeTool(ToolType.Color);
        }
        #endregion
        
        #region << Feedbacks >>
        
        private void HandleFurHover(Vector2 uv)
        {
            Shader.SetGlobalVector(GlobalMembers.ShaderIDs.HitUV, new Vector4(uv.x, uv.y, 0, 0));
            Shader.SetGlobalFloat(GlobalMembers.ShaderIDs.BrushSize, brushSize);
        }

        private void HandleInteractionStopped()
        {
            painter.StopAllPainting();
            Shader.SetGlobalVector(GlobalMembers.ShaderIDs.HitUV, new Vector4(-1, -1, 0, 0));
        }
        #endregion

        private Color GetColorFromCRT(Vector2 uv)
        {
            CustomRenderTexture playerColorCanvas = _customerModelController.CustomerColorCanvas;
            
            int x = Mathf.Clamp(Mathf.FloorToInt(uv.x * playerColorCanvas.width), 0, playerColorCanvas.width - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt(uv.y * playerColorCanvas.height), 0, playerColorCanvas.height - 1);

            RenderTexture previousActive = RenderTexture.active;
            RenderTexture.active = playerColorCanvas;

            _pixelReader.ReadPixels(new Rect(x, y, 1, 1), 0, 0);
            _pixelReader.Apply();

            RenderTexture.active = previousActive;
            return _pixelReader.GetPixel(0, 0);
        }
    }
}