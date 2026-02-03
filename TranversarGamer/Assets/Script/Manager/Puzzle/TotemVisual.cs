using UnityEngine;

public class TotemVisual : MonoBehaviour
{
    [Header("Render del tótem")]
    public Renderer renderTotem;

    [Header("Colores")]
    public Color colorBase = Color.white;
    public Color colorNaturaleza = new Color(0.2f, 1f, 0.2f);
    public Color colorCorrupcion = new Color(0.7f, 0f, 0.7f);

    [Header("Brillo")]
    public float intensidadBrillo = 2f;

    private Material mat;

    void Start()
    {
        if (renderTotem == null)
            renderTotem = GetComponentInChildren<Renderer>();

        mat = renderTotem.material;

        Desactivar();
    }

    public void ActivarNaturaleza()
    {
        mat.color = colorNaturaleza;
        mat.SetColor("_EmissionColor", colorNaturaleza * intensidadBrillo);
        mat.EnableKeyword("_EMISSION");
    }

    public void ActivarCorrupcion()
    {
        mat.color = colorCorrupcion;
        mat.SetColor("_EmissionColor", colorCorrupcion * intensidadBrillo);
        mat.EnableKeyword("_EMISSION");
    }

    public void Desactivar()
    {
        mat.color = colorBase;
        mat.SetColor("_EmissionColor", Color.black);
        mat.DisableKeyword("_EMISSION");
    }
}
