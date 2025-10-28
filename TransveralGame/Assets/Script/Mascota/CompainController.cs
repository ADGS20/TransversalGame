using UnityEngine;



/// <summary>
/// Script para controlar al compa�ero peque�o
/// Sigue al jugador principal y puede ser controlado en zonas espec�ficas
/// </summary>
public class CompainController : MonoBehaviour
{
    [Header("Configuraci�n de Seguimiento")]
    [Tooltip("Referencia al transform del jugador principal")]
    public Transform jugadorPrincipal;

    [Tooltip("Distancia m�nima para empezar a seguir")]
    [SerializeField] private float distanciaMinima = 2f;

    [Tooltip("Velocidad de seguimiento")]
    [SerializeField] private float velocidadSeguimiento = 4f;

    [Header("Configuraci�n de Movimiento Manual")]
    [Tooltip("Velocidad cuando el jugador controla al compa�ero")]
    [SerializeField] private float velocidadMovimiento = 5f;

    [Header("Estado")]
    [Tooltip("�Est� siendo controlado por el jugador?")]
    public bool esControlable = false;

    // Variables privadas
    private float movimientoHorizontal;
    private float movimientoVertical;
    private Rigidbody2D rb;

    void Start()
    {
        // Obtener el Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        // Si no tiene Rigidbody2D, a�adirlo
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // Configurar el Rigidbody2D para que no rote
        rb.freezeRotation = true;
        rb.gravityScale = 0; // Sin gravedad para movimiento top-down

        // Si no se asign� el jugador principal, intentar encontrarlo
        if (jugadorPrincipal == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
            {
                jugadorPrincipal = jugador.transform;
            }
        }
    }

    void Update()
    {
        if (esControlable)
        {
            // El jugador controla al compa�ero directamente
            ControlarManualmente();
        }
        else
        {
            // El compa�ero sigue al jugador principal
            SeguirJugador();
        }
    }

    /// <summary>
    /// L�gica para seguir al jugador principal autom�ticamente
    /// </summary>
    private void SeguirJugador()
    {
        if (jugadorPrincipal == null) return;

        float distancia = Vector2.Distance(transform.position, jugadorPrincipal.position);

        // Solo seguir si est� lejos
        if (distancia > distanciaMinima)
        {
            // Calcular direcci�n hacia el jugador
            Vector2 direccion = (jugadorPrincipal.position - transform.position).normalized;

            // Mover usando Rigidbody2D
            rb.linearVelocity = direccion * velocidadSeguimiento;
        }
        else
        {
            // Detener el movimiento
            rb.linearVelocity = Vector2.zero;
        }
    }

    /// <summary>
    /// L�gica para controlar al compa�ero manualmente
    /// </summary>
    private void ControlarManualmente()
    {
        // Capturar input (usando las mismas teclas que el jugador principal)
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");
        movimientoVertical = Input.GetAxisRaw("Vertical");

        // Crear vector de direcci�n
        Vector2 direccion = new Vector2(movimientoHorizontal, movimientoVertical);
        direccion.Normalize();

        // Mover usando Rigidbody2D
        rb.linearVelocity = direccion * velocidadMovimiento;
    }

    /// <summary>
    /// Activar control manual del compa�ero
    /// </summary>
    public void ActivarControl()
    {
        esControlable = true;
    }

    /// <summary>
    /// Desactivar control manual del compa�ero
    /// </summary>
    public void DesactivarControl()
    {
        esControlable = false;
        rb.linearVelocity = Vector2.zero; // Detener movimiento al desactivar
    }
}