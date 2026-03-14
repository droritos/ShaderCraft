using System.Collections.Generic;
using Global_Data;
using Statue;
using UnityEngine;
// Make sure your MeshPart and TriangleData are still accessible

namespace Grooming
{
    public class GroomingGPUManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] ComputeShader myComputeShader;
        [SerializeField] ModelController  myModelController;
        private List<MeshFilter> TargetMeshFilter => myModelController.GroomableObject.GetMeshFilters();
    
        [Header("Passed Settings")]
        [SerializeField] float gravityStrength = 5.0f;
        [SerializeField, Range(0.1f, 1.0f)] float explosionTimeScale = 0.5f;

        // We make this public so our tools can access the buffers later!
        public List<MeshPart> MeshParts { get; private set; } = new List<MeshPart>();

        void Start()
        {
            InitializeBuffers();
        }

        void Update()
        {
            myComputeShader.SetFloat(GlobalData.ShaderProperties.Gravity, gravityStrength);
            myComputeShader.SetFloat(GlobalData.ShaderProperties.DeltaTime, Time.deltaTime * explosionTimeScale);
        
            foreach (MeshPart part in MeshParts)
            {
                myComputeShader.SetBuffer(0, GlobalData.ShaderProperties.TriangleBuffer, part.buffer);
                int batches = Mathf.CeilToInt(part.triangleCount / 64f);
                myComputeShader.Dispatch(0, batches, 1, 1);
            }
        }

        private void InitializeBuffers()
        {
            foreach (MeshFilter meshFilter in TargetMeshFilter)
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
            
                part.material.SetBuffer(GlobalData.ShaderProperties.TriangleBuffer, buffer);
                MeshParts.Add(part);
            
                SetShaderIntoLitAsset(meshFilter, buffer);
            }
        }

        private void OnDestroy()
        {
            foreach (MeshPart part in MeshParts)
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
                m.shader = Shader.Find(GlobalData.ShaderProperties.ShaveShaderName);
                m.SetBuffer(GlobalData.ShaderProperties.TriangleBuffer, buffer);
            }
        }
    }
}