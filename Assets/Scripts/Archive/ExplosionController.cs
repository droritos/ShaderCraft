using System.Collections.Generic;
using Global_Data;
using UnityEngine;

namespace Archive
{
    public class ExplosionController : MonoBehaviour
    {
  
        [Header("References")]
        [SerializeField] ComputeShader myComputeShader;
        [SerializeField] List<MeshFilter> targetMeshFilter;
    
        [Header("Passed Settings")]
        [SerializeField] float gravityStrength = 5.0f;
    
        [Header("Triangle Settings")]
        [SerializeField] float lifetime = 6f;
        [SerializeField, Range(0.1f, 1.0f)] float explosionTimeScale = 0.5f;

        [Header("Random Settings")] 
        [SerializeField] private float minXZ;
        [SerializeField] private float maxXZ;
        [SerializeField] private float minY;
        [SerializeField] private float maxY;

        #region Private Members
        private List<MeshPart> _meshParts = new List<MeshPart>();
        ComputeBuffer _triangleBuffer;
        int _triangleCount;
        #endregion

        #region << Unity Functions >>
        void Start()
        {
            foreach (MeshFilter meshFilter in targetMeshFilter)
            {
                Mesh mesh = meshFilter.sharedMesh;
                int count = mesh.triangles.Length / 3;
        
                // Create the individual buffer
                ComputeBuffer buffer = new ComputeBuffer(count, 28);
        
                TriangleData[] initialData = new TriangleData[count];
                for (int i = 0; i < count; i++)
                {
                    initialData[i].PositionOffset = Vector3.zero;
                    initialData[i].Velocity = new Vector3(
                        UnityEngine.Random.Range(-minXZ, maxXZ), 
                        UnityEngine.Random.Range(minY, maxY), 
                        UnityEngine.Random.Range(-minXZ, maxXZ)
                    );
                    initialData[i].Lifetime = 3.0f;
                } 
                buffer.SetData(initialData);

                // Store the part info
                MeshPart part = new MeshPart();
                part.buffer = buffer;
                part.triangleCount = count;
                part.material = meshFilter.GetComponent<MeshRenderer>().material;
        
                // Link the buffer to the material once here
                part.material.SetBuffer("_TriangleBuffer", buffer);
        
                _meshParts.Add(part);
            
                SetShaderIntoLitAsset(meshFilter, buffer);
            }
        
        }

        void Update()
        {
            myComputeShader.SetFloat("_Gravity", gravityStrength);
            myComputeShader.SetFloat("_DeltaTime", Time.deltaTime * explosionTimeScale);
        
            // Press 'R' to explode again!
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetExplosion();
            }
        
            foreach (MeshPart part in _meshParts)
            {
                // Tell the Compute Shader WHICH buffer to work on right now
                myComputeShader.SetBuffer(0, "_TriangleBuffer", part.buffer);

                int batches = Mathf.CeilToInt(part.triangleCount / 64f);
                myComputeShader.Dispatch(0, batches, 1, 1);
            }
        }

        private void OnDestroy()
        {
            foreach (MeshPart part in _meshParts)
            {
                if (part.buffer != null) part.buffer.Release();
            }
        }
        private void OnValidate()
        {
            // Convert the array from GetComponentsInChildren into a List
            if (targetMeshFilter == null || targetMeshFilter.Count <= 0)
            {
                targetMeshFilter = new List<MeshFilter>(GetComponentsInChildren<MeshFilter>());
            }
        }
        #endregion

        private void ResetExplosion()
        {
            foreach (MeshPart part in _meshParts)
            {
                // 1. Create a temporary array specifically for this part's size
                TriangleData[] resetData = new TriangleData[part.triangleCount];
        
                for (int i = 0; i < part.triangleCount; i++)
                {
                    resetData[i].PositionOffset = Vector3.zero;
                    resetData[i].Velocity = new Vector3(
                        UnityEngine.Random.Range(-minXZ, maxXZ), // Outward spread
                        UnityEngine.Random.Range(minY, maxY),      // Upward lift
                        UnityEngine.Random.Range(-minXZ, maxXZ)
                    );
                    resetData[i].Lifetime = lifetime;
                }
        
                // 2. Upload the new data to this specific part's GPU buffer
                part.buffer.SetData(resetData);
            }
        }
        private static void SetShaderIntoLitAsset(MeshFilter meshFilter, ComputeBuffer buffer)
        {
            // Inside your foreach (MeshFilter meshFilter in targetMeshFilter)
            MeshRenderer renderer = meshFilter.GetComponent<MeshRenderer>();
            Material[] mats = renderer.materials; // Accessing the material instances

            foreach (Material m in mats)
            {
                // You must manually set the shader to your custom one if it's currently "Lit"
                m.shader = Shader.Find("Custom/TriangleExplosionShader");
    
                // Now give it the buffer
                m.SetBuffer("_TriangleBuffer", buffer);
            }
        }
    }
}