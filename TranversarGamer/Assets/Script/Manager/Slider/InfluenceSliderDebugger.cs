using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class InfluenceSliderDebugger : MonoBehaviour
{
    public Slider slider;

    void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }

    [ContextMenu("Log Slider Info")]
    public void LogInfo()
    {
        if (slider == null)
        {
            Debug.LogError("[InfluenceSliderDebugger] No hay Slider asignado.");
            return;
        }

        Debug.Log($"[SliderDebug] slider.value={slider.value} min={slider.minValue} max={slider.maxValue}");
        if (slider.fillRect != null)
        {
            var f = slider.fillRect;
            Debug.Log($"[SliderDebug] FillRect name={f.name} anchors=({f.anchorMin.x},{f.anchorMin.y})-({f.anchorMax.x},{f.anchorMax.y}) offsets=({f.offsetMin.x},{f.offsetMin.y})-({f.offsetMax.x},{f.offsetMax.y}) sizeDelta={f.sizeDelta}");
            var img = f.GetComponent<Image>();
            if (img != null) Debug.Log($"[SliderDebug] Fill Image type={img.type} preserveAspect={img.preserveAspect} fillAmount={img.fillAmount}");
        }
        else Debug.LogWarning("[SliderDebug] slider.fillRect es null");

        if (slider.handleRect != null)
        {
            var h = slider.handleRect;
            Debug.Log($"[SliderDebug] HandleRect name={h.name} anchors=({h.anchorMin.x},{h.anchorMin.y})-({h.anchorMax.x},{h.anchorMax.y}) offsets=({h.offsetMin.x},{h.offsetMin.y})-({h.offsetMax.x},{h.offsetMax.y}) sizeDelta={h.sizeDelta}");
        }
        else Debug.LogWarning("[SliderDebug] slider.handleRect es null");

        var parent = slider.transform.parent as RectTransform;
        if (parent != null)
        {
            Debug.Log($"[SliderDebug] ParentRect name={parent.name} anchors=({parent.anchorMin.x},{parent.anchorMin.y})-({parent.anchorMax.x},{parent.anchorMax.y}) sizeDelta={parent.sizeDelta} rect.width={parent.rect.width}");
        }

        // Detectar Layout/Mask que puedan limitar
        var layout = slider.GetComponentInParent<UnityEngine.UI.LayoutGroup>();
        if (layout != null) Debug.LogWarning($"[SliderDebug] LayoutGroup en padre: {layout.GetType().Name} puede limitar tamaño.");
        var mask = slider.GetComponentInParent<Mask>();
        if (mask != null) Debug.LogWarning($"[SliderDebug] Mask en padre: {mask.name} puede recortar Fill.");
    }
}
