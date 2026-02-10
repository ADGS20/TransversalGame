using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class InfluenceSliderForceFill : MonoBehaviour
{
    public Slider slider;
    public bool forceFilledImage = true;

    void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
        if (slider == null) { enabled = false; return; }

        // Forzar rango coherente
        slider.minValue = -100f;
        slider.maxValue = 100f;

        if (slider.fillRect != null && forceFilledImage)
        {
            var img = slider.fillRect.GetComponent<Image>();
            if (img != null)
            {
                img.type = Image.Type.Filled;
                img.fillMethod = Image.FillMethod.Horizontal;
                img.fillOrigin = 0;
                img.fillAmount = slider.normalizedValue;
                Debug.Log("[InfluenceSliderForceFill] Forzado Image.Type = Filled en Fill");
            }
        }
    }

    void Update()
    {
        if (slider == null || slider.fillRect == null) return;
        var img = slider.fillRect.GetComponent<Image>();
        if (img != null && img.type == Image.Type.Filled)
            img.fillAmount = slider.normalizedValue;
    }
}
