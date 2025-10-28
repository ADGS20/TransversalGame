using UnityEngine;

/// <summary>
/// Script de movimiento para jugador en 2.5D
/// Movimiento en plano XZ (suelo) con cámara ortográfica
/// Estilo Pokémon Blanco/Negro o Zelda
/// </summary>

public class Mov_Player3D : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad de movimiento del jugador")]
    [SerializeField] private float velocidad = 5f;

    [Header("Configuración de Rotación")]
    [Tooltip("Velocidad de rotación del personaje")]
    [SerializeField] private float velocidadRotacion = 10f;

    [Header("Animación (Opcional)")]
    [SerializeField] private Animator animator;

    // Variables privadas para almacenar el input del jugador
    private float movimientoHorizontal;
    private float movimientoVertical;
    private Vector3 direccionMovimiento;
    private CharacterController characterController;

    void Start()
    {
        // Obtener CharacterController si existe
        characterController = GetComponent<CharacterController>();

        // Si no tiene CharacterController, añadirlo
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.center = new Vector3(0, 1, 0);
            characterController.radius = 0.5f;
            characterController.height = 2f;
        }

        // Obtener animator si existe
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Se ejecuta cada frame
    /// Captura el input y mueve al jugador
    /// </summary>
    void Update()
    {
        // Capturar input horizontal (A/D o Flechas Izquierda/Derecha)
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");

        // Capturar input vertical (W/S o Flechas Arriba/Abajo)
        movimientoVertical = Input.GetAxisRaw("Vertical");

        // Crear vector de dirección en el plano XZ (suelo)
        // X = izquierda/derecha, Z = adelante/atrás, Y = 0 (sin altura)
        direccionMovimiento = new Vector3(movimientoHorizontal, 0, movimientoVertical);

        // Normalizar para que el movimiento diagonal no sea más rápido
        if (direccionMovimiento.magnitude > 1)
        {
            direccionMovimiento.Normalize();
        }

        // Mover al jugador usando CharacterController
        characterController.Move(direccionMovimiento * velocidad * Time.deltaTime);

        // Rotar al jugador hacia la dirección de movimiento
        if (direccionMovimiento != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
        }

        // Actualizar animaciones (si tienes animator)
        if (animator != null)
        {
            // Parámetro "Velocidad" en el Animator (0 = idle, 1 = caminando)
            animator.SetFloat("Velocidad", direccionMovimiento.magnitude);
        }
    }
}
