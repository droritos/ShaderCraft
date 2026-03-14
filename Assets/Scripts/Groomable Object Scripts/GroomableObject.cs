using System.Collections.Generic;
using UnityEngine;

namespace Groomable_Object_Scripts
{
    public class GroomableObject : MonoBehaviour
    {   
        [SerializeField] List<MeshFilter> meshFilters;

        public List<MeshFilter> GetMeshFilters()
        {
            if (meshFilters == null || meshFilters.Count <= 0)
            {
                meshFilters = new List<MeshFilter>(GetComponentsInChildren<MeshFilter>());
            }
        
            return meshFilters;
        }

        private void OnValidate()
        {
            if (meshFilters == null || meshFilters.Count <= 0)
            {
                meshFilters = new List<MeshFilter>(GetComponentsInChildren<MeshFilter>());
            }
        }
    }
}
