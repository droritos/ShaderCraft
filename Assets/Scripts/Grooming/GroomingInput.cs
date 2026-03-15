using System;
using Global_Data;
using Grooming;
using Manager;
using UnityEngine;

// This ensures our Input script always has access to the Tools and the GPU Manager
[RequireComponent(typeof(GroomingActions))]
[RequireComponent(typeof(GroomingGPUManager))]
public class GroomingInput : MonoBehaviour
{
    private GroomingActions _tools;
    private GroomingGPUManager _gpuManager;
    
    private ToolType _activeTool;

    #region << Unity Functions >>
    private void Awake()
    {
        // 1. Find the required components!
        if(!_tools)
            _tools = GetComponent<GroomingActions>();
        if(!_gpuManager)
            _gpuManager = GetComponent<GroomingGPUManager>();
        
        ToolBoxManager.Instance.OnToolSelected += (tool) => _activeTool = tool;
    }
    private void OnDestroy()
    {
        // Always unsubscribe (-) when this object is destroyed!
        if (ToolBoxManager.Instance != null)
        {
            ToolBoxManager.Instance.OnToolSelected -= (tool) => _activeTool = tool;
        }
    }
    private void Update()
    {
        // LEFT CLICK = SHAVE (Cut)
        if (Input.GetMouseButton(0))
        {
            //HandleMouseClick((part, index) => _tools.CutTriangle(part, index));
            HandleClickTool(_activeTool);
        }


        // PRESS R = RESET ALL
        if (Input.GetKeyDown(KeyCode.R))
        {
            _tools.ResetAll();
        }
    }
    #endregion

    // This shoots the laser from the mouse and triggers whatever tool we asked for
    private void HandleMouseClick(Action<MeshPart, int> toolAction)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            MeshPart hitPart = GetHitPart(hit);
            if (hitPart != null)
            {
                // Run the specific tool logic (Cut, Paint, or Grow)
                toolAction(hitPart, hit.triangleIndex);
            }
        }
    }

    // Finds which specific material/buffer we hit
    private MeshPart GetHitPart(RaycastHit hit)
    {
        // We search through the GPU Manager's public list of mesh parts
        return _gpuManager.MeshParts.Find(p => 
            p.material == hit.collider.GetComponent<MeshRenderer>().sharedMaterial || 
            p.material.name.Contains(hit.collider.GetComponent<MeshRenderer>().material.name.Replace(" (Instance)", ""))
        );
    }

    private void HandleClickTool(ToolType toolType)
    {
        switch (toolType)
        {
            case ToolType.Shave:
                HandleMouseClick((part, index) => _tools.CutTriangle(part, index));
                break;
            case ToolType.Grow:
                HandleMouseClick((part, index) => _tools.GrowTriangle(part, index));
                break;
            case ToolType.Color:
                HandleMouseClick((part, index) => _tools.PaintTriangle(part, index));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(toolType), toolType, null);
        }
    }
}