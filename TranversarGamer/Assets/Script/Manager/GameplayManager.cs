//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

/// <summary>
/// Gestiona el cambio entre el jugador principal y el compañero.
/// También sincroniza la rotación de ambos con la cámara orbital.
/// </summary>
public class GameplayManager : MonoBehaviour
{
    [Header("Referencias")]
    public Mov_Player3D scriptJugadorPrincipal; // Script del jugador principal
    public Rigidbody rbJugadorPrincipal;        // Rigidbody del jugador principal
    public CompainController companionController; // Script del compañero
    public Rigidbody rbCompanion;               // Rigidbody del compañero

    [Header("Configuración")]
    public KeyCode teclaCambio = KeyCode.Tab;   // Tecla para alternar personaje
    public KeyCode teclaActivarSeguimiento = KeyCode.X; // Tecla para activar seguimiento del compañero
    public bool puedeCambiar = true;            // Indica si el cambio está permitido

    [Header("Cámara Orbital")]
    public CameraOrbital camaraOrbital;         // Cámara orbital usada para orientar personajes

    [Header("Estado global de control")]
    public bool controlesGlobalBloqueados = false;

    private bool controlandoCompanion = false;  // Indica si el compañero está siendo controlado
    private Transform objetivoActual;           // Objetivo actual de la cámara

    private bool estadoPrevioPuedeCambiar;      // Guarda estado previo de cambio
    private bool estadoPrevioControlandoCompanion;

    void Start()
    {
        // Buscar cámara orbital si no está asignada
        if (camaraOrbital == null)
        {
            camaraOrbital = Camera.main.GetComponent<CameraOrbital>();
            if (camaraOrbital == null)
            {
                Debug.LogError("No se encontró CameraOrbital en la Main Camera");
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

        // Establecer jugador como objetivo inicial
        if (scriptJugadorPrincipal != null)
        {
            objetivoActual = scriptJugadorPrincipal.transform;

            if (camaraOrbital != null)
            {
                camaraOrbital.CambiarObjetivo(objetivoActual);
            }
        }
    }

    void Update()
    {
        // Detectar cambio de personaje
        bool cambioTeclado = Input.GetKeyDown(teclaCambio);
        bool cambioMando = Input.GetKeyDown(KeyCode.JoystickButton3);

        if (puedeCambiar && (cambioTeclado || cambioMando))
        {
            CambiarPersonaje();
        }

        // Activar seguimiento del compañero
        bool activarSeguimientoTeclado = Input.GetKeyDown(teclaActivarSeguimiento);
        bool activarSeguimientoMando = Input.GetKeyDown(KeyCode.JoystickButton1);

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
        // Sincronizar rotación de personajes con la cámara
        SincronizarRotacionConCamara();
    }

    private void SincronizarRotacionConCamara()
    {
        if (camaraOrbital == null) return;

        float anguloY = camaraOrbital.ObtenerAnguloActual();
        Quaternion rotacionObjetivo = Quaternion.Euler(0, anguloY, 0);

        // Rotar jugador principal
        if (rbJugadorPrincipal != null)
            rbJugadorPrincipal.MoveRotation(rotacionObjetivo);

        // Rotar compañero
        if (rbCompanion != null)
            rbCompanion.MoveRotation(rotacionObjetivo);
    }

    public void CambiarPersonaje()
    {
        controlandoCompanion = !controlandoCompanion;

        if (controlandoCompanion)
        {
            // Desactivar control del jugador
            if (scriptJugadorPrincipal != null)
                scriptJugadorPrincipal.enabled = false;

            // Detener movimiento del jugador
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

                if (camaraOrbital != null)
                    camaraOrbital.CambiarObjetivo(objetivoActual);
            }
        }
        else
        {
            // Activar control del jugador
            if (scriptJugadorPrincipal != null)
                scriptJugadorPrincipal.enabled = true;

            // Detener movimiento del compañero
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

                if (camaraOrbital != null)
                    camaraOrbital.CambiarObjetivo(objetivoActual);
            }
        }
    }

    public void HabilitarCambio()
    {
        puedeCambiar = true;
    }

    public void DeshabilitarCambio()
    {
        puedeCambiar = false;

        // Si estaba controlando al compañero, volver al jugador
        if (controlandoCompanion)
        {
            CambiarPersonaje();
        }
    }

    public void PausarControl()
    {
        estadoPrevioPuedeCambiar = puedeCambiar;
        estadoPrevioControlandoCompanion = controlandoCompanion;

        puedeCambiar = false;

        if (scriptJugadorPrincipal != null)
            scriptJugadorPrincipal.enabled = false;

        if (companionController != null)
            companionController.esControlable = false;

        if (rbJugadorPrincipal != null)
            rbJugadorPrincipal.linearVelocity = Vector3.zero;

        if (rbCompanion != null)
            rbCompanion.linearVelocity = Vector3.zero;
    }

    public void ReanudarControl()
    {
        puedeCambiar = estadoPrevioPuedeCambiar;

        if (estadoPrevioControlandoCompanion)
        {
            if (scriptJugadorPrincipal != null)
                scriptJugadorPrincipal.enabled = false;

            if (companionController != null)
                companionController.ActivarControl();
        }
        else
        {
            if (scriptJugadorPrincipal != null)
                scriptJugadorPrincipal.enabled = true;

            if (companionController != null)
                companionController.DesactivarControl();
        }
    }
}
