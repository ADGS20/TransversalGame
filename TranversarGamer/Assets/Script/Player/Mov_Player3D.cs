//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

/// <summary>
/// Script de movimiento para jugador en 2.5D usando Rigidbody
/// RECIBE la dirección de movimiento desde GameplayManager
/// </summary>
public class Mov_Player3D : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad base de movimiento del jugador")]
    [SerializeField] private float velocidad = 5f;

    [Header("Correr")]
    [Tooltip("¿Permitir correr con Shift?")]
    [SerializeField] private bool permitirCorrer = true;

    [Tooltip("Multiplicador de velocidad al correr")]
    [SerializeField] private float multiplicadorCorrer = 1.6f;

    [Header("Animación (Opcional)")]
    [SerializeField] private Animator animator;

    // Variables privadas
    private Rigidbody rb;
    private Vector3 direccionMovimiento;

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

        // Obtener animator si existe
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        Debug.Log("🎮 Mov_Player3D iniciado");
    }

    void Update()
    {
        // Actualizar animaciones
        if (animator != null)
        {
            float speedFactor = ObtenerFactorCorrer();
            animator.SetFloat("Velocidad", direccionMovimiento.magnitude * speedFactor);
            animator.SetBool("Corriendo", permitirCorrer && EstaCorriendo());
        }
    }

    void FixedUpdate()
    {
        // Mover usando física
        float speedFactor = ObtenerFactorCorrer();
        Vector3 nuevaPosicion = rb.position + direccionMovimiento * (velocidad * speedFactor) * Time.fixedDeltaTime;
        rb.MovePosition(nuevaPosicion);
    }

    /// <summary>
    /// Establece la dirección de movimiento (llamado desde GameplayManager)
    /// </summary>
    public void EstablecerDireccion(Vector3 direccion)
    {
        direccionMovimiento = direccion;
    }

    private bool EstaCorriendo()
    {
        // Shift izquierdo o derecho
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private float ObtenerFactorCorrer()
    {
        if (!permitirCorrer) return 1f;
        return EstaCorriendo() ? multiplicadorCorrer : 1f;
    }

    void OnDisable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
        direccionMovimiento = Vector3.zero;

        if (animator != null)
        {
            animator.SetBool("Corriendo", false);
            animator.SetFloat("Velocidad", 0f);
        }
    }
}