using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject panelDialogo; // El objeto que contiene el fondo
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoCuerpo;
    public Image imagenIcono;

    [Header("Ajustes")]
    public float velocidadEscritura = 0.025f;

    private Queue<LineaDialogo> lineas;
    public bool estaHablando = false;
    private bool estaEscribiendo = false;
    private string mensajeCompleto = "";
    private Coroutine corrutinaTexto;

    void Awake()
    {
        if (Instance == null) Instance = this;
        lineas = new Queue<LineaDialogo>();

        // SEGURO TOTAL: Deshabilitamos el panel nada más arrancar el motor
        if (panelDialogo != null) panelDialogo.SetActive(false);
    }

    public void IniciarDialogo(LineaDialogo[] nuevasLineas)
    {
        if (nuevasLineas.Length == 0) return;

        // Si el panel estaba apagado, lo encendemos. Si estaba encendido, se queda encendido.
        if (panelDialogo != null) panelDialogo.SetActive(true);

        lineas.Clear();
        foreach (LineaDialogo linea in nuevasLineas) lineas.Enqueue(linea);

        estaHablando = true;
        BloquearControles(true);
        MostrarSiguienteLinea();
    }

    void Update()
    {
        // Detectar entrada para pasar texto
        if (estaHablando && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)))
        {
            if (estaEscribiendo)
            {
                StopCoroutine(corrutinaTexto);
                textoCuerpo.text = mensajeCompleto;
                estaEscribiendo = false;
            }
            else
            {
                MostrarSiguienteLinea();
            }
        }
    }

    public void MostrarSiguienteLinea()
    {
        if (lineas.Count == 0)
        {
            FinalizarDialogo();
            return;
        }

        LineaDialogo lineaActual = lineas.Dequeue();
        textoNombre.text = lineaActual.nombrePersonaje;
        imagenIcono.sprite = lineaActual.iconoPersonaje;
        mensajeCompleto = lineaActual.texto;

        if (corrutinaTexto != null) StopCoroutine(corrutinaTexto);
        corrutinaTexto = StartCoroutine(EfectoMaquinaDeEscribir(lineaActual.texto));
    }

    IEnumerator EfectoMaquinaDeEscribir(string texto)
    {
        textoCuerpo.text = "";
        estaEscribiendo = true;

        foreach (char letra in texto.ToCharArray())
        {
            textoCuerpo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        estaEscribiendo = false;
    }

    void FinalizarDialogo()
    {
        estaHablando = false;

        // SEGURO TOTAL: Desactivamos el panel al terminar los mensajes
        if (panelDialogo != null) panelDialogo.SetActive(false);

        BloquearControles(false);
    }

    void BloquearControles(bool bloquear)
    {
        var gm = FindObjectOfType<GameplayManager>();
        if (gm != null) gm.controlesGlobalBloqueados = bloquear;

        var player = FindObjectOfType<Mov_Player3D>();
        if (player != null) player.controlesBloqueados = bloquear;
    }
}