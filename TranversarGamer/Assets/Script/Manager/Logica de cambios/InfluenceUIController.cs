using UnityEngine;
using System.Collections.Generic; // Necesario para usar Listas

public class InfluenceUIController : MonoBehaviour
{
    [Header("Imágenes de Título (Uno solo)")]
    [Tooltip("Aparece cuando la influencia es menor a -33")]
    public GameObject tituloCorrupto;

    [Tooltip("Aparece cuando la influencia está entre -33 y 33")]
    public GameObject tituloNeutral;

    [Tooltip("Aparece cuando la influencia es mayor a 33")]
    public GameObject tituloSanado;

    [Header("Listas de Botones / Objetos por Estado")]
    [Tooltip("Arrastra aquí todos los botones/objetos del estado Corrupto")]
    public List<GameObject> botonesCorrupto = new List<GameObject>();

    [Tooltip("Arrastra aquí todos los botones/objetos del estado Neutral")]
    public List<GameObject> botonesNeutral = new List<GameObject>();

    [Tooltip("Arrastra aquí todos los botones/objetos del estado Sanado")]
    public List<GameObject> botonesSanado = new List<GameObject>();

    private void Start()
    {
        if (InfluenceState.Instance != null)
        {
            // 1. Actualizar nada más empezar
            ActualizarVisuales(InfluenceState.Instance.currentInfluence);

            // 2. Suscribirse para cambios futuros
            InfluenceState.Instance.OnEstadoChanged += ActualizarVisuales;
        }
    }

    private void OnDestroy()
    {
        if (InfluenceState.Instance != null)
            InfluenceState.Instance.OnEstadoChanged -= ActualizarVisuales;
    }

    public void ActualizarVisuales(float valor)
    {
        // --- 1. APAGAR TODO PRIMERO (Limpieza) ---
        // Títulos
        if (tituloCorrupto != null) tituloCorrupto.SetActive(false);
        if (tituloNeutral != null) tituloNeutral.SetActive(false);
        if (tituloSanado != null) tituloSanado.SetActive(false);

        // Listas de botones (Apagamos todas)
        ToggleLista(botonesCorrupto, false);
        ToggleLista(botonesNeutral, false);
        ToggleLista(botonesSanado, false);

        // --- 2. ENCENDER SEGÚN RANGO ---
        if (valor < -33f)
        {
            // ESTADO: CORRUPTO (-100 a -34)
            if (tituloCorrupto != null) tituloCorrupto.SetActive(true);
            ToggleLista(botonesCorrupto, true);
        }
        else if (valor > 33f)
        {
            // ESTADO: SANADO (34 a 100)
            if (tituloSanado != null) tituloSanado.SetActive(true);
            ToggleLista(botonesSanado, true);
        }
        else
        {
            // ESTADO: NEUTRAL (-33 a 33)
            if (tituloNeutral != null) tituloNeutral.SetActive(true);
            ToggleLista(botonesNeutral, true);
        }
    }

    // Función auxiliar para activar/desactivar una lista entera
    private void ToggleLista(List<GameObject> lista, bool estado)
    {
        if (lista == null) return;

        foreach (GameObject obj in lista)
        {
            if (obj != null)
            {
                obj.SetActive(estado);
            }
        }
    }
}