
using UnityEngine;

namespace PublicObjects
{
    struct TriangleData
    {
        public Vector3 PositionOffset;
        public Vector3 Velocity;
        public float Lifetime;
    }
    public class MeshPart
    {
        public ComputeBuffer buffer;
        public Material material;
        public int triangleCount;
    }
}