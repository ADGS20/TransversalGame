//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;
using UnityEngine.UI;

public class SliderAnimado : MonoBehaviour
{
    public Slider slider;                 // Referencia al componente Slider
    public float duracionTransicion = 0.5f; // Tiempo que tarda en alcanzar el valor objetivo

    [HideInInspector]
    public float valorObjetivo;           // Valor al que debe llegar el slider de forma suave

    private float velocidad = 0f;         // Velocidad usada por SmoothDamp

    void Awake()
    {
        // Si no se asignó manualmente, se obtiene el Slider del mismo objeto
        if (slider == null)
            slider = GetComponent<Slider>();

        // El valor objetivo inicial es el valor actual del slider
        valorObjetivo = slider.value;
    }

    void Update()
    {
        // Si la diferencia es significativa, suavizar el movimiento
        if (Mathf.Abs(slider.value - valorObjetivo) > 0.01f)
        {
            slider.value = Mathf.SmoothDamp(
                slider.value,
                valorObjetivo,
                ref velocidad,
                duracionTransicion
            );
        }
        else
        {
            // Si ya está cerca, fijar el valor exacto
            slider.value = valorObjetivo;
        }
    }

    // Cambia el valor objetivo del slider dentro de sus límites
    public void SetValor(float nuevoValor)
    {
        valorObjetivo = Mathf.Clamp(nuevoValor, slider.minValue, slider.maxValue);
    }

    // Suma o resta un valor al objetivo
    public void SumarValor(float cantidad)
    {
        SetValor(valorObjetivo + cantidad);
    }
}
