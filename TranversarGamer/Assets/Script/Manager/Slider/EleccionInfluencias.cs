//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

public class EleccionInfluencias : MonoBehaviour
{
    public SliderAnimado sliderAnimado;
    public GameObject canvasEleccion;

    [Header("Control (se asignan solos en Start)")]
    public Mov_Player3D scriptMovimientoJugador;  // Jugador
    public GameplayManager gameplayManager;       // Manager global

    private bool bloqueadoJugadorAntes = false;
    private bool bloqueadoGlobalAntes = false;

    private void Start()
    {
        // Buscar automáticamente el jugador si no está asignado
        if (scriptMovimientoJugador == null)
        {
            // Primero intento por tag "Player"
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                scriptMovimientoJugador = playerObj.GetComponent<Mov_Player3D>();
            }

            // Si no lo encuentra por tag, busca cualquier Mov_Player3D en la escena
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
        // Bloqueo del jugador
        if (scriptMovimientoJugador != null)
        {
            bloqueadoJugadorAntes = scriptMovimientoJugador.controlesBloqueados;
            scriptMovimientoJugador.controlesBloqueados = true;
            scriptMovimientoJugador.ForzarIdle();
        }

        // Bloqueo global del gameplay manager
        if (gameplayManager != null)
        {
            bloqueadoGlobalAntes = gameplayManager.controlesGlobalBloqueados;
            gameplayManager.controlesGlobalBloqueados = true;
        }
    }

    private void OnDisable()
    {
        // Restaurar bloqueo del jugador
        if (scriptMovimientoJugador != null)
        {
            scriptMovimientoJugador.controlesBloqueados = bloqueadoJugadorAntes;
        }

        // Restaurar bloqueo global
        if (gameplayManager != null)
        {
            gameplayManager.controlesGlobalBloqueados = bloqueadoGlobalAntes;
        }
    }

    public void EleccionBuena()
    {
        if (sliderAnimado != null)
            sliderAnimado.SumarValor(30);

        canvasEleccion.SetActive(false);
    }

    public void EleccionMala()
    {
        if (sliderAnimado != null)
            sliderAnimado.SumarValor(-30);

        canvasEleccion.SetActive(false);
    }
}