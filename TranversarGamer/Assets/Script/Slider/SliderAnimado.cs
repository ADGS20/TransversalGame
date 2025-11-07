using UnityEngine;
using UnityEngine.UI;

public class SliderAnimado : MonoBehaviour
{
    public Slider slider;
    public float duracionTransicion = 0.5f;

    [HideInInspector] public float valorObjetivo; // El valor real al que debe llegar el slider

    private float velocidad = 0f;

    void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
        valorObjetivo = slider.value;
    }

    void Update()
    {
        if (Mathf.Abs(slider.value - valorObjetivo) > 0.01f)
        {
            // Puedes usar Mathf.SmoothDamp para una transición suave
            slider.value = Mathf.SmoothDamp(slider.value, valorObjetivo, ref velocidad, duracionTransicion);
        }
        else
        {
            slider.value = valorObjetivo;
        }
    }

    // Llama a este método para cambiar el valor de la barra
    public void SetValor(float nuevoValor)
    {
        valorObjetivo = Mathf.Clamp(nuevoValor, slider.minValue, slider.maxValue);
    }

    // Si quieres sumar/restar
    public void SumarValor(float cantidad)
    {
        SetValor(valorObjetivo + cantidad);
    }
}