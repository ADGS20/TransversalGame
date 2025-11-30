//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

/// <summary>
/// Gestiona el cambio entre el jugador principal y el compañero en 2.5D
/// CALCULA EL MOVIMIENTO RELATIVO A LA CÁMARA
/// </summary>
public class GameplayManager : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Script de movimiento del jugador principal")]
    public Mov_Player3D scriptJugadorPrincipal;

    [Tooltip("Rigidbody del jugador principal")]
    public Rigidbody rbJugadorPrincipal;

    [Tooltip("Script del compañero")]
    public CompainController companionController;

    [Tooltip("Rigidbody del compañero")]
    public Rigidbody rbCompanion;

    [Header("Configuración")]
    [Tooltip("Tecla para cambiar de personaje")]
    public KeyCode teclaCambio = KeyCode.Tab;

    [Tooltip("Tecla para activar seguimiento de la mascota")]
    public KeyCode teclaActivarSeguimiento = KeyCode.X;

    [Tooltip("¿Se puede cambiar de personaje actualmente?")]
    public bool puedeCambiar = true;

    [Header("Cámara Orbital")]
    [Tooltip("Script de cámara orbital (se busca automáticamente si no se asigna)")]
    public CameraOrbital camaraOrbital;

    [Header("Estado global de control")]
    public bool controlesGlobalBloqueados = false;

    // Estado actual
    private bool controlandoCompanion = false;
    private Transform objetivoActual;

    // Estado para recordar si estaba permitido el cambio y a quién controlábamos
    private bool estadoPrevioPuedeCambiar;
    private bool estadoPrevioControlandoCompanion;

    void Start()
    {
        // Buscar cámara orbital si no está asignada
        if (camaraOrbital == null)
        {
            camaraOrbital = Camera.main.GetComponent<CameraOrbital>();
            if (camaraOrbital == null)
            {
                Debug.LogError("❌ No se encontró CameraOrbital en la Main Camera");
                return;
            }
        }

        // Obtener Rigidbody del jugador si no está asignado
        if (rbJugadorPrincipal == null && scriptJugadorPrincipal != null)
        {
            rbJugadorPrincipal = scriptJugadorPrincipal.GetComponent<Rigidbody>();
        }

        // Obtener Rigidbody del compañero si no está asignado
        if (rbCompanion == null && companionController != null)
        {
            rbCompanion = companionController.GetComponent<Rigidbody>();
        }

        // El objetivo inicial es el jugador principal
        if (scriptJugadorPrincipal != null)
        {
            objetivoActual = scriptJugadorPrincipal.transform;

            // Asignar objetivo inicial a la cámara orbital
            if (camaraOrbital != null)
            {
                camaraOrbital.CambiarObjetivo(objetivoActual);
            }
        }

        Debug.Log("🎮 GameplayManager iniciado. Presiona Tab (o botón Y del mando) para cambiar de personaje.");
    }

    void Update()
    {
        // Permitir cambio de personaje: Tab o botón Y del mando
        bool cambioTeclado = Input.GetKeyDown(teclaCambio);
        bool cambioMando = Input.GetKeyDown(KeyCode.JoystickButton3); // Botón Y Xbox

        if (puedeCambiar && (cambioTeclado || cambioMando))
        {
            CambiarPersonaje();
        }

        // --- ACTIVAR SEGUIMIENTO DE LA MASCOTA (X / botón B) ---
        bool activarSeguimientoTeclado = Input.GetKeyDown(teclaActivarSeguimiento);
        bool activarSeguimientoMando = Input.GetKeyDown(KeyCode.JoystickButton1); // B en Xbox

        // Solo tiene sentido cuando estamos controlando al jugador
        if (!controlandoCompanion && (activarSeguimientoTeclado || activarSeguimientoMando))
        {
            if (companionController != null)
            {
                companionController.ActivarSeguimiento();
            }
        }
    }

    void FixedUpdate()
    {
        // Sincronizar rotación de la cámara con los personajes
        SincronizarRotacionConCamara();
    }

    /// <summary>
    /// Sincroniza la rotación Y de los personajes con la cámara orbital
    /// </summary>
    private void SincronizarRotacionConCamara()
    {
        if (camaraOrbital == null) return;

        // Obtener el ángulo actual de la cámara
        float anguloY = camaraOrbital.ObtenerAnguloActual();
        Quaternion rotacionObjetivo = Quaternion.Euler(0, anguloY, 0);

        // Rotar el jugador principal usando física
        if (rbJugadorPrincipal != null)
        {
            rbJugadorPrincipal.MoveRotation(rotacionObjetivo);
        }

        // Rotar el compañero usando física
        if (rbCompanion != null)
        {
            rbCompanion.MoveRotation(rotacionObjetivo);
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
            if (scriptJugadorPrincipal != null)
            {
                scriptJugadorPrincipal.enabled = false;
            }

            // Detener completamente el jugador principal
            if (rbJugadorPrincipal != null)
            {
                rbJugadorPrincipal.linearVelocity = Vector3.zero;
                rbJugadorPrincipal.angularVelocity = Vector3.zero;
            }

            // Activar control del compañero
            if (companionController != null)
            {
                companionController.ActivarControl();
                objetivoActual = companionController.transform;

                // Cambiar objetivo de la cámara orbital
                if (camaraOrbital != null)
                {
                    camaraOrbital.CambiarObjetivo(objetivoActual);
                }
            }

            Debug.Log("🐾 Controlando al COMPANERO");
        }
        else
        {
            // Cambiar a controlar el jugador principal
            if (scriptJugadorPrincipal != null)
            {
                scriptJugadorPrincipal.enabled = true;
            }

            // Detener completamente el compañero
            if (rbCompanion != null)
            {
                rbCompanion.linearVelocity = Vector3.zero;
                rbCompanion.angularVelocity = Vector3.zero;
            }

            // Desactivar control del compañero
            if (companionController != null)
            {
                companionController.DesactivarControl();
                objetivoActual = scriptJugadorPrincipal.transform;

                // Cambiar objetivo de la cámara orbital
                if (camaraOrbital != null)
                {
                    camaraOrbital.CambiarObjetivo(objetivoActual);
                }
            }

            Debug.Log("👤 Controlando al JUGADOR");
        }
    }

    /// <summary>
    /// Habilitar la posibilidad de cambiar de personaje
    /// </summary>
    public void HabilitarCambio()
    {
        puedeCambiar = true;
        Debug.Log("✅ Zona de cambio activada. Presiona Tab o botón Y para alternar.");
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

        Debug.Log("❌ Zona de cambio desactivada.");
    }

    /// <summary>
    /// Desactiva temporalmente el control de personajes (para menús, elecciones, etc.)
    /// </summary>
    public void PausarControl()
    {
        // Guardar estado previo
        estadoPrevioPuedeCambiar = puedeCambiar;
        estadoPrevioControlandoCompanion = controlandoCompanion;

        // Bloquear cambios de personaje
        puedeCambiar = false;

        // Desactivar scripts de movimiento
        if (scriptJugadorPrincipal != null)
            scriptJugadorPrincipal.enabled = false;

        if (companionController != null)
            companionController.esControlable = false;

        // Opcional: parar velocidades
        if (rbJugadorPrincipal != null)
            rbJugadorPrincipal.linearVelocity = Vector3.zero;
        if (rbCompanion != null)
            rbCompanion.linearVelocity = Vector3.zero;
    }

    /// <summary>
    /// Restaura el control tal y como estaba antes de PausarControl()
    /// </summary>
    public void ReanudarControl()
    {
        // Restaurar posibilidad de cambio
        puedeCambiar = estadoPrevioPuedeCambiar;

        // Restaurar quién estaba siendo controlado
        if (estadoPrevioControlandoCompanion)
        {
            // Volver a controlar compañero
            if (scriptJugadorPrincipal != null)
                scriptJugadorPrincipal.enabled = false;

            if (companionController != null)
                companionController.ActivarControl();
        }
        else
        {
            // Volver a controlar jugador
            if (scriptJugadorPrincipal != null)
                scriptJugadorPrincipal.enabled = true;

            if (companionController != null)
                companionController.DesactivarControl();
        }
    }
}