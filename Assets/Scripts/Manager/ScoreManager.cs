using Global_Data;
using UnityEngine;

namespace Manager
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] ComputeShader scoreShader;
        
        [Header("Length Maps")]
        [SerializeField] CustomRenderTexture playerCanvas; 
        [SerializeField] Texture2D targetCanvas;           

        [Header("Color Maps")]
        [SerializeField] CustomRenderTexture playerColorCanvas; 
        [SerializeField] Texture2D targetColorCanvas;           

        [Header("Settings")]
        [Range(0.01f, 0.5f)]
        [SerializeField] float tolerance = 0.1f; 
        
        private const string ScoreHair = "ScoreHair";

        public void CalculateScore()
        {
            if (scoreShader == null || playerCanvas == null || targetCanvas == null || playerColorCanvas == null || targetColorCanvas == null)
            {
                Debug.LogError("ScoreManager: Missing a texture or shader reference!");
                return;
            }

            int resolution = playerCanvas.width;
            int totalPixels = resolution * resolution;

            ComputeBuffer resultBuffer = new ComputeBuffer(1, sizeof(int));
            int[] resultData = new int[1] { 0 };
            resultBuffer.SetData(resultData);

            int kernelID = scoreShader.FindKernel(ScoreHair);

            // Hand to GPU using the fast Hashes
            scoreShader.SetTexture(kernelID, GlobalMembers.ShaderIDs.PlayerCanvas, playerCanvas);
            scoreShader.SetTexture(kernelID, GlobalMembers.ShaderIDs.TargetCanvas, targetCanvas);
            scoreShader.SetTexture(kernelID, GlobalMembers.ShaderIDs.PlayerColorCanvas, playerColorCanvas);
            scoreShader.SetTexture(kernelID, GlobalMembers.ShaderIDs.TargetColorCanvas, targetColorCanvas);
            
            scoreShader.SetBuffer(kernelID, GlobalMembers.ShaderIDs.ResultBuffer, resultBuffer);
            scoreShader.SetFloat(GlobalMembers.ShaderIDs.Tolerance, tolerance);
            scoreShader.SetInt(GlobalMembers.ShaderIDs.Resolution, resolution);

            int threadGroups = Mathf.CeilToInt(resolution / 8f);
            scoreShader.Dispatch(kernelID, threadGroups, threadGroups, 1);

            resultBuffer.GetData(resultData);
            int matchingPixels = resultData[0];
            resultBuffer.Release();

            float matchPercentage = ((float)matchingPixels / totalPixels) * 100f;
            EventManager.ScoreSystem.RaiseMatchValue(matchPercentage);
            Debug.Log($"<color=green><b>Final Grooming Score: {matchPercentage:F1}%</b></color>");
        }
    }
}