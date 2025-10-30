using UnityEngine;

/// <summary>
/// Script de movimiento para jugador en 2.5D usando Rigidbody
/// Movimiento RELATIVO A LA CÁMARA (proyectado en plano XZ)
/// W siempre aleja de la cámara, S siempre acerca
/// </summary>
public class Mov_Player3D : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad de movimiento del jugador")]
    [SerializeField] private float velocidad = 5f;

    [Header("Animación (Opcional)")]
    [SerializeField] private Animator animator;

    [Header("Cámara")]
    private Camera camaraJuego;

    // Variables privadas
    private float movimientoHorizontal;
    private float movimientoVertical;
    private Vector3 direccionMovimiento;
    private Rigidbody rb;

    void Start()
    {
        // Obtener Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Configurar Rigidbody para movimiento 2.5D
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                        RigidbodyConstraints.FreezeRotationZ |
                        RigidbodyConstraints.FreezePositionY;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Obtener cámara principal
        camaraJuego = Camera.main;
        if (camaraJuego == null)
        {
            Debug.LogError("❌ No se encontró la cámara principal");
        }

        // Obtener animator si existe
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        Debug.Log("🎮 Mov_Player3D iniciado - Controles relativos a la cámara");
    }

    void Update()
    {
        // Capturar input
        movimientoHorizontal = Input.GetAxisRaw("Horizontal"); // A/D
        movimientoVertical = Input.GetAxisRaw("Vertical");     // W/S

        // Calcular dirección RELATIVA A LA CÁMARA (PROYECTADA EN PLANO XZ)
        if (camaraJuego != null)
        {
            // Obtener forward de la cámara y proyectarlo en el plano XZ
            Vector3 camaraForward = camaraJuego.transform.forward;
            camaraForward.y = 0; // Proyectar en plano horizontal
            camaraForward.Normalize(); // Normalizar después de eliminar Y

            // Obtener right de la cámara y proyectarlo en el plano XZ
            Vector3 camaraRight = camaraJuego.transform.right;
            camaraRight.y = 0; // Proyectar en plano horizontal
            camaraRight.Normalize(); // Normalizar después de eliminar Y

            // Crear vector de movimiento relativo a la cámara
            // W = Alejar de cámara (dirección opuesta a donde mira)
            // S = Acercar a cámara (dirección hacia donde mira)
            direccionMovimiento = (camaraRight * movimientoHorizontal - camaraForward * movimientoVertical);
        }
        else
        {
            // Fallback si no hay cámara
            direccionMovimiento = new Vector3(movimientoHorizontal, 0, movimientoVertical);
        }

        // Normalizar para movimiento diagonal
        if (direccionMovimiento.magnitude > 1)
        {
            direccionMovimiento.Normalize();
        }

        // Actualizar animaciones
        if (animator != null)
        {
            animator.SetFloat("Velocidad", direccionMovimiento.magnitude);
        }
    }

    void FixedUpdate()
    {
        // Mover usando física
        Vector3 nuevaPosicion = rb.position + direccionMovimiento * velocidad * Time.fixedDeltaTime;
        rb.MovePosition(nuevaPosicion);
    }

    void OnDisable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}