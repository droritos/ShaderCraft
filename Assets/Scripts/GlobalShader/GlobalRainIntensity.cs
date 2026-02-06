
using UnityEngine;

[ExecuteAlways]
public class GlobalRainIntensity : MonoBehaviour
{
    [Range(0f, 1f)]
    public float rainIntensity = 0.2f;

    private void OnValidate()
    {
        Shader.SetGlobalFloat("_RainIntensity", rainIntensity);
    }

    private void OnEnable()
    {
        // Ensures it applies when entering Play Mode or enabling object
        Shader.SetGlobalFloat("_RainIntensity", rainIntensity);
    }
}
