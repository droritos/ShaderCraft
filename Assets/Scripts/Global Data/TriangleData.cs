
using UnityEngine;

namespace Global_Data
{
    struct TriangleData
    {
        public Vector3 PositionOffset;
        public Vector3 Velocity;
        public Vector3 normal;
        public float Lifetime;
        public Vector3 Color; // <--- Add this!
    }
    public class MeshPart
    {
        public ComputeBuffer buffer;
        public Material material;
        public int triangleCount;
    }
}