using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class InfluenceSliderFix : MonoBehaviour
{
    public Slider slider; // si no lo arrastras, se obtiene automáticamente
    [Tooltip("Si true, forzar también Image.type = Filled y fillAmount = 1 en el Fill Image")]
    public bool forceImageFilled = false;

    void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError("[InfluenceSliderFix] No hay Slider en este GameObject.");
            enabled = false;
            return;
        }

        // Validar FillRect y HandleRect asignados
        if (slider.fillRect == null)
            Debug.LogWarning("[InfluenceSliderFix] Slider.fillRect no asignado en el inspector.");
        else
            FixRectTransform(slider.fillRect);

        if (slider.handleRect == null)
            Debug.LogWarning("[InfluenceSliderFix] Slider.handleRect no asignado en el inspector.");
        else
            FixHandleRect(slider.handleRect);

        // Forzar rango coherente
        slider.minValue = -100f;
        slider.maxValue = 100f;

        // Aplicar valor actual inmediatamente (evita artefactos)
        slider.value = Mathf.Clamp(slider.value, slider.minValue, slider.maxValue);

        if (forceImageFilled && slider.fillRect != null)
        {
            var img = slider.fillRect.GetComponent<Image>();
            if (img != null)
            {
                img.type = Image.Type.Filled;
                img.fillMethod = Image.FillMethod.Horizontal;
                img.fillOrigin = 0;
                img.fillAmount = 1f;
            }
        }

        Debug.Log($"[InfluenceSliderFix] Awake -> min={slider.minValue} max={slider.maxValue} value={slider.value}");
    }

    void FixRectTransform(RectTransform rt)
    {
        if (rt == null) return;

        // Estirar el Fill para cubrir todo el track: anchors 0..1 y offsets 0
        rt.anchorMin = new Vector2(0f, rt.anchorMin.y);
        rt.anchorMax = new Vector2(1f, rt.anchorMax.y);
        rt.offsetMin = new Vector2(0f, rt.offsetMin.y);
        rt.offsetMax = new Vector2(0f, rt.offsetMax.y);

        // Si hay LayoutElement o ContentSizeFitter, deshabilitarlos para el Fill
        var layout = rt.GetComponent<LayoutElement>();
        if (layout != null) layout.enabled = false;
        var csf = rt.GetComponent<ContentSizeFitter>();
        if (csf != null) csf.enabled = false;

        Debug.Log($"[InfluenceSliderFix] FixRectTransform aplicado a {rt.name}");
    }

    void FixHandleRect(RectTransform rt)
    {
        if (rt == null) return;

        // Asegurar que el handle no esté restringido por anchors extrañas
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        var layout = rt.GetComponent<LayoutElement>();
        if (layout != null) layout.enabled = false;
        var csf = rt.GetComponent<ContentSizeFitter>();
        if (csf != null) csf.enabled = false;

        Debug.Log($"[InfluenceSliderFix] FixHandleRect aplicado a {rt.name}");
    }
}
