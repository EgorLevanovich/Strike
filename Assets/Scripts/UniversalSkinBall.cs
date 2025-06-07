using UnityEngine;

public class UniversalSkinBall : MonoBehaviour
{
    public float fadeSpeed = 1.0f;
    public float targetAlpha = 1.0f;
    private Material skinMaterial;
    private float currentAlpha = 1.0f;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Create a new material instance to avoid modifying the shared material
            skinMaterial = new Material(spriteRenderer.material);
            spriteRenderer.material = skinMaterial;
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

    // Method to set the skin texture
    public void SetSkinTexture(Texture2D newTexture)
    {
        if (skinMaterial != null && newTexture != null)
        {
            skinMaterial.SetTexture("_MainTex", newTexture);
        }
    }

    // Method to set the skin color
    public void SetSkinColor(Color newColor)
    {
        if (skinMaterial != null)
        {
            skinMaterial.SetColor("_Color", newColor);
        }
    }
} 