using UnityEngine;

public class Cube : MonoBehaviour
{ 
    public Color color;
    private SpriteRenderer sr;

    public int cubeIndex { get; set; }
    public byte cubeType { get; set; }

    public byte moisture { get; set; }
    public byte drainage { get; set; }

    public byte mineral { get; set; }

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
    }

    public void UpdateColor()
    {
        if (moisture > 0) sr.color = new Color(color.r - 0.15f, color.g - 0.15f, color.b - 0.15f);
        else sr.color = color;
    }
}
