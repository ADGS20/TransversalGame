//----Creador de este script----//
//---- Hecho por: Andres Diaz Guerrero Soto ----//
//----//

using UnityEngine;

/// <summary>
/// Script de movimiento para jugador en 2.5D usando Rigidbody
/// Movimiento RELATIVO A LA CÁMARA (proyectado en plano XZ)
/// W siempre aleja de cámara, S siempre acerca
/// Controla un Animator en el hijo (frisk sprite) con un Blend 1D ("Blend")
/// </summary>
public class Mov_Player3D : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidad = 5f;

    [Header("Configuración de Salto")]
    [SerializeField] private float fuerzaSalto = 5f;
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private Transform checkSuelo;
    [SerializeField] private float radioCheckSuelo = 0.2f;

    [Header("Animación (en hijo)")]
    [SerializeField] private Animator animator;   // Animator del frisk sprite

    [Header("Cámara")]
    private Camera camaraJuego;

    // Privado
    private float movimientoHorizontal;
    private float movimientoVertical;
    private Vector3 direccionMovimiento;
    private Rigidbody rb;
    private bool enSuelo;

    void Start()
    {
        // Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Cámara
        camaraJuego = Camera.main;
        if (camaraJuego == null)
            Debug.LogError("❌ No se encontró la cámara principal");

        // Animator (en el hijo)
        if (animator == null)
        {
            // Buscar en hijos (frisk sprite)
            animator = GetComponentInChildren<Animator>();
        }

        // Check suelo
        if (checkSuelo == null)
        {
            GameObject check = new GameObject("CheckSuelo");
            check.transform.SetParent(transform);
            check.transform.localPosition = new Vector3(0, -0.5f, 0);
            checkSuelo = check.transform;
        }

        Debug.Log("🎮 Mov_Player3D iniciado (físicas en padre, animación en hijo)");
    }

    void Update()
    {
        // Suelo
        enSuelo = Physics.CheckSphere(checkSuelo.position, radioCheckSuelo, capaSuelo);

        // Input
        movimientoHorizontal = Input.GetAxisRaw("Horizontal"); // A/D
        movimientoVertical = Input.GetAxisRaw("Vertical");   // W/S

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
        }

        // Dirección relativa a la cámara
        if (camaraJuego != null)
        {
            Vector3 camaraForward = camaraJuego.transform.forward;
            camaraForward.y = 0;
            camaraForward.Normalize();

            Vector3 camaraRight = camaraJuego.transform.right;
            camaraRight.y = 0;
            camaraRight.Normalize();

            direccionMovimiento = (camaraRight * movimientoHorizontal - camaraForward * movimientoVertical);
        }
        else
        {
            direccionMovimiento = new Vector3(movimientoHorizontal, 0, movimientoVertical);
        }

        if (direccionMovimiento.magnitude > 1)
            direccionMovimiento.Normalize();

        // ANIMACIÓN SENCILLA CON BLEND 1D
        if (animator != null)
        {
            float blendValue = 0f; // Idle por defecto

            if (movimientoVertical > 0)          // W → Up
                blendValue = 1f;
            else if (movimientoVertical < 0)     // S → Down
                blendValue = 0.25f;
            else if (movimientoHorizontal > 0)   // D → Right
                blendValue = 0.5f;
            else if (movimientoHorizontal < 0)   // A → Left
                blendValue = 0.75f;
            else
                blendValue = 0f;                 // Idle

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

    // Para el GameplayManager (si lo usas)
    public void EstablecerDireccion(Vector3 nuevaDireccion)
    {
        direccionMovimiento = nuevaDireccion;

        if (animator != null)
        {
            // Convertimos la dirección en algo equivalente a las teclas, muy simple
            float blendValue = 0f;

            if (nuevaDireccion.z > 0.1f)
                blendValue = 1f;          // Up
            else if (nuevaDireccion.z < -0.1f)
                blendValue = 0.25f;       // Down
            else if (nuevaDireccion.x > 0.1f)
                blendValue = 0.5f;        // Right
            else if (nuevaDireccion.x < -0.1f)
                blendValue = 0.75f;       // Left
            else
                blendValue = 0f;          // Idle

            animator.SetFloat("Blend", blendValue);
        }
    }
}