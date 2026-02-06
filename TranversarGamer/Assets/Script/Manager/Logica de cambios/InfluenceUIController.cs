using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InfluenceUIController : MonoBehaviour
{
    [Header("Referencias")]
    public SliderAnimado sliderAnimado;          // Valor de influencia (-100 a 100)
    public Text titulo;                          // Título dinámico
    public List<GameObject> botonesBuenos;       // Botones que aparecen en estado luminoso
    public List<GameObject> botonesMalos;        // Botones que aparecen en estado corrupto
    public List<GameObject> botonesNeutrales;    // Botones que aparecen en estado neutral

    [Header("Rangos de Estado")]
    public float limiteCorrupto = -30f;
    public float limiteLuminoso = 30f;

    public enum EstadoInfluencia
    {
        Corrupto,
        Neutral,
        Luminoso
    }

    public EstadoInfluencia estadoActual { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Persistente en toda la escena
    }

    void Update()
    {
        ActualizarEstado();
        ActualizarUI();
    }

    void ActualizarEstado()
    {
        float valor = sliderAnimado.valorObjetivo;

        if (valor <= limiteCorrupto)
            estadoActual = EstadoInfluencia.Corrupto;
        else if (valor >= limiteLuminoso)
            estadoActual = EstadoInfluencia.Luminoso;
        else
            estadoActual = EstadoInfluencia.Neutral;
    }

    void ActualizarUI()
    {
        // Ocultar todo
        SetLista(botonesBuenos, false);
        SetLista(botonesMalos, false);
        SetLista(botonesNeutrales, false);

        switch (estadoActual)
        {
            case EstadoInfluencia.Corrupto:
                titulo.text = "Influencia Corrupta";
                SetLista(botonesMalos, true);
                break;

            case EstadoInfluencia.Neutral:
                titulo.text = "Influencia Neutral";
                SetLista(botonesNeutrales, true);
                break;

            case EstadoInfluencia.Luminoso:
                titulo.text = "Influencia Lumínica";
                SetLista(botonesBuenos, true);
                break;
        }
    }

    void SetLista(List<GameObject> lista, bool estado)
    {
        foreach (var obj in lista)
        {
            if (obj != null)
                obj.SetActive(estado);
        }
    }
}
