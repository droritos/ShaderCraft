using Global_Data;
using Grooming;
using UnityEngine;
using Manager; // Needed to talk to your ToolBoxManager

[RequireComponent(typeof(GroomingGPUManager))]
public class GroomingActions : MonoBehaviour
{
    private GroomingGPUManager _gpuManager;
    private Color _activeColor = Color.white;
    
    [Header("Random Settings")] 
    [SerializeField] private float minXZ;
    [SerializeField] private float maxXZ;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private void Awake()
    {
        // 1. Grab the GPU manager so we can access its data
        _gpuManager = GetComponent<GroomingGPUManager>();
        
        // 2. Subscribe to your UI Color Picker
        if (ToolBoxManager.Instance != null)
        {
            ToolBoxManager.Instance.OnColorSelected -= SetColor; 
            ToolBoxManager.Instance.OnColorSelected += SetColor; 
        }
    }

    private void SetColor(Color color)
    {
        _activeColor = color;
    }

    public void CutTriangle(MeshPart part, int index)
    {
        TriangleData[] data = new TriangleData[1];
        part.buffer.GetData(data, 0, index, 1);

        if (data[0].Lifetime >= 3.0f)
        {
            data[0].Lifetime = 2.99f; 
            part.buffer.SetData(data, 0, index, 1);
        }
    }

    public void PaintTriangle(MeshPart part, int index)
    {
        TriangleData[] data = new TriangleData[1];
        part.buffer.GetData(data, 0, index, 1);

        // Uses the color selected from the ToolBoxManager UI!
        data[0].Color = new Vector3(_activeColor.r, _activeColor.g, _activeColor.b);
        
        part.buffer.SetData(data, 0, index, 1);
    }

    public void GrowTriangle(MeshPart part, int index)
    {
        TriangleData[] data = new TriangleData[1];
        part.buffer.GetData(data, 0, index, 1);

        if (data[0].Lifetime < 3.0f)
        {
            data[0].PositionOffset = Vector3.zero;
            data[0].Lifetime = 3.0f;
            part.buffer.SetData(data, 0, index, 1);
        }
    }

    public void ResetAll()
    {
        // We use the public 'meshParts' list we created in the GPU Manager
        foreach (MeshPart part in _gpuManager.meshParts)
        {
            TriangleData[] resetData = new TriangleData[part.triangleCount];
            for (int i = 0; i < part.triangleCount; i++)
            {
                resetData[i].PositionOffset = Vector3.zero;
                
                // Keep the flick velocity random so it explodes differently next time
                resetData[i].Velocity = new Vector3(
                    UnityEngine.Random.Range(minXZ, maxXZ), 
                    UnityEngine.Random.Range(minY, maxY), 
                    UnityEngine.Random.Range(minXZ, maxXZ)
                );
                
                resetData[i].Lifetime = 3.0f;
                resetData[i].Color = Vector3.one; 
            }
            part.buffer.SetData(resetData);
        }
    }
}