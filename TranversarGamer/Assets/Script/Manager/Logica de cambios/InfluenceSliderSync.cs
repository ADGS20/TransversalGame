using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (sliderAnimado == null)
        {
            Debug.LogWarning("[InfluenceSliderSync] No hay SliderAnimado en este GameObject.");
            return;
        }

        if (InfluenceState.Instance != null)
        {
            sliderAnimado.SetValor(InfluenceState.Instance.Value);
            InfluenceState.Instance.OnValueChanged += OnInfluenceChanged;
        }
        else
        {
            Debug.LogWarning("[InfluenceSliderSync] InfluenceState.Instance es null al habilitar.");
        }
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (InfluenceState.Instance != null)
            InfluenceState.Instance.OnValueChanged -= OnInfluenceChanged;
    }

    void OnInfluenceChanged(float nuevo)
    {
        if (sliderAnimado == null) return;
        sliderAnimado.SetValor(nuevo);
    }

    // Si la escena se carga y este objeto persiste o se crea después, forzamos sincronía
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (sliderAnimado == null) return;

        if (InfluenceState.Instance != null)
        {
            sliderAnimado.SetValor(InfluenceState.Instance.Value);
        }
    }
}
