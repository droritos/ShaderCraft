using System;
using Global_Data;
using UnityEngine;

namespace Manager
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        ComputeShader scoreShader;

        [Header("Length Maps")] [SerializeField]
        CustomRenderTexture playerCanvas;

        [SerializeField] Texture2D targetTexture;

        [Header("Color Maps")] [SerializeField]
        CustomRenderTexture playerColorCanvas;

        [SerializeField] Texture2D targetColorTexture;

        private const string ScoreHair = "ScoreHair";

      

        public void CalculateScore()
        {
            if (!AreReferencesValid()) return;

            int resolution = playerCanvas.width;

            // 1. Let the GPU do the heavy lifting safely
            int matchingPixels = ExecuteScoreComputeShader(resolution);

            // 2. Calculate the final math
            float matchPercentage = CalculatePercentage(matchingPixels, resolution);

            // 3. Broadcast the result
            EventManager.ScoreSystem.RaiseMatchValue(matchPercentage);
        }

// --- EXTRACTED METHODS ---

        private bool AreReferencesValid()
        {
            if (scoreShader == null || playerCanvas == null || targetTexture == null ||
                playerColorCanvas == null || targetColorTexture == null)
            {
                Debug.LogError("ScoreManager: Missing a texture or shader reference!");
                return false;
            }

            return true;
        }

        private int ExecuteScoreComputeShader(int resolution)
        {
            // Create the buffer to park the data
            ComputeBuffer resultBuffer = new ComputeBuffer(1, sizeof(int));

            // Using a try-finally block guarantees the buffer is released, 
            // even if the Compute Shader throws an unexpected error.
            try
            {
                int[] resultData = new int[1] { 0 };
                resultBuffer.SetData(resultData);

                // Note: Make sure "ScoreHair" is a string literal in quotes unless it's a cached string variable!
                int kernelID = scoreShader.FindKernel(ScoreHair);

                // Hand to GPU using the fast Hashes
                scoreShader.SetTexture(kernelID, GlobalMembers.ShaderIDs.PlayerCanvas, playerCanvas);
                scoreShader.SetTexture(kernelID, GlobalMembers.ShaderIDs.TargetCanvas, targetTexture);
                scoreShader.SetTexture(kernelID, GlobalMembers.ShaderIDs.PlayerColorCanvas, playerColorCanvas);
                scoreShader.SetTexture(kernelID, GlobalMembers.ShaderIDs.TargetColorCanvas, targetColorTexture);

                scoreShader.SetBuffer(kernelID, GlobalMembers.ShaderIDs.ResultBuffer, resultBuffer);
                scoreShader.SetFloat(GlobalMembers.ShaderIDs.Tolerance, DifficultyManager.CurrentTolerance);
                scoreShader.SetInt(GlobalMembers.ShaderIDs.Resolution, resolution);
                scoreShader.SetInt(GlobalMembers.ShaderIDs.ScaleFactor, DifficultyManager.CurrentScaleFactor);

                int threadGroups = Mathf.CeilToInt(resolution / 8f);
                scoreShader.Dispatch(kernelID, threadGroups, threadGroups, 1);

                resultBuffer.GetData(resultData);
                return resultData[0];
            }
            finally
            {
                resultBuffer.Release();
            }
        }

        private float CalculatePercentage(int matchingPixels, int resolution)
        {
            int totalPixels = resolution * resolution;
    
            float rawPercentage = ((float)matchingPixels / totalPixels) * 100f;

            float curvedScore = Mathf.InverseLerp(30f, 60f, rawPercentage) * 100f;

            Debug.Log($"<color=yellow>Raw Score: {rawPercentage:F1}% | Curved Final Score: {curvedScore:F1}%</color>");
    
            return curvedScore;
        }
    }
}