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

    [Header("Inspector")]
    [Tooltip("Si está activo, al arrancar se reinicia el valor guardado a 0 (modo 'nuevo').")]
    public bool resetOnStart = false;

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
            Debug.Log($"[InfluenceState] Nuevo valor: {_value} (estado: {CurrentEstado})");

            Save();
            OnValueChanged?.Invoke(_value);
            OnEstadoChanged?.Invoke(CurrentEstado);
        }
    }

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

        if (resetOnStart)
        {
            Debug.Log("[InfluenceState] resetOnStart activo: borrando valor guardado y estableciendo 0");
            ClearSavedValue();
            _value = 0f;
            // Emitir eventos para sincronizar listeners que ya existan
            OnValueChanged?.Invoke(_value);
            OnEstadoChanged?.Invoke(CurrentEstado);
        }
        else
        {
            Load();
        }
    }

    void Load()
    {
        if (PlayerPrefs.HasKey(playerPrefsKey))
        {
            _value = PlayerPrefs.GetFloat(playerPrefsKey);
            Debug.Log($"[InfluenceState] Cargado desde PlayerPrefs: {_value} (estado: {CurrentEstado})");
        }
        else
        {
            _value = 0f;
            Debug.Log("[InfluenceState] Sin valor previo, inicializando a 0");
        }
    }

    void Save()
    {
        PlayerPrefs.SetFloat(playerPrefsKey, _value);
        PlayerPrefs.Save();
        Debug.Log($"[InfluenceState] Guardado en PlayerPrefs: {_value}");
    }

    [ContextMenu("Clear Saved Influence")]
    public void ClearSavedValue()
    {
        if (PlayerPrefs.HasKey(playerPrefsKey))
        {
            PlayerPrefs.DeleteKey(playerPrefsKey);
            PlayerPrefs.Save();
            Debug.Log("[InfluenceState] PlayerPrefs: clave borrada");
        }
        else
        {
            Debug.Log("[InfluenceState] PlayerPrefs: no existía clave para borrar");
        }
    }

    // API pública
    public void SetValue(float nuevoValor)
    {
        Debug.Log($"[InfluenceState] SetValue({nuevoValor})");
        Value = Mathf.Clamp(nuevoValor, minValue, maxValue);
    }

    public void ModifyValue(float delta)
    {
        Debug.Log($"[InfluenceState] ModifyValue({delta}) desde {_value}");
        SetValue(Value + delta);
    }

    public void ResetValue(float nuevo = 0f)
    {
        Debug.Log($"[InfluenceState] ResetValue({nuevo})");
        SetValue(nuevo);
    }

    // Añadir dentro de InfluenceState (clase existente)
    public static InfluenceState EnsureInstance()
    {
        if (Instance != null) return Instance;

        // Buscar en la escena una instancia existente
        InfluenceState found = UnityEngine.Object.FindObjectOfType<InfluenceState>();
        if (found != null)
        {
            Instance = found;
            UnityEngine.Object.DontDestroyOnLoad(Instance.gameObject);
            Instance.Load(); // cargar valor guardado
            Debug.Log("[InfluenceState] EnsureInstance: encontrada instancia en escena");
            return Instance;
        }

        // Crear nueva si no existe
        GameObject go = new GameObject("InfluenceState");
        Instance = go.AddComponent<InfluenceState>();
        UnityEngine.Object.DontDestroyOnLoad(go);
        Instance.Load();
        Debug.Log("[InfluenceState] EnsureInstance: creada dinámicamente");
        return Instance;
    }


}
