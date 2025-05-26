using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraGrainEffect : MonoBehaviour
{
    public Material grainMaterial;

    [Range(0f, 1f)]
    public float intensity = 0.5f;

    [Range(0.1f, 100f)]
    public float grainSize = 1f;

    [Range(0f, 10f)]
    public float speed = 1f;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (grainMaterial != null)
        {
            grainMaterial.SetFloat("_Intensity", intensity);
            grainMaterial.SetFloat("_GrainSize", grainSize);
            grainMaterial.SetFloat("_Speed", speed);
            Graphics.Blit(source, destination, grainMaterial);
        }
        else
        {
            Graphics.Blit(source, destination); // fallback
        }
    }
}
