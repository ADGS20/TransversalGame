using UnityEngine;
using System;

public class InfluenceState : MonoBehaviour
{
    public static InfluenceState Instance { get; private set; }

    public enum EstadoInfluencia { Corrupto, Neutral, Luminoso }

    [Header("Configuración")]
    public float minValue = -100f;
    public float maxValue = 100f;
    public float corruptThreshold = -30f;
    public float luminousThreshold = 30f;
    public string playerPrefsKey = "InfluenceValue";

    [SerializeField]
    float _value = 0f;

    public float Value
    {
        get => _value;
        private set
        {
            float nuevo = Mathf.Clamp(value, minValue, maxValue);
            if (Mathf.Approximately(nuevo, _value)) return;
            _value = nuevo;
            Save();
            OnValueChanged?.Invoke(_value);
            OnEstadoChanged?.Invoke(CurrentEstado);
        }
    }

    // Eventos: se disparan cuando cambia el valor o el estado
    public event Action<float> OnValueChanged;
    public event Action<EstadoInfluencia> OnEstadoChanged;

    public EstadoInfluencia CurrentEstado
    {
        get
        {
            if (_value <= corruptThreshold) return EstadoInfluencia.Corrupto;
            if (_value >= luminousThreshold) return EstadoInfluencia.Luminoso;
            return EstadoInfluencia.Neutral;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    void Load()
    {
        if (PlayerPrefs.HasKey(playerPrefsKey))
            _value = PlayerPrefs.GetFloat(playerPrefsKey);
        else
            _value = 0f;
    }

    void Save()
    {
        PlayerPrefs.SetFloat(playerPrefsKey, _value);
        PlayerPrefs.Save();
    }

    // API pública
    public void SetValue(float nuevoValor)
    {
        Value = Mathf.Clamp(nuevoValor, minValue, maxValue);
    }

    public void ModifyValue(float delta)
    {
        SetValue(Value + delta);
    }

    public void ResetValue(float nuevo = 0f)
    {
        SetValue(nuevo);
    }
}
