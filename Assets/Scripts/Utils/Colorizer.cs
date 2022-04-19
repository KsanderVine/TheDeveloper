using UnityEngine;
using System.Collections;

public class Colorizer : MonoBehaviour
{
    public Material Material;
    public SpriteRenderer Renderer;
    public Color Red = new Color(0, 0, 1, 1);
    public Color Green = new Color(0, 0, 1, 1);
    public Color Blue = new Color(0, 1, 0, 1);

    private void ColorTint()
    {
        if (Renderer != null)
        {
            Material tempMaterial = new Material(Material);
            tempMaterial.SetColor("_TintColorRed", Red);
            tempMaterial.SetColor("_TintColorGreen", Green);
            tempMaterial.SetColor("_TintColorBlue", Blue);
            Renderer.material = tempMaterial;
        }
    }

    protected void OnEnable()
    {
        ColorTint();
    }
}