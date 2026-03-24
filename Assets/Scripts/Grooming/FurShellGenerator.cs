using UnityEngine;

namespace Grooming
{
    public class FurShellGenerator : MonoBehaviour
    {
        [Header("Fur Settings")]
        [SerializeField] MeshFilter targetMeshFilter; // Changed to MeshFilter so you can drag the GameObject!
        [SerializeField] Material furMaterial;     
        [SerializeField] Transform parent;     
    
        [Range(1, 40)]
        [SerializeField] int shellCount = 16;      
    
        [SerializeField] float furLength = 0.1f;   

        void Start()
        {
            if (targetMeshFilter != null && furMaterial != null)
            {
                GenerateShells();
            }
            else
            {
                Debug.LogError("FurShellGenerator is missing its references!");
            }
        }

        void GenerateShells()
        {
            // Extract the actual mesh data from the MeshFilter you dragged in
            Mesh targetMesh = targetMeshFilter.sharedMesh;

            for (int i = 0; i < shellCount; i++)
            {
                // 1. Create a new empty GameObject for this layer
                GameObject shell = new GameObject($"FurShell_Layer_{i}");
            
                // 2. Make it a child of this object so it stays organized
                shell.transform.SetParent(parent, false);

                // 3. Add the Mesh and Material
                MeshFilter mf = shell.AddComponent<MeshFilter>();
                mf.mesh = targetMesh;

                MeshRenderer mr = shell.AddComponent<MeshRenderer>();
            
                // Create a unique instance of the material for this specific layer
                Material layerMat = new Material(furMaterial);
                mr.material = layerMat;

                // 4. THE MAGIC MATH: Calculate how high this layer is (from 0.0 to 1.0)
                float normalizedHeight = (float)i / shellCount;
            
                // 5. Send this height to the Shader Graph!
                layerMat.SetFloat("_ShellHeight", normalizedHeight);
                layerMat.SetFloat("_MaxFurLength", furLength);
            }
        }
    }
}