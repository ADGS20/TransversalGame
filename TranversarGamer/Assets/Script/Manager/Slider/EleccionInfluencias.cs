//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

public class EleccionInfluencias : MonoBehaviour
{
    public SliderAnimado sliderAnimado;      // Controla la barra animada de influencia
    public GameObject canvasEleccion;        // Canvas que muestra las opciones de elección

    [Header("Control (se asignan solos en Start)")]
    public Mov_Player3D scriptMovimientoJugador;  // Referencia al jugador
    public GameplayManager gameplayManager;       // Referencia al gestor global

    private bool bloqueadoJugadorAntes = false;   // Guarda el estado previo del jugador
    private bool bloqueadoGlobalAntes = false;    // Guarda el estado previo del manager

    private void Start()
    {
        // Buscar automáticamente el script del jugador si no está asignado
        if (scriptMovimientoJugador == null)
        {
            // Intentar encontrarlo por tag
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                scriptMovimientoJugador = playerObj.GetComponent<Mov_Player3D>();
            }

            // Si no se encontró por tag, buscar cualquier instancia en la escena
            if (scriptMovimientoJugador == null)
            {
                scriptMovimientoJugador = Object.FindFirstObjectByType<Mov_Player3D>();
            }

            if (scriptMovimientoJugador == null)
            {
                Debug.LogWarning("EleccionInfluencias: No se encontró Mov_Player3D automáticamente.");
            }
        }

        // Buscar automáticamente el GameplayManager si no está asignado
        if (gameplayManager == null)
        {
            gameplayManager = Object.FindFirstObjectByType<GameplayManager>();
            if (gameplayManager == null)
            {
                Debug.LogWarning("EleccionInfluencias: No se encontró GameplayManager automáticamente.");
            }
        }
    }

    private void OnEnable()
    {
        // Bloquear controles del jugador mientras el menú está activo
        if (scriptMovimientoJugador != null)
        {
            bloqueadoJugadorAntes = scriptMovimientoJugador.controlesBloqueados;
            scriptMovimientoJugador.controlesBloqueados = true;
            scriptMovimientoJugador.ForzarIdle();
        }

        // Bloquear controles globales del juego
        if (gameplayManager != null)
        {
            bloqueadoGlobalAntes = gameplayManager.controlesGlobalBloqueados;
            gameplayManager.controlesGlobalBloqueados = true;
        }
    }

    private void OnDisable()
    {
        // Restaurar estado previo del jugador
        if (scriptMovimientoJugador != null)
        {
            scriptMovimientoJugador.controlesBloqueados = bloqueadoJugadorAntes;
        }

        // Restaurar estado previo del manager
        if (gameplayManager != null)
        {
            gameplayManager.controlesGlobalBloqueados = bloqueadoGlobalAntes;
        }
    }

    // Eleccion buena
    public void EleccionBuena()
    {
        InfluenceState.Instance.ModifyValue(30f);
        canvasEleccion.SetActive(false);
    }

    // Eleccion mala
    public void EleccionMala()
    {
        InfluenceState.Instance.ModifyValue(-30f);
        canvasEleccion.SetActive(false);
    }


}