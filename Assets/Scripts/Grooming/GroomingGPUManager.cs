using System.Collections.Generic;
using Global_Data;
using UnityEngine;
// Make sure your MeshPart and TriangleData are still accessible

namespace Grooming
{
    public class GroomingGPUManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] ComputeShader myComputeShader;
        [SerializeField] List<MeshFilter> targetMeshFilter;
    
        [Header("Passed Settings")]
        [SerializeField] float gravityStrength = 5.0f;
        [SerializeField, Range(0.1f, 1.0f)] float explosionTimeScale = 0.5f;

        // We make this public so our tools can access the buffers later!
        public List<MeshPart> meshParts { get; private set; } = new List<MeshPart>();

        void Start()
        {
            InitializeBuffers();
        }

        void Update()
        {
            myComputeShader.SetFloat("_Gravity", gravityStrength);
            myComputeShader.SetFloat("_DeltaTime", Time.deltaTime * explosionTimeScale);
        
            foreach (MeshPart part in meshParts)
            {
                myComputeShader.SetBuffer(0, "_TriangleBuffer", part.buffer);
                int batches = Mathf.CeilToInt(part.triangleCount / 64f);
                myComputeShader.Dispatch(0, batches, 1, 1);
            }
        }

        private void InitializeBuffers()
        {
            foreach (MeshFilter meshFilter in targetMeshFilter)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Vector3[] normals = mesh.normals;
                int[] triangles = mesh.triangles;
                int count = mesh.triangles.Length / 3;
            
                int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TriangleData));
                ComputeBuffer buffer = new ComputeBuffer(count, stride);
            
                TriangleData[] initialData = new TriangleData[count];
                for (int i = 0; i < count; i++)
                {
                    Vector3 triNormal = normals[triangles[i * 3]];
                    initialData[i].normal = triNormal;

                    Vector3 randomDir = (triNormal + UnityEngine.Random.insideUnitSphere * 0.3f).normalized;
                    initialData[i].Velocity = randomDir * UnityEngine.Random.Range(2.0f, 4.0f);
                    initialData[i].Lifetime = 3.0f;
                    initialData[i].Color = Vector3.one; 
                }
                buffer.SetData(initialData);

                MeshPart part = new MeshPart();
                part.buffer = buffer;
                part.triangleCount = count;
                part.material = meshFilter.GetComponent<MeshRenderer>().material;
            
                part.material.SetBuffer("_TriangleBuffer", buffer);
                meshParts.Add(part);
            
                SetShaderIntoLitAsset(meshFilter, buffer);
            }
        }

        private void OnDestroy()
        {
            foreach (MeshPart part in meshParts)
            {
                if (part.buffer != null) part.buffer.Release();
            }
        }

        private void SetShaderIntoLitAsset(MeshFilter meshFilter, ComputeBuffer buffer)
        {
            MeshRenderer renderer = meshFilter.GetComponent<MeshRenderer>();
            Material[] mats = renderer.materials; 
            foreach (Material m in mats)
            {
                m.shader = Shader.Find("Custom/ShaveShader");
                m.SetBuffer("_TriangleBuffer", buffer);
            }
        }
    }
}