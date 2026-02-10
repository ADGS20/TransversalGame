using UnityEngine;
using UnityEngine.UI;

public class InfluenceSliderSync : MonoBehaviour
{
    [Header("Referencias UI")]
    public Slider slider;       // Arrastra el componente Slider
    public Image fillImage;     // Arrastra la imagen "Fill"

    [Header("Configuración de Animación")]
    [Range(1f, 20f)]
    public float velocidadAnimacion = 5f;

    // Variables internas
    private float valorVisualActual;
    private float valorObjetivoReal;

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();

        // Auto-corrección de configuración
        if (slider != null)
        {
            slider.minValue = -100f;
            slider.maxValue = 100f;
            slider.wholeNumbers = false;
            // Quitamos la interactividad para que el usuario no pueda arrastrarla
            slider.interactable = false;

            // Desconectar control nativo de Unity para evitar parpadeos
            if (fillImage != null && slider.fillRect == fillImage.rectTransform)
            {
                slider.fillRect = null;
            }
        }
    }

    private void Start()
    {
        if (InfluenceState.Instance != null)
        {
            valorObjetivoReal = InfluenceState.Instance.currentInfluence;
            valorVisualActual = valorObjetivoReal; // Empezamos ya en el sitio

            InfluenceState.Instance.OnEstadoChanged += AlCambiarInfluencia;
        }
    }

    private void OnDestroy()
    {
        if (InfluenceState.Instance != null)
            InfluenceState.Instance.OnEstadoChanged -= AlCambiarInfluencia;
    }

    private void AlCambiarInfluencia(float nuevoValor)
    {
        valorObjetivoReal = nuevoValor;
    }

    private void Update()
    {
        if (slider == null) return;

        // 1. CÁLCULO: Acercar el valor visual al objetivo
        if (Mathf.Abs(valorVisualActual - valorObjetivoReal) > 0.01f)
        {
            valorVisualActual = Mathf.Lerp(valorVisualActual, valorObjetivoReal, Time.deltaTime * velocidadAnimacion);
        }
        else
        {
            valorVisualActual = valorObjetivoReal;
        }

        // 2. APLICACIÓN: Forzar SIEMPRE los valores visuales (Bloquea interferencias)
        ActualizarVisuales(valorVisualActual);
    }

    private void ActualizarVisuales(float valor)
    {
        // Mover la bolita (Handle)
        slider.value = valor;

        // Mover la barra de color (Fill) manualmente
        if (fillImage != null)
        {
            float rangoTotal = slider.maxValue - slider.minValue; // 200
            float porcentaje = (valor - slider.minValue) / rangoTotal;
            fillImage.fillAmount = porcentaje;
        }
    }
}