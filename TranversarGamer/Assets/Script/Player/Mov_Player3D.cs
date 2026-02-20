//------------Creador de este script------------//
//---- Hecho por: Andres Diaz Guerrero Soto ----//
//----------------------------------------------//

using UnityEngine;

public class Mov_Player3D : MonoBehaviour
{
    [Header("Input Handler (opcional)")]
    [Tooltip("Si existe, usará InputActions (teclado + mando). Si es null, usa Input.GetAxis.")]
    public InputHandler input;  // Referencia al InputHandler

    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidad = 5f;

    [Header("Configuración de Salto")]
    [SerializeField] private float fuerzaSalto = 5f;
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private Transform checkSuelo;
    [SerializeField] private float radioCheckSuelo = 0.2f;

    [Header("Animación (en hijo)")]
    [SerializeField] private Animator animator;   // Animator del frisk sprite

    [Header("Estado de control")]
    [HideInInspector] public bool controlesBloqueados = false;

    [HideInInspector] public Vector3 direccionMovimiento;

    private Camera camaraJuego;
    private float movimientoHorizontal;
    private float movimientoVertical;
    private Rigidbody rb;
    private bool enSuelo;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        camaraJuego = Camera.main; // La MainCamera tiene el CameraOrbital

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (checkSuelo == null)
        {
            GameObject check = new GameObject("CheckSuelo");
            check.transform.SetParent(transform);
            checkSuelo = check.transform;
            checkSuelo.localPosition = new Vector3(0, -0.5f, 0);
        }

        // Buscar InputHandler si no está asignado
        if (input == null)
            input = FindFirstObjectByType<InputHandler>();
    }

    void Update()
    {
        // Suelo
        enSuelo = Physics.CheckSphere(checkSuelo.position, radioCheckSuelo, capaSuelo);

        // ---- LEER INPUT ----
        Vector2 mov = Vector2.zero;
        bool salto = false;

        if (!controlesBloqueados)
        {
            if (input != null)
            {
                // Nuevo Input System: teclado + mando a la vez
                mov = input.Movimiento;
                salto = input.Saltar;
            }
            else
            {
                // Fallback: sistema clásico (por si el InputHandler no está en escena)
                mov.x = Input.GetAxisRaw("Horizontal");
                mov.y = Input.GetAxisRaw("Vertical");
                salto = Input.GetButtonDown("Jump");
            }
        }

        // Asignar a nuestras variables
        movimientoHorizontal = mov.x;
        movimientoVertical = mov.y;

        // Salto
        if (!controlesBloqueados && salto && enSuelo)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }

        // ---- DIRECCIÓN RELATIVA A LA CÁMARA ----
        if (camaraJuego != null)
        {
            // Usamos la dirección de la cámara actual (que ya rota con CameraOrbital)
            Vector3 camaraForward = camaraJuego.transform.forward;
            camaraForward.y = 0;
            camaraForward.Normalize();

            Vector3 camaraRight = camaraJuego.transform.right;
            camaraRight.y = 0;
            camaraRight.Normalize();

            // W ALEJA de la cámara, S ACERCA
            direccionMovimiento = (camaraRight * movimientoHorizontal + camaraForward * movimientoVertical);
        }
        else
        {
            direccionMovimiento = new Vector3(movimientoHorizontal, 0, movimientoVertical);
        }

        if (direccionMovimiento.magnitude > 1f)
            direccionMovimiento.Normalize();

        // ---- ANIMACIÓN: Blend 1D según WASD ----
        if (animator != null)
        {
            float blendValue = 0f; // Idle

            if (Mathf.Abs(movimientoVertical) > 0.1f)
            {
                if (movimientoVertical > 0)      // W = alejar de cámara
                    blendValue = 1f;             // Walk Up
                else                              // S = acercar a cámara
                    blendValue = 0.25f;          // Walk Down
            }
            else if (Mathf.Abs(movimientoHorizontal) > 0.1f)
            {
                if (movimientoHorizontal > 0)    // D = derecha pantalla
                    blendValue = 0.5f;           // Walk Right
                else                             // A = izquierda pantalla
                    blendValue = 0.75f;          // Walk Left
            }
            else
            {
                blendValue = 0f;                 // Idle
            }

            animator.SetFloat("Blend", blendValue);
        }
    }

    void FixedUpdate()
    {
        Vector3 movimiento = direccionMovimiento * velocidad * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movimiento);
    }

    void OnDisable()
    {
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }

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
        direccionMovimiento = nuevaDireccion;
    }

    public void ForzarIdle()
    {
        movimientoHorizontal = 0f;
        movimientoVertical = 0f;
        direccionMovimiento = Vector3.zero;

        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        if (animator != null)
            animator.SetFloat("Blend", 0f); // Idle
    }
}