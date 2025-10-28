using UnityEngine;



/// <summary>
/// Script para controlar al compañero pequeño
/// Sigue al jugador principal y puede ser controlado en zonas específicas
/// </summary>
public class CompainController : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    [Tooltip("Referencia al transform del jugador principal")]
    public Transform jugadorPrincipal;

    [Tooltip("Distancia mínima para empezar a seguir")]
    [SerializeField] private float distanciaMinima = 2f;

    [Tooltip("Velocidad de seguimiento")]
    [SerializeField] private float velocidadSeguimiento = 4f;

    [Header("Configuración de Movimiento Manual")]
    [Tooltip("Velocidad cuando el jugador controla al compañero")]
    [SerializeField] private float velocidadMovimiento = 5f;

    [Header("Estado")]
    [Tooltip("¿Está siendo controlado por el jugador?")]
    public bool esControlable = false;

    // Variables privadas
    private float movimientoHorizontal;
    private float movimientoVertical;
    private Vector2 ultimaPosicionJugador;
    private float tiempoSinMovimiento = 0f;

    void Start()
    {
        // Si no se asignó el jugador principal, intentar encontrarlo
        if (jugadorPrincipal == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
            {
                jugadorPrincipal = jugador.transform;
            }
        }

        ultimaPosicionJugador = jugadorPrincipal.position;
    }

    void Update()
    {
        if (esControlable)
        {
            // El jugador controla al compañero directamente
            ControlarManualmente();
        }
        else
        {
            // El compañero sigue al jugador principal
            SeguirJugador();
        }
    }

    /// <summary>
    /// Lógica para seguir al jugador principal automáticamente
    /// </summary>
    private void SeguirJugador()
    {
        if (jugadorPrincipal == null) return;

        float distancia = Vector2.Distance(transform.position, jugadorPrincipal.position);

        // Solo seguir si está lejos
        if (distancia > distanciaMinima)
        {
            // Calcular dirección hacia el jugador
            Vector2 direccion = (jugadorPrincipal.position - transform.position).normalized;

            // Mover hacia el jugador
            transform.Translate(direccion * velocidadSeguimiento * Time.deltaTime);

            tiempoSinMovimiento = 0f;
        }
        else
        {
            tiempoSinMovimiento += Time.deltaTime;
        }

        ultimaPosicionJugador = jugadorPrincipal.position;
    }

    /// <summary>
    /// Lógica para controlar al compañero manualmente
    /// </summary>
    private void ControlarManualmente()
    {
        // Capturar input (usando teclas diferentes al jugador principal)
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");
        movimientoVertical = Input.GetAxisRaw("Vertical");

        // Crear vector de dirección
        Vector2 direccion = new Vector2(movimientoHorizontal, movimientoVertical);
        direccion.Normalize();

        // Mover al compañero
        transform.Translate(direccion * velocidadMovimiento * Time.deltaTime);
    }

    /// <summary>
    /// Activar control manual del compañero
    /// </summary>
    public void ActivarControl()
    {
        esControlable = true;
    }

    /// <summary>
    /// Desactivar control manual del compañero
    /// </summary>
    public void DesactivarControl()
    {
        esControlable = false;
    }
}