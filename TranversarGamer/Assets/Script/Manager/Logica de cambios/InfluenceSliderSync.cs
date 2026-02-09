using UnityEngine;

[RequireComponent(typeof(SliderAnimado))]
public class InfluenceSliderSync : MonoBehaviour
{
    SliderAnimado sliderAnimado;

    void Awake()
    {
        sliderAnimado = GetComponent<SliderAnimado>();
    }

    void OnEnable()
    {
        if (InfluenceState.Instance != null)
        {
            sliderAnimado.SetValor(InfluenceState.Instance.Value);
            InfluenceState.Instance.OnValueChanged += OnInfluenceChanged;
        }
    }

    void OnDisable()
    {
        if (InfluenceState.Instance != null)
            InfluenceState.Instance.OnValueChanged -= OnInfluenceChanged;
    }

    void OnInfluenceChanged(float nuevo)
    {
        sliderAnimado.SetValor(nuevo);
    }
}
