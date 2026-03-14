    using System.Collections.Generic;
    using Global_Data;
    using Manager;
    //using UnityEditor.EditorTools;
    using UnityEngine;
/*
    public class ShaveController : MonoBehaviour
    {
      
        [Header("References")]
        [SerializeField] ComputeShader myComputeShader;
        [SerializeField] List<MeshFilter> targetMeshFilter;
        
        [Header("Passed Settings")]
        [SerializeField] float gravityStrength = 5.0f;
        
        [Header("Triangle Settings")]
        [SerializeField, Range(0.1f, 1.0f)] float explosionTimeScale = 0.5f;
        
        [Header("Color Settings")]
        private Color _activeColor = Color.white;


        #region Private Members
        private List<MeshPart> _meshParts = new List<MeshPart>();
        ComputeBuffer _triangleBuffer;
        int _triangleCount;
        #endregion

        #region << Unity Functions >>

        private void Awake()
        {
            ToolBoxManager.Instance.OnColorSelected -= SetColor; // Unsub at first 
            ToolBoxManager.Instance.OnColorSelected += SetColor; // Than Resub
        }

        void Start()
        {
            foreach (MeshFilter meshFilter in targetMeshFilter)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Vector3[] normals = mesh.normals;
                int[] triangles = mesh.triangles;
                int count = mesh.triangles.Length / 3;
            
                // Set Buffer
                int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(TriangleData));
                ComputeBuffer buffer = new ComputeBuffer(count, stride);
            
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
            
            // LEFT CLICK = SHAVE
            if (Input.GetMouseButton(0)) 
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    MeshPart hitPart = GetHitPart(hit);
                    if (hitPart != null) 
                    {
                        CutTriangle(hitPart, hit.triangleIndex); 
                    }
                }
            }

// RIGHT CLICK = PAINT
            if (Input.GetMouseButton(1)) 
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    MeshPart hitPart = GetHitPart(hit);
                    if (hitPart != null) 
                    {
                        // Uses that activePaintColor we added earlier!
                        PaintTriangle(hitPart, hit.triangleIndex, _activeColor); 
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
                    resetData[i].Color = Vector3.one; 
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

        private void SetColor(Color color)
        {
            _activeColor = color;
        }
        private void CutTriangle(MeshPart part, int index)
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
        private void PaintTriangle(MeshPart part, int index, Color newColor)
        {
            TriangleData[] data = new TriangleData[1];
    
            // 1. Fetch the specific triangle's data from the GPU
            part.buffer.GetData(data, 0, index, 1);

            // 2. Only update the color! Leave the lifetime alone so it doesn't fall.
            data[0].Color = new Vector3(newColor.r, newColor.g, newColor.b);
    
            // 3. Send the updated color back to the GPU
            part.buffer.SetData(data, 0, index, 1);
        }
        public void GrowTriangle(MeshPart part, int index)
        {
            TriangleData[] data = new TriangleData[1];
    
            // 1. Fetch the specific triangle's data from the GPU
            part.buffer.GetData(data, 0, index, 1);

            // 2. Only grow it if it is currently falling or dead (Lifetime < 3.0)
            if (data[0].Lifetime < 3.0f)
            {
                // Snap it back to its original position on the head
                data[0].PositionOffset = Vector3.zero;
        
                // Set the timer back to 3.0 so it "sticks" to the head again
                data[0].Lifetime = 3.0f;
        
                // (Optional) If you want the grown hair to reset to white, uncomment the line below. 
                // Otherwise, it will remember the color you painted it before you cut it!
                // data[0].Color = Vector3.one; 
        
                // 3. Send the fixed data back to the GPU
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
    */