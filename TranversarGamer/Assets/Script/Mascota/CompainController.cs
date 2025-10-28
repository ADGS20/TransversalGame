using UnityEngine;



/// <summary>
/// Script para controlar al compañero pequeño en 2.5D
/// Sigue al jugador principal y puede ser controlado en zonas específicas
/// Movimiento en plano XZ (suelo)
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

    [Header("Configuración de Rotación")]
    [Tooltip("Velocidad de rotación del compañero")]
    [SerializeField] private float velocidadRotacion = 10f;

    [Header("Estado")]
    [Tooltip("¿Está siendo controlado por el jugador?")]
    public bool esControlable = false;

    [Header("Animación (Opcional)")]
    [SerializeField] private Animator animator;

    // Variables privadas
    private float movimientoHorizontal;
    private float movimientoVertical;
    private Vector3 direccionMovimiento;
    private CharacterController characterController;

    void Start()
    {
        // Obtener CharacterController
        characterController = GetComponent<CharacterController>();

        // Si no tiene CharacterController, añadirlo
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.center = new Vector3(0, 0.75f, 0);
            characterController.radius = 0.4f;
            characterController.height = 1.5f;
        }

        // Si no se asignó el jugador principal, intentar encontrarlo
        if (jugadorPrincipal == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
            {
                jugadorPrincipal = jugador.transform;
            }
        }

        // Obtener animator si existe
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
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

        // Calcular distancia en el plano XZ (ignorando altura Y)
        Vector3 posJugador = new Vector3(jugadorPrincipal.position.x, transform.position.y, jugadorPrincipal.position.z);
        float distancia = Vector3.Distance(transform.position, posJugador);

        // Solo seguir si está lejos
        if (distancia > distanciaMinima)
        {
            // Calcular dirección hacia el jugador (solo en plano XZ)
            direccionMovimiento = (posJugador - transform.position).normalized;

            // Mover hacia el jugador
            characterController.Move(direccionMovimiento * velocidadSeguimiento * Time.deltaTime);

            // Rotar hacia la dirección de movimiento
            if (direccionMovimiento != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
            }

            // Actualizar animación
            if (animator != null)
            {
                animator.SetFloat("Velocidad", 1f);
            }
        }
        else
        {
            // Detener animación
            if (animator != null)
            {
                animator.SetFloat("Velocidad", 0f);
            }
        }
    }

    /// <summary>
    /// Lógica para controlar al compañero manualmente
    /// </summary>
    private void ControlarManualmente()
    {
        // Capturar input (usando las mismas teclas que el jugador principal)
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");
        movimientoVertical = Input.GetAxisRaw("Vertical");

        // Crear vector de dirección en el plano XZ
        direccionMovimiento = new Vector3(movimientoHorizontal, 0, movimientoVertical);

        // Normalizar
        if (direccionMovimiento.magnitude > 1)
        {
            direccionMovimiento.Normalize();
        }

        // Mover al compañero
        characterController.Move(direccionMovimiento * velocidadMovimiento * Time.deltaTime);

        // Rotar hacia la dirección de movimiento
        if (direccionMovimiento != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
        }

        // Actualizar animación
        if (animator != null)
        {
            animator.SetFloat("Velocidad", direccionMovimiento.magnitude);
        }
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

        // Detener animación
        if (animator != null)
        {
            animator.SetFloat("Velocidad", 0f);
        }
    }
}