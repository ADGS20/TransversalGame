//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

/// <summary>
/// Control del compañero en 2.5D:
/// - Sigue al jugador cuando no es controlable
/// - Se mueve con WASD cuando es controlable
/// - Salta con Space
/// - Planea si mantiene Space en el aire (cae lentamente)
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
    [SerializeField] private float fuerzaSalto = 5f;
    [SerializeField] private float gravedadNormal = -9.81f;
    [SerializeField] private float gravedadPlaneo = -2f;
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private Transform checkSuelo;
    [SerializeField] private float radioCheckSuelo = 0.2f;

    [Header("Sprite Billboard")]
    [SerializeField] private float anguloInclinacionX = 45f;
    [SerializeField] private bool mirarACamara = true;
    [SerializeField] private bool rotacionSuave = false;
    [SerializeField] private float velocidadRotacion = 10f;

    [Header("Estado")]
    [Tooltip("¿Está siendo controlado por el jugador?")]
    public bool esControlable = false;

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

        // NO congelamos Y (porque debe saltar)
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

            // Salto
            if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
            {
                velocidadVertical = fuerzaSalto;
                estaPlanando = false;
                Debug.Log("🐾 Compañero saltó");
            }

            // Planeo (mantener Space en el aire mientras cae)
            if (Input.GetKey(KeyCode.Space) && !enSuelo && velocidadVertical < 0)
            {
                estaPlanando = true;
            }
            else if (!Input.GetKey(KeyCode.Space))
            {
                estaPlanando = false;
            }
        }
        else
        {
            CalcularSeguimiento();
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

    private void CalcularSeguimiento()
    {
        if (jugadorPrincipal == null) return;

        Vector3 posJugador = new Vector3(jugadorPrincipal.position.x, transform.position.y, jugadorPrincipal.position.z);
        float distancia = Vector3.Distance(transform.position, posJugador);

        if (distancia > distanciaMinima)
            direccionMovimiento = (posJugador - transform.position).normalized;
        else
            direccionMovimiento = Vector3.zero;
    }

    private void CapturarInputManual()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");
        movimientoVertical = Input.GetAxisRaw("Vertical");

        direccionMovimiento = new Vector3(movimientoHorizontal, 0, movimientoVertical);

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
        if (colliderCompanero != null)
            colliderCompanero.isTrigger = false;
        Debug.Log("🐾 Compañero activado");
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

        Debug.Log("👤 Compañero desactivado");
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

    // Para el GameplayManager
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