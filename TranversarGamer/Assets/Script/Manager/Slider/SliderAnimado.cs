using UnityEngine;
using UnityEngine.UI;

public class SliderAnimado : MonoBehaviour
{
    public Slider slider;
    public float duracionTransicion = 0.5f;

    [HideInInspector]
    public float valorObjetivo;

    private float velocidad = 0f;

    void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>() ?? GetComponentInChildren<Slider>();

        if (slider == null)
        {
            Debug.LogWarning("[SliderAnimado] No se encontró Slider en este GameObject. Este componente quedará inactivo hasta que se asigne un Slider.");
            return;
        }

        slider.minValue = -100f;
        slider.maxValue = 100f;
        valorObjetivo = slider.value;
        Debug.Log($"[SliderAnimado] Awake -> slider.min={slider.minValue} max={slider.maxValue} value={slider.value}");
    }


    void Update()
    {
        if (slider == null) return;

        // Debug para ver si algo limita el valor visual
        // (puedes comentar estas líneas cuando ya esté todo correcto)
        // Debug.Log($"[SliderAnimado] Update -> slider.value={slider.value} objetivo={valorObjetivo}");

        if (Mathf.Abs(slider.value - valorObjetivo) > 0.01f)
        {
            slider.value = Mathf.SmoothDamp(slider.value, valorObjetivo, ref velocidad, duracionTransicion);
        }
        else
        {
            slider.value = valorObjetivo;
            velocidad = 0f;
        }
    }

    // Cambia el objetivo y aplica inmediatamente al slider (evita quedarse a mitad)
    public void SetValor(float nuevoValor)
    {
        float clamped = Mathf.Clamp(nuevoValor, -100f, 100f);
        valorObjetivo = clamped;

        if (slider == null)
        {
            Debug.Log($"[SliderAnimado] SetValor guardado objetivo (no hay Slider): {valorObjetivo}");
            return;
        }

        // Aplicar inmediatamente el valor final para evitar artefactos visuales
        slider.value = valorObjetivo;
        velocidad = 0f;

        Debug.Log($"[SliderAnimado] SetValor -> aplicado slider.value={slider.value} objetivo={valorObjetivo}");
    }

    public void SumarValor(float cantidad)
    {
        SetValor(valorObjetivo + cantidad);
    }
}
