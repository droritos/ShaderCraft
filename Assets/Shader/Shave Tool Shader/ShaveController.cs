    using System;
    using System.Collections.Generic;
    using PublicObjects;
    using UnityEngine;

    public class ShaveController : MonoBehaviour
    {
      
        [Header("References")]
        [SerializeField] ComputeShader myComputeShader;
        [SerializeField] List<MeshFilter> targetMeshFilter;
        
        [Header("Passed Settings")]
        [SerializeField] float gravityStrength = 5.0f;
        //
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
                Vector3[] normals = mesh.normals;
                int[] triangles = mesh.triangles;
                int count = mesh.triangles.Length / 3;
            
                // Create the individual buffer
                ComputeBuffer buffer = new ComputeBuffer(count, 52);
            
                TriangleData[] initialData = new TriangleData[count];
                for (int i = 0; i < count; i++)
                {
                    // Use the normal of the first vertex of the triangle
                    Vector3 triNormal = normals[triangles[i * 3]];
                    initialData[i].normal = triNormal;

                    // The "Spread" Math:
                    // We take the normal and add a bit of random 'chaos' so it's not robotic
                    Vector3 randomDir = (triNormal + UnityEngine.Random.insideUnitSphere * 0.3f).normalized;
                    initialData[i].Velocity = randomDir * UnityEngine.Random.Range(2.0f, 4.0f);
    
                    initialData[i].Lifetime = 3.0f;
                    initialData[i].Color = Vector3.one; // Set to White
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
            
            foreach (MeshPart part in _meshParts)
            {
                // Tell the Compute Shader WHICH buffer to work on right now
                myComputeShader.SetBuffer(0, "_TriangleBuffer", part.buffer);

                int batches = Mathf.CeilToInt(part.triangleCount / 64f);
                myComputeShader.Dispatch(0, batches, 1, 1);
            }
            
            // Press 'R' to explode again!
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetExplosion();
            }
            
            if (Input.GetMouseButton(0)) 
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    MeshPart hitPart = GetHitPart(hit);

                    if (hitPart != null) 
                    {
                        CutTriangle(hitPart, hit.triangleIndex);
                    }
                }
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
                // 1. Create a fresh array for this specific mesh part
                TriangleData[] resetData = new TriangleData[part.triangleCount];
            
                for (int i = 0; i < part.triangleCount; i++)
                {
                    // Reset position to the original mesh spot
                    resetData[i].PositionOffset = Vector3.zero;
                
                    // Give them new random "flick" directions for the next time they are cut
                    resetData[i].Velocity = new Vector3(//
                        UnityEngine.Random.Range(-minXZ, maxXZ), 
                        UnityEngine.Random.Range(minY, maxY), 
                        UnityEngine.Random.Range(-minXZ, maxXZ)
                    );
                
                    // Set lifetime back to 3.0 so they stop falling/moving
                    resetData[i].Lifetime = 3.0f;
                }
            
                // 2. Upload the reset data to the specific GPU buffer for this part
                part.buffer.SetData(resetData);
            }
        }
        private MeshPart GetHitPart(RaycastHit hit)
        {
            MeshPart hitPart = _meshParts.Find(p => p.material == hit.collider.GetComponent<MeshRenderer>().sharedMaterial 
                                                    || p.material.name.Contains(hit.collider.GetComponent<MeshRenderer>().material.name.Replace(" (Instance)", "")));
            return hitPart;
        }
        void CutTriangle(MeshPart part, int index)
        {
            TriangleData[] data = new TriangleData[1];
        
            // Fetch data from the CORRECT buffer
            part.buffer.GetData(data, 0, index, 1);

            if (data[0].Lifetime >= 3.0f)
            {
                data[0].Lifetime = 2.99f; 
                part.buffer.SetData(data, 0, index, 1);
            }
        }
        private static void SetShaderIntoLitAsset(MeshFilter meshFilter, ComputeBuffer buffer)
        {
            MeshRenderer renderer = meshFilter.GetComponent<MeshRenderer>();
            // Use .materials (plural) to get the instances currently being rendered
            Material[] mats = renderer.materials; 

            foreach (Material m in mats)
            {
                m.shader = Shader.Find("Custom/ShaveShader");
                // Force the buffer connection to the instance
                m.SetBuffer("_TriangleBuffer", buffer);
            }
        }
    }