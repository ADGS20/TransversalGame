//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

/// <summary>
/// Control del compañero en 2.5D:
/// - Sigue al jugador cuando seguimientoActivo = true y no es controlable
/// - Se mueve con WASD / stick izquierdo relativo a la cámara cuando es controlable
/// - Salta con Space / botón A
/// - Planea si mantiene el botón de saltar en el aire (cae lentamente)
/// - Sprite billboard mirando a la cámara
/// </summary>
public class CompainController : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    public Transform jugadorPrincipal;
    [SerializeField] private float distanciaMinima = 2f;
    [SerializeField] private float velocidadSeguimiento = 4f;

    [Header("Movimiento Manual")]
    [SerializeField] private float velocidadMovimiento = 5f;

    [Header("Salto y Planeo")]
    [SerializeField] private float fuerzaSalto = 7f;
    [SerializeField] private float gravedadNormal = -20f;
    [SerializeField] private float gravedadPlaneo = -5f;
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private Transform checkSuelo;
    [SerializeField] private float radioCheckSuelo = 0.2f;

    [Header("Sprite Billboard")]
    [SerializeField] private float anguloInclinacionX = 30f;
    [SerializeField] private bool mirarACamara = true;
    [SerializeField] private bool rotacionSuave = false;
    [SerializeField] private float velocidadRotacion = 10f;

    [Header("Estado")]
    [Tooltip("¿Está siendo controlado por el jugador?")]
    public bool esControlable = false;

    [Tooltip("¿Debe seguir automáticamente al jugador cuando no es controlable?")]
    public bool seguimientoActivo = true;

    [Header("Animación")]
    [SerializeField] private Animator animator;

    // Privado
    private float movimientoHorizontal;
    private float movimientoVertical;
    private Vector3 direccionMovimiento;
    private Rigidbody rb;
    private Transform spriteTransform;
    private SpriteRenderer spriteRenderer;
    private Camera camaraJuego;
    private Collider colliderCompanero;

    private bool enSuelo;
    private bool estaPlanando = false;
    private float velocidadVertical = 0f;

    void Start()
    {
        // Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY |
                         RigidbodyConstraints.FreezeRotationZ;
        rb.useGravity = false; // usamos gravedad personalizada
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Sprite
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteTransform = spriteRenderer.transform;

        // Cámara
        camaraJuego = Camera.main;

        // Jugador
        if (jugadorPrincipal == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null) jugadorPrincipal = jugador.transform;
        }

        // Animator
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Collider
        colliderCompanero = GetComponent<Collider>();
        if (colliderCompanero != null)
            colliderCompanero.isTrigger = true; // modo seguimiento por defecto

        // Check suelo auto si no se asigna
        if (checkSuelo == null)
        {
            GameObject check = new GameObject("CheckSuelo_Companero");
            check.transform.SetParent(transform);
            check.transform.localPosition = new Vector3(0, -0.5f, 0);
            checkSuelo = check.transform;
        }
    }

    void Update()
    {
        // Comprobar suelo
        enSuelo = Physics.CheckSphere(checkSuelo.position, radioCheckSuelo, capaSuelo);

        if (esControlable)
        {
            CapturarInputManual();
            AplicarMovimientoRelativoACamara();

            // --- LECTURA BOTÓN DE SALTO (teclado + mando) ---
            bool saltoDown = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0);   // A (Xbox)
            bool saltoHold = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0);

            // Salto inicial
            if (saltoDown && enSuelo)
            {
                velocidadVertical = fuerzaSalto;
                estaPlanando = false;
            }

            // Planeo (mantener botón mientras cae)
            if (saltoHold && !enSuelo && velocidadVertical < 0)
            {
                estaPlanando = true;
            }
            else if (!saltoHold)
            {
                estaPlanando = false;
            }
        }
        else
        {
            // Cuando no es controlable:
            if (seguimientoActivo)
            {
                CalcularSeguimientoAutomatico(); // sigue al jugador
            }
            else
            {
                direccionMovimiento = Vector3.zero; // se queda quieto
            }

            estaPlanando = false;
        }

        // Sprite billboard
        if (spriteTransform != null && camaraJuego != null)
            ActualizarOrientacionSprite();

        // Flip
        ActualizarFlipSprite();

        // Animaciones
        if (animator != null)
        {
            animator.SetFloat("Velocidad", direccionMovimiento.magnitude);
            animator.SetBool("EnSuelo", enSuelo);
            animator.SetBool("Planando", estaPlanando);
        }
    }

    void FixedUpdate()
    {
        // Gravedad personalizada
        if (!enSuelo)
        {
            float g = estaPlanando ? gravedadPlaneo : gravedadNormal;
            velocidadVertical += g * Time.fixedDeltaTime;
        }
        else
        {
            if (velocidadVertical < 0f) velocidadVertical = 0f;
        }

        // Movimiento XZ
        float velocidadActual = esControlable ? velocidadMovimiento : velocidadSeguimiento;
        Vector3 movimientoXZ = direccionMovimiento * velocidadActual * Time.fixedDeltaTime;

        // Movimiento Y
        Vector3 movimientoY = Vector3.up * velocidadVertical * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + movimientoXZ + movimientoY);
    }

    /// <summary>
    /// Seguimiento automático en plano XZ hacia el jugador.
    /// </summary>
    private void CalcularSeguimientoAutomatico()
    {
        if (jugadorPrincipal == null) return;

        Vector3 posJugador = new Vector3(jugadorPrincipal.position.x, transform.position.y, jugadorPrincipal.position.z);
        float distancia = Vector3.Distance(transform.position, posJugador);

        if (distancia > distanciaMinima)
            direccionMovimiento = (posJugador - transform.position).normalized;
        else
            direccionMovimiento = Vector3.zero;
    }

    /// <summary>
    /// Lee input de WASD / stick izquierdo (ejes Horizontal/Vertical).
    /// </summary>
    private void CapturarInputManual()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");
        movimientoVertical = Input.GetAxisRaw("Vertical");
    }

    /// <summary>
    /// Convierte el input en un vector relativo a la cámara (igual que el jugador).
    /// </summary>
    private void AplicarMovimientoRelativoACamara()
    {
        if (camaraJuego != null)
        {
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
    }

    private void ActualizarOrientacionSprite()
    {
        if (!mirarACamara)
        {
            spriteTransform.localRotation = Quaternion.Euler(anguloInclinacionX, 0, 0);
            return;
        }

        Vector3 dirCam = camaraJuego.transform.position - spriteTransform.position;
        dirCam.y = 0;

        Quaternion rotCam = Quaternion.LookRotation(dirCam);
        Quaternion rotFinal = rotCam * Quaternion.Euler(anguloInclinacionX, 0, 0);

        if (rotacionSuave)
            spriteTransform.rotation = Quaternion.Slerp(spriteTransform.rotation, rotFinal, velocidadRotacion * Time.deltaTime);
        else
            spriteTransform.rotation = rotFinal;
    }

    private void ActualizarFlipSprite()
    {
        if (spriteRenderer == null) return;

        if (esControlable)
        {
            if (movimientoHorizontal > 0)
                spriteRenderer.flipX = true;
            else if (movimientoHorizontal < 0)
                spriteRenderer.flipX = false;
        }
        else
        {
            if (jugadorPrincipal == null) return;

            float difX = transform.position.x - jugadorPrincipal.position.x;
            spriteRenderer.flipX = difX < 0;
        }
    }

    public void ActivarControl()
    {
        esControlable = true;
        seguimientoActivo = false; // cuando tomas el control, dejas de seguir

        if (colliderCompanero != null)
            colliderCompanero.isTrigger = false;

        velocidadVertical = 0f;
    }

    public void DesactivarControl()
    {
        esControlable = false;
        direccionMovimiento = Vector3.zero;
        velocidadVertical = 0f;
        estaPlanando = false;

        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        if (colliderCompanero != null)
            colliderCompanero.isTrigger = true;

        // NO activamos seguimiento aquí: se queda en el sitio hasta que el jugador pulse X (GameplayManager → ActivarSeguimiento)
    }

    /// <summary>
    /// Activa el seguimiento automático al jugador (llamado cuando jugador pulsa X).
    /// </summary>
    public void ActivarSeguimiento()
    {
        seguimientoActivo = true;
    }

    public void DesactivarSeguimiento()
    {
        seguimientoActivo = false;
        direccionMovimiento = Vector3.zero;
    }

    public void CambiarAnguloInclinacion(float nuevoAngulo)
    {
        anguloInclinacionX = nuevoAngulo;
    }

    // Para que veas el check de suelo
    private void OnDrawGizmosSelected()
    {
        if (checkSuelo != null)
        {
            Gizmos.color = enSuelo ? Color.green : Color.red;
            Gizmos.DrawWireSphere(checkSuelo.position, radioCheckSuelo);
        }
    }

    // Para el GameplayManager (si algún día lo usas)
    public void EstablecerDireccion(Vector3 nuevaDireccion)
    {
        if (esControlable)
        {
            direccionMovimiento = nuevaDireccion;
            if (direccionMovimiento.magnitude > 1f)
                direccionMovimiento.Normalize();
        }
    }
}