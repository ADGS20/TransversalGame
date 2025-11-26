//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

/// <summary>
/// Script de movimiento para jugador en 2.5D usando Rigidbody
/// Movimiento RELATIVO A LA CÁMARA (proyectado en plano XZ)
/// W siempre aleja de cámara, S siempre acerca
/// </summary>
public class Mov_Player3D : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad de movimiento del jugador")]
    [SerializeField] private float velocidad = 5f;

    [Header("Configuración de Salto")]
    [SerializeField] private float fuerzaSalto = 5f;
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private Transform checkSuelo;
    [SerializeField] private float radioCheckSuelo = 0.2f;

    [Header("Animación (Opcional)")]
    [SerializeField] private Animator animator;

    [Header("Cámara")]
    private Camera camaraJuego;

    // Variables privadas
    private float movimientoHorizontal;
    private float movimientoVertical;
    private Vector3 direccionMovimiento;
    private Rigidbody rb;
    private bool enSuelo;

    void Start()
    {
        // Obtener Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Configurar Rigidbody para movimiento 2.5D con salto
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                        RigidbodyConstraints.FreezeRotationZ;
        rb.useGravity = true; // Activar gravedad para el salto
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

        // Crear checkSuelo si no existe
        if (checkSuelo == null)
        {
            GameObject check = new GameObject("CheckSuelo");
            check.transform.SetParent(transform);
            check.transform.localPosition = new Vector3(0, -0.5f, 0);
            checkSuelo = check.transform;
        }

        Debug.Log("🎮 Mov_Player3D iniciado - Controles relativos a la cámara + Salto");
    }

    void Update()
    {
        // Verificar si está en el suelo
        enSuelo = Physics.CheckSphere(checkSuelo.position, radioCheckSuelo, capaSuelo);

        // Capturar input de movimiento
        movimientoHorizontal = Input.GetAxisRaw("Horizontal"); // A/D
        movimientoVertical = Input.GetAxisRaw("Vertical");    // W/S

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            Debug.Log("🎮 Jugador saltó");
        }

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
            animator.SetBool("EnSuelo", enSuelo);
        }
    }

    void FixedUpdate()
    {
        // Mover usando física (solo en XZ, mantener velocidad Y)
        Vector3 movimiento = direccionMovimiento * velocidad * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movimiento);
    }

    void OnDisable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    // Visualizar el check de suelo en el editor
    void OnDrawGizmosSelected()
    {
        if (checkSuelo != null)
        {
            Gizmos.color = enSuelo ? Color.green : Color.red;
            Gizmos.DrawWireSphere(checkSuelo.position, radioCheckSuelo);
        }
    }

    public void EstablecerDireccion(Vector3 nuevaDireccion)
    {
        // La dirección viene ya calculada en el Manager, solo la usamos
        direccionMovimiento = nuevaDireccion;

        // Actualizar animación si existe
        if (animator != null)
        {
            animator.SetFloat("Velocidad", direccionMovimiento.magnitude);
        }
    }

}