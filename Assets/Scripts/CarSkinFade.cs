using UnityEngine;

public class CarSkinFade : MonoBehaviour
{
    public float fadeSpeed = 1.0f;
    public float targetAlpha = 1.0f;
    private Material skinMaterial;
    private float currentAlpha = 1.0f;

    void Start()
    {
        // Get the material from the renderer
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            skinMaterial = renderer.material;
            currentAlpha = skinMaterial.GetFloat("_Alpha");
        }
    }

    void Update()
    {
        if (skinMaterial != null)
        {
            // Smoothly interpolate current alpha towards target alpha
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
            skinMaterial.SetFloat("_Alpha", currentAlpha);
        }
    }

    // Public method to set target alpha
    public void SetTargetAlpha(float alpha)
    {
        targetAlpha = Mathf.Clamp01(alpha);
    }

    // Public method to fade in
    public void FadeIn()
    {
        SetTargetAlpha(1.0f);
    }

    // Public method to fade out
    public void FadeOut()
    {
        SetTargetAlpha(0.0f);
    }
} 