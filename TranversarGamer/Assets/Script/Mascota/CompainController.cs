using UnityEngine;

/// <summary>
/// Script para controlar al compañero en 2.5D con sprite billboard
/// USA FÍSICA PURA con Rigidbody.MoveRotation
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

    [Header("Configuración de Sprite Billboard")]
    [Tooltip("Ángulo de inclinación del sprite en el eje X (45°, 60°, 30°)")]
    [SerializeField] private float anguloInclinacionX = 45f;

    [Tooltip("¿El sprite siempre mira a la cámara? (Billboard)")]
    [SerializeField] private bool mirarACamara = true;

    [Tooltip("¿Usar rotación suave del sprite?")]
    [SerializeField] private bool rotacionSuave = false;

    [Tooltip("Velocidad de rotación suave (si está activado)")]
    [SerializeField] private float velocidadRotacion = 10f;

    [Header("Estado")]
    [Tooltip("¿Está siendo controlado por el jugador?")]
    public bool esControlable = false;

    [Header("Animación")]
    [SerializeField] private Animator animator;

    [Header("Cámara Orbital")]
    private CameraOrbital camaraOrbital;

    // Variables privadas
    private float movimientoHorizontal;
    private float movimientoVertical;
    private Vector3 direccionMovimiento;
    private Rigidbody rb;
    private Transform spriteTransform;
    private SpriteRenderer spriteRenderer;
    private Camera camaraJuego;
    private int direccionActual = 0;
    private Collider colliderCompanero;

    // NUEVO: Variable para acumular la rotación Y
    private float yawActual = 0f;

    void Start()
    {
        // Obtener Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // IMPORTANTE: Solo bloquear X y Z, dejar Y libre
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                        RigidbodyConstraints.FreezeRotationZ |
                        RigidbodyConstraints.FreezePositionY;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Buscar sprite
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteTransform = spriteRenderer.transform;
        }

        // Obtener cámara
        camaraJuego = Camera.main;
        if (camaraJuego != null)
        {
            camaraOrbital = camaraJuego.GetComponent<CameraOrbital>();
            if (camaraOrbital == null)
            {
                Debug.LogWarning("⚠️ No se encontró CameraOrbital en la cámara principal");
            }
        }

        // Buscar jugador principal
        if (jugadorPrincipal == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
            {
                jugadorPrincipal = jugador.transform;
            }
        }

        // Obtener animator
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // Obtener collider
        colliderCompanero = GetComponent<Collider>();
        if (colliderCompanero != null)
        {
            colliderCompanero.isTrigger = true;
        }

        // Inicializar yaw con la rotación actual
        yawActual = transform.eulerAngles.y;

        // Sincronizar con la cámara si existe
        if (camaraOrbital != null)
        {
            yawActual = camaraOrbital.ObtenerAnguloActual();
            Debug.Log($"🐾 Compañero inicializado con rotación: {yawActual}°");
        }
    }

    void Update()
    {
        if (esControlable)
        {
            CapturarInputManual();
        }
        else
        {
            CalcularSeguimiento();
        }

        // Actualizar sprite para que mire a la cámara
        if (spriteTransform != null && camaraJuego != null)
        {
            ActualizarOrientacionSprite();
        }

        // Actualizar flip del sprite
        ActualizarFlipSprite();

        // Actualizar animaciones
        if (animator != null)
        {
            animator.SetFloat("Velocidad", direccionMovimiento.magnitude);
            animator.SetInteger("Direccion", direccionActual);
        }

        // NUEVO: Actualizar yaw objetivo desde la cámara
        if (camaraOrbital != null)
        {
            yawActual = camaraOrbital.ObtenerAnguloActual();
        }
    }

    void FixedUpdate()
    {
        // Mover usando Rigidbody
        if (direccionMovimiento != Vector3.zero)
        {
            Vector3 nuevaPosicion = rb.position + direccionMovimiento *
                (esControlable ? velocidadMovimiento : velocidadSeguimiento) * Time.fixedDeltaTime;
            rb.MovePosition(nuevaPosicion);
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }

        // CORRECTO: Rotar usando MoveRotation (RESPETA LA FÍSICA)
        Quaternion rotacionObjetivo = Quaternion.Euler(0, yawActual, 0);
        rb.MoveRotation(rotacionObjetivo);
    }

    private void CalcularSeguimiento()
    {
        if (jugadorPrincipal == null) return;

        Vector3 posJugador = new Vector3(jugadorPrincipal.position.x, transform.position.y, jugadorPrincipal.position.z);
        float distancia = Vector3.Distance(transform.position, posJugador);

        if (distancia > distanciaMinima)
        {
            direccionMovimiento = (posJugador - transform.position).normalized;
        }
        else
        {
            direccionMovimiento = Vector3.zero;
        }
    }

    private void CapturarInputManual()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");
        movimientoVertical = Input.GetAxisRaw("Vertical");

        // Crear vector de dirección RELATIVO A LA ROTACIÓN ACTUAL
        Vector3 forward = Quaternion.Euler(0, yawActual, 0) * Vector3.forward;
        Vector3 right = Quaternion.Euler(0, yawActual, 0) * Vector3.right;

        direccionMovimiento = (right * movimientoHorizontal + forward * movimientoVertical);

        if (direccionMovimiento.magnitude > 1)
        {
            direccionMovimiento.Normalize();
        }
    }

    private void ActualizarOrientacionSprite()
    {
        if (mirarACamara)
        {
            Vector3 direccionACamara = camaraJuego.transform.position - spriteTransform.position;
            direccionACamara.y = 0;

            Quaternion rotacionHaciaCamera = Quaternion.LookRotation(direccionACamara);
            Quaternion rotacionFinal = rotacionHaciaCamera * Quaternion.Euler(anguloInclinacionX, 0, 0);

            if (rotacionSuave)
            {
                spriteTransform.rotation = Quaternion.Slerp(
                    spriteTransform.rotation,
                    rotacionFinal,
                    velocidadRotacion * Time.deltaTime
                );
            }
            else
            {
                spriteTransform.rotation = rotacionFinal;
            }
        }
        else
        {
            spriteTransform.localRotation = Quaternion.Euler(anguloInclinacionX, 0, 0);
        }
    }

    private void ActualizarFlipSprite()
    {
        if (spriteRenderer == null) return;

        if (esControlable)
        {
            if (movimientoHorizontal > 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (movimientoHorizontal < 0)
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            if (jugadorPrincipal == null) return;

            float diferenciaX = transform.position.x - jugadorPrincipal.position.x;

            if (diferenciaX < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    public void ActivarControl()
    {
        esControlable = true;

        if (colliderCompanero != null)
        {
            colliderCompanero.isTrigger = false;
        }

        Debug.Log("🐾 Compañero activado - Trigger desactivado");
    }

    public void DesactivarControl()
    {
        esControlable = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
        direccionMovimiento = Vector3.zero;

        if (colliderCompanero != null)
        {
            colliderCompanero.isTrigger = true;
        }

        Debug.Log("👤 Compañero desactivado - Trigger activado");
    }

    public void CambiarAnguloInclinacion(float nuevoAngulo)
    {
        anguloInclinacionX = nuevoAngulo;
    }
}