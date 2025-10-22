using UnityEngine;

/// <summary>
/// Script de movimiento básico para jugador en 2D
/// Permite moverse en las 4 direcciones: arriba, abajo, izquierda, derecha
/// Sin física, solo movimiento directo del transform
/// </summary>

public class Mov_Player2D : MonoBehaviour
{

    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad de movimiento del jugador")]
    [SerializeField] private float velocidad = 5f;

    // Variables para almacenar el input del jugador
    private float movimientoHorizontal;
    private float movimientoVertical;

    void Start()
    {

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

        // Crear vector de dirección
        Vector2 direccion = new Vector2(movimientoHorizontal, movimientoVertical);

        // Normalizar para que el movimiento diagonal no sea más rápido
        direccion.Normalize();

        // Mover al jugador directamente (sin física)
        transform.Translate(direccion * velocidad * Time.deltaTime);
    }
}
