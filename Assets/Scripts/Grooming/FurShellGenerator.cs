using System.Collections.Generic;
using UnityEngine;
using Global_Data; 

namespace Grooming
{
    public class FurShellGenerator : MonoBehaviour
    {
        [Header("Fur Settings")]
        [SerializeField] MeshFilter targetMeshFilter; 
        [SerializeField] Material furMaterial;     
        [SerializeField] Transform parent;     
    
        [Range(1, 40)]
        [SerializeField] int shellCount = 16;      
        [SerializeField] float furLength = 0.1f;   

        [Header("Pre-Baked Data (Do not touch)")]
        [SerializeField] private List<MeshRenderer> _preBakedRenderers = new List<MeshRenderer>();
        
        private List<Material> _shellMaterials = new List<Material>();

        void Start()
        {
            if (_preBakedRenderers.Count == 0)
            {
                Debug.LogError("No shells found! Did you forget to click 'Generate Shells' in the Inspector?");
                return;
            }

            // At runtime, we ONLY create the material clones. 
            // The heavy lifting (creating GameObjects and Meshes) is already done!
            for (int i = 0; i < _preBakedRenderers.Count; i++)
            {
                MeshRenderer mr = _preBakedRenderers[i];
                
                Material layerMat = new Material(furMaterial);
                mr.material = layerMat;

                float normalizedHeight = (float)i / shellCount;
                layerMat.SetFloat(GlobalMembers.ShaderIDs.ShellHeight, normalizedHeight);
                layerMat.SetFloat(GlobalMembers.ShaderIDs.MaxFurLength, furLength);

                _shellMaterials.Add(layerMat);
            }
        }

        public void UpdateAllShellTextures(Texture maskTex, Texture colorTex)
        {
            foreach (Material mat in _shellMaterials)
            {
                if (mat != null)
                {
                    mat.SetTexture(GlobalMembers.ShaderIDs.ShaveMask, maskTex);
                    mat.SetTexture(GlobalMembers.ShaderIDs.ColorMap, colorTex);
                }
            }
        }


        // --- 2. EDITOR LOGIC (The Magic Buttons) ---

        [ContextMenu("1. Generate Shells (Editor Only)")]
        private void GenerateShellsInEditor()
        {
            ClearShellsInEditor(); // Clean up old ones first!

            if (targetMeshFilter == null) return;
            Mesh targetMesh = targetMeshFilter.sharedMesh;

            for (int i = 0; i < shellCount; i++)
            {
                GameObject shell = new GameObject($"FurShell_Layer_{i}");
                shell.transform.SetParent(parent, false);

                MeshFilter mf = shell.AddComponent<MeshFilter>();
                mf.mesh = targetMesh;

                MeshRenderer mr = shell.AddComponent<MeshRenderer>();
                // We don't assign the material yet. We do that at runtime to keep memory clean!
                
                _preBakedRenderers.Add(mr);
            }
            
            Debug.Log($"<color=cyan>Successfully baked {shellCount} fur shells into the scene!</color>");
        }

        [ContextMenu("2. Clear Shells (Editor Only)")]
        private void ClearShellsInEditor()
        {
            // DestroyImmediate is required when destroying things outside of Play Mode
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(parent.GetChild(i).gameObject);
            }
            _preBakedRenderers.Clear();
        }
    }
}