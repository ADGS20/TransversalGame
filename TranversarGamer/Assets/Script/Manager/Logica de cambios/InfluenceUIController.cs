using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InfluenceUIController : MonoBehaviour
{
    [Header("UI")]
    public Text titulo; // si quieres mantener un texto además de la imagen (opcional)

    [Header("Botones por estado")]
    public List<GameObject> botonesBuenos = new List<GameObject>();
    public List<GameObject> botonesMalos = new List<GameObject>();
    public List<GameObject> botonesNeutrales = new List<GameObject>();

    [Header("Imágenes de título por estado (arrastrar Image components)")]
    public List<Image> titulosBuenos = new List<Image>();
    public List<Image> titulosNeutrales = new List<Image>();
    public List<Image> titulosMalos = new List<Image>();

    [Header("Selección de título")]
    public int tituloIndex = 0;
    public bool concatenarTitulos = false; // si quieres mostrar varias imágenes a la vez

    [Header("Textos por defecto (si quieres usar texto además de imagen)")]
    public string textoCorrupto = "Influencia Corrupta";
    public string textoNeutral = "Influencia Neutral";
    public string textoLuminoso = "Influencia Lumínica";


    void Start()
    {
        SetImagenes(titulosBuenos, false);
        SetImagenes(titulosNeutrales, false);
        SetImagenes(titulosMalos, false);

        if (InfluenceState.Instance != null)
            AplicarEstado(InfluenceState.Instance.CurrentEstado);
        else
            InfluenceState.EnsureInstance(); // opcional: crear para sincronizar
    }



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
        Debug.Log($"[InfluenceUIController] AplicarEstado: {estado}");

        SetLista(botonesBuenos, false);
        SetLista(botonesMalos, false);
        SetLista(botonesNeutrales, false);

        // Ocultar todas las imágenes de título primero
        SetImagenes(titulosBuenos, false);
        SetImagenes(titulosNeutrales, false);
        SetImagenes(titulosMalos, false);

        switch (estado)
        {
            case InfluenceState.EstadoInfluencia.Corrupto:
                SetLista(botonesMalos, true);
                MostrarTitulos(titulosMalos, textoCorrupto);
                break;

            case InfluenceState.EstadoInfluencia.Neutral:
                SetLista(botonesNeutrales, true);
                MostrarTitulos(titulosNeutrales, textoNeutral);
                break;

            case InfluenceState.EstadoInfluencia.Luminoso:
                SetLista(botonesBuenos, true);
                MostrarTitulos(titulosBuenos, textoLuminoso);
                break;
        }
    }

    void MostrarTitulos(List<Image> lista, string fallbackText)
    {
        if (lista == null || lista.Count == 0)
        {
            if (titulo != null) titulo.text = fallbackText;
            return;
        }

        if (concatenarTitulos)
        {
            // activar todas las imágenes de la lista
            SetImagenes(lista, true);
            if (titulo != null) titulo.text = ""; // opcional
            return;
        }

        int idx = (tituloIndex >= 0 && tituloIndex < lista.Count) ? tituloIndex : 0;
        SetImagenes(lista, false);
        if (lista[idx] != null) lista[idx].gameObject.SetActive(true);

        if (titulo != null) titulo.text = ""; // opcional: dejar texto vacío si se usa imagen
    }

    void SetImagenes(List<Image> lista, bool activo)
    {
        if (lista == null) return;
        foreach (var img in lista)
            if (img != null) img.gameObject.SetActive(activo);
    }

    void SetLista(List<GameObject> lista, bool activo)
    {
        if (lista == null) return;
        foreach (var go in lista)
            if (go != null) go.SetActive(activo);
    }
}
