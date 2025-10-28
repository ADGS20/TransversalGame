using UnityEngine;

/// <summary>
/// Gestiona el cambio entre el jugador principal y el companero
/// </summary>
public class GameplayManager : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Script de movimiento del jugador principal")]
    public MonoBehaviour scriptJugadorPrincipal; // Tu Mov_Player3D

    [Tooltip("Script del compañero")]
    public CompainController companionController;

    [Header("Configuración")]
    [Tooltip("Tecla para cambiar de personaje")]
    public KeyCode teclaCambio = KeyCode.Tab;

    [Tooltip("¿Se puede cambiar de personaje actualmente?")]
    public bool puedeCambiar = false;

    [Header("Cámara (Opcional)")]
    [Tooltip("Cámara que seguirá al personaje activo")]
    public Transform camaraTransform;

    [Tooltip("Offset de la cámara respecto al personaje")]
    public Vector3 offsetCamara = new Vector3(0, 10, -10);

    [Tooltip("Velocidad de seguimiento de la cámara")]
    public float velocidadCamara = 5f;

    // Estado actual
    private bool controlandoCompanion = false;
    private Transform objetivoActual;

    void Start()
    {
        // Configurar cámara si no está asignada
        if (camaraTransform == null)
        {
            camaraTransform = Camera.main.transform;
        }

        // El objetivo inicial es el jugador principal
        objetivoActual = scriptJugadorPrincipal.transform;
    }

    void Update()
    {
        // Solo permitir cambio si estamos en una zona especial
        if (puedeCambiar && Input.GetKeyDown(teclaCambio))
        {
            CambiarPersonaje();
        }

        // Actualizar posición de la cámara
        if (camaraTransform != null && objetivoActual != null)
        {
            Vector3 posicionObjetivo = objetivoActual.position + offsetCamara;
            camaraTransform.position = Vector3.Lerp(camaraTransform.position, posicionObjetivo, velocidadCamara * Time.deltaTime);
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
            objetivoActual = companionController.transform;
            Debug.Log("Controlando al compañero");
        }
        else
        {
            // Cambiar a controlar el jugador principal
            scriptJugadorPrincipal.enabled = true;
            companionController.DesactivarControl();
            objetivoActual = scriptJugadorPrincipal.transform;
            Debug.Log("Controlando al jugador principal");
        }
    }

    /// <summary>
    /// Habilitar la posibilidad de cambiar de personaje
    /// </summary>
    public void HabilitarCambio()
    {
        puedeCambiar = true;
        Debug.Log("Zona de cambio activada. Presiona Tab para alternar.");
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