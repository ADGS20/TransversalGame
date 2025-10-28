using UnityEngine;

/// <summary>
/// Gestiona el cambio entre el jugador principal y el compañero
/// </summary>
public class GameplayManager : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Script de movimiento del jugador principal")]
    public MonoBehaviour scriptJugadorPrincipal; // Tu Mov_Player2D

    [Tooltip("Script del compañero")]
    public CompainController companionController;

    [Header("Configuración")]
    [Tooltip("Tecla para cambiar de personaje")]
    public KeyCode teclaCambio = KeyCode.Tab;

    [Tooltip("¿Se puede cambiar de personaje actualmente?")]
    public bool puedeCambiar = false;

    // Estado actual
    private bool controlandoCompanion = false;

    void Update()
    {
        // Solo permitir cambio si estamos en una zona especial
        if (puedeCambiar && Input.GetKeyDown(teclaCambio))
        {
            CambiarPersonaje();
        }
    }

    /// <summary>
    /// Alterna entre controlar al jugador principal y al compañero
    /// </summary>
    public void CambiarPersonaje()
    {
        controlandoCompanion = !controlandoCompanion;

        if (controlandoCompanion)
        {
            // Cambiar a controlar el compañero
            scriptJugadorPrincipal.enabled = false;
            companionController.ActivarControl();
            Debug.Log("Controlando al compañero");
        }
        else
        {
            // Cambiar a controlar el jugador principal
            scriptJugadorPrincipal.enabled = true;
            companionController.DesactivarControl();
            Debug.Log("Controlando al jugador principal");
        }
    }

    /// <summary>
    /// Habilitar la posibilidad de cambiar de personaje
    /// </summary>
    public void HabilitarCambio()
    {
        puedeCambiar = true;
    }

    /// <summary>
    /// Deshabilitar la posibilidad de cambiar de personaje
    /// </summary>
    public void DeshabilitarCambio()
    {
        puedeCambiar = false;

        // Asegurar que volvemos al jugador principal
        if (controlandoCompanion)
        {
            CambiarPersonaje();
        }
    }
}