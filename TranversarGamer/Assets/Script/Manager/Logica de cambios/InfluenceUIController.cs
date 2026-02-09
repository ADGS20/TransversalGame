using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InfluenceUIController : MonoBehaviour
{
    [Header("UI")]
    public Text titulo;

    [Header("Botones por estado")]
    public List<GameObject> botonesBuenos = new List<GameObject>();
    public List<GameObject> botonesMalos = new List<GameObject>();
    public List<GameObject> botonesNeutrales = new List<GameObject>();

    [Header("Títulos por estado (listas)")]
    [Tooltip("Lista de títulos posibles para estado Luminoso. Se elige por index o el primero si el index no existe.")]
    public List<string> titulosBuenos = new List<string>();
    [Tooltip("Lista de títulos posibles para estado Neutral.")]
    public List<string> titulosNeutrales = new List<string>();
    [Tooltip("Lista de títulos posibles para estado Corrupto.")]
    public List<string> titulosMalos = new List<string>();

    [Header("Selección de título")]
    [Tooltip("Índice usado para elegir el título dentro de la lista. Si está fuera de rango se usa el primero.")]
    public int tituloIndex = 0;
    [Tooltip("Si está activado, concatena todos los títulos de la lista separados por ' - ' en lugar de elegir uno.")]
    public bool concatenarTitulos = false;

    [Header("Textos por defecto (si las listas están vacías)")]
    public string textoCorrupto = "Influencia Corrupta";
    public string textoNeutral = "Influencia Neutral";
    public string textoLuminoso = "Influencia Lumínica";

    void OnEnable()
    {
        if (InfluenceState.Instance != null)
        {
            AplicarEstado(InfluenceState.Instance.CurrentEstado);
            InfluenceState.Instance.OnEstadoChanged += AplicarEstado;
        }
    }

    void OnDisable()
    {
        if (InfluenceState.Instance != null)
            InfluenceState.Instance.OnEstadoChanged -= AplicarEstado;
    }

    void AplicarEstado(InfluenceState.EstadoInfluencia estado)
    {
        // Ocultar todo primero
        SetLista(botonesBuenos, false);
        SetLista(botonesMalos, false);
        SetLista(botonesNeutrales, false);

        // Aplicar botones y título según estado
        switch (estado)
        {
            case InfluenceState.EstadoInfluencia.Corrupto:
                SetLista(botonesMalos, true);
                SetTitulo(GetTituloFromList(titulosMalos, textoCorrupto));
                break;

            case InfluenceState.EstadoInfluencia.Neutral:
                SetLista(botonesNeutrales, true);
                SetTitulo(GetTituloFromList(titulosNeutrales, textoNeutral));
                break;

            case InfluenceState.EstadoInfluencia.Luminoso:
                SetLista(botonesBuenos, true);
                SetTitulo(GetTituloFromList(titulosBuenos, textoLuminoso));
                break;
        }
    }

    string GetTituloFromList(List<string> lista, string fallback)
    {
        if (titulo == null) return fallback;

        if (lista == null || lista.Count == 0)
            return fallback;

        if (concatenarTitulos)
            return string.Join(" - ", lista);

        if (tituloIndex >= 0 && tituloIndex < lista.Count)
            return lista[tituloIndex];

        return lista[0];
    }

    void SetTitulo(string texto)
    {
        if (titulo != null)
            titulo.text = texto;
    }

    void SetLista(List<GameObject> lista, bool activo)
    {
        if (lista == null) return;
        foreach (var go in lista)
            if (go != null) go.SetActive(activo);
    }
}
