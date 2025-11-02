//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

/// <summary>
/// Control del compañero en 2.5D con movimiento físico y billboard
/// </summary>
public class CompainController : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    public Transform jugadorPrincipal;
    [SerializeField] private float distanciaMinima = 2f;
    [SerializeField] private float velocidadSeguimiento = 4f;

    [Header("Movimiento Manual")]
    [SerializeField] private float velocidadMovimiento = 5f;

    [Header("Correr (Manual y Seguimiento)")]
    [Tooltip("¿Permitir correr con Shift cuando es controlable?")]
    [SerializeField] private bool permitirCorrer = true;

    [Tooltip("Multiplicador de velocidad al correr (cuando es controlable)")]
    [SerializeField] private float multiplicadorCorrer = 1.6f;

    [Tooltip("¿Acelerar cuando el jugador está corriendo mientras lo sigue?")]
    [SerializeField] private bool seguirAcelerandoSiJugadorCorre = true;

    [Tooltip("Multiplicador de velocidad al seguir si el jugador corre")]
    [SerializeField] private float multiplicadorSeguirCuandoJugadorCorre = 1.3f;

    [Header("Sprite Billboard")]
    [SerializeField] private float anguloInclinacionX = 45f;
    [SerializeField] private bool mirarACamara = true;
    [SerializeField] private bool rotacionSuave = false;
    [SerializeField] private float velocidadRotacion = 10f;

    [Header("Estado")]
    public bool esControlable = false;

    [Header("Animación")]
    [SerializeField] private Animator animator;

    private CameraOrbital camaraOrbital;
    private Vector3 direccionMovimiento;
    private Rigidbody rb;
    private Transform spriteTransform;
    private SpriteRenderer spriteRenderer;
    private Camera camaraJuego;
    private Collider colliderCompanero;
    private float yawActual = 0f;

    // Referencias opcionales para detectar si el jugador corre
    [Header("Referencias (Opcional)")]
    [Tooltip("Script de movimiento del jugador (si se asigna, se usará para detectar correr)")]
    [SerializeField] private Mov_Player3D scriptJugador; // opcional

    void Start()
    {
        rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ |
                         RigidbodyConstraints.FreezePositionY;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) spriteTransform = spriteRenderer.transform;

        camaraJuego = Camera.main;
        if (camaraJuego != null)
        {
            camaraOrbital = camaraJuego.GetComponent<CameraOrbital>();
        }

        if (jugadorPrincipal == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null) jugadorPrincipal = jugador.transform;
        }

        if (scriptJugador == null && jugadorPrincipal != null)
        {
            scriptJugador = jugadorPrincipal.GetComponent<Mov_Player3D>();
        }

        if (animator == null) animator = GetComponentInChildren<Animator>();

        colliderCompanero = GetComponent<Collider>();
        if (colliderCompanero != null) colliderCompanero.isTrigger = true;

        yawActual = transform.eulerAngles.y;
        if (camaraOrbital != null) yawActual = camaraOrbital.ObtenerAnguloActual();
    }

    void Update()
    {
        if (!esControlable)
        {
            CalcularSeguimiento();
        }

        if (spriteTransform != null && camaraJuego != null)
        {
            ActualizarOrientacionSprite();
        }

        ActualizarFlipSprite();

        if (animator != null)
        {
            float speedFactor = esControlable
                ? ObtenerFactorCorrerManual()
                : ObtenerFactorSeguirCuandoJugadorCorre();

            animator.SetFloat("Velocidad", direccionMovimiento.magnitude * speedFactor);
            animator.SetBool("Corriendo", esControlable ? (permitirCorrer && EstaCorriendo()) :
                                   (seguirAcelerandoSiJugadorCorre && JugadorEstaCorriendo()));
        }

        if (camaraOrbital != null)
        {
            yawActual = camaraOrbital.ObtenerAnguloActual();
        }
    }

    void FixedUpdate()
    {
        float speed = esControlable
            ? velocidadMovimiento * ObtenerFactorCorrerManual()
            : velocidadSeguimiento * ObtenerFactorSeguirCuandoJugadorCorre();

        if (direccionMovimiento != Vector3.zero)
        {
            Vector3 nuevaPosicion = rb.position + direccionMovimiento * speed * Time.fixedDeltaTime;
            rb.MovePosition(nuevaPosicion);
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }

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

    private void ActualizarOrientacionSprite()
    {
        if (mirarACamara)
        {
            Vector3 direccionACamara = camaraJuego.transform.position - spriteTransform.position;
            direccionACamara.y = 0;

            Quaternion rotacionHaciaCamera = Quaternion.LookRotation(direccionACamara);
            Quaternion rotacionFinal = rotacionHaciaCamera * Quaternion.Euler(anguloInclinacionX, 0, 0);

            spriteTransform.rotation = rotacionSuave
                ? Quaternion.Slerp(spriteTransform.rotation, rotacionFinal, velocidadRotacion * Time.deltaTime)
                : rotacionFinal;
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
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (horizontal > 0) spriteRenderer.flipX = true;
            else if (horizontal < 0) spriteRenderer.flipX = false;
        }
        else
        {
            if (jugadorPrincipal == null) return;
            float diferenciaX = transform.position.x - jugadorPrincipal.position.x;
            spriteRenderer.flipX = (diferenciaX < 0);
        }
    }

    public void ActivarControl()
    {
        esControlable = true;
        if (colliderCompanero != null) colliderCompanero.isTrigger = false;
    }

    public void DesactivarControl()
    {
        esControlable = false;
        if (rb != null) rb.linearVelocity = Vector3.zero;
        direccionMovimiento = Vector3.zero;
        if (colliderCompanero != null) colliderCompanero.isTrigger = true;
        if (animator != null) animator.SetBool("Corriendo", false);
    }

    public void CambiarAnguloInclinacion(float nuevoAngulo)
    {
        anguloInclinacionX = nuevoAngulo;
    }

    /// <summary>
    /// Establece la dirección de movimiento (llamado desde GameplayManager)
    /// </summary>
    public void EstablecerDireccion(Vector3 direccion)
    {
        direccionMovimiento = direccion;
    }

    // ——— Correr (Manual) ———
    private bool EstaCorriendo()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private float ObtenerFactorCorrerManual()
    {
        if (!permitirCorrer) return 1f;
        return EstaCorriendo() ? multiplicadorCorrer : 1f;
    }

    // ——— Correr (Seguimiento del jugador) ———
    private bool JugadorEstaCorriendo()
    {
        // Si tenemos referencia a scriptJugador, usamos la misma detección
        if (scriptJugador != null)
        {
            // No tiene un getter público; usamos input como fallback
            // Si prefieres, podemos exponer un método en Mov_Player3D para saber si corre.
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }

        // Fallback: detectar Shift globalmente
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private float ObtenerFactorSeguirCuandoJugadorCorre()
    {
        if (!seguirAcelerandoSiJugadorCorre) return 1f;
        return JugadorEstaCorriendo() ? multiplicadorSeguirCuandoJugadorCorre : 1f;
    }
}