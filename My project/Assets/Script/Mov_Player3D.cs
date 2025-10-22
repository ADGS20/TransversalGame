using UnityEngine;

public class Mov_Player3D : MonoBehaviour
{

    /// <summary>
    /// Script de movimiento básico para jugador en 3D
    /// Movimiento WASD, rotación con mouse, y salto simple
    /// </summary>

    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad de movimiento del jugador")]
    [SerializeField] private float velocidad = 5f;

    [Tooltip("Fuerza del salto")]
    [SerializeField] private float fuerzaSalto = 5f;

    [Header("Configuración de Cámara")]
    [Tooltip("Sensibilidad del mouse")]
    [SerializeField] private float sensibilidadMouse = 2f;

    [Tooltip("Cámara del jugador (arrastra aquí la cámara)")]
    [SerializeField] private Transform camara;

    // Variables privadas
    private Rigidbody rb;
    private float rotacionX = 0f;
    private bool enSuelo = true;


    /// <summary>
    /// Inicialización
    /// Configura el cursor y obtiene componentes
    /// </summary>
    /// 
    void Start()
    {
        // Obtener Rigidbody
        rb = GetComponent<Rigidbody>();

        // Bloquear el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Se ejecuta cada frame
    /// Controla el input y la cámara
    /// </summary>    
     
    void Update()
    {
        // Capturar input de movimiento (WASD o Flechas)
        float movimientoX = Input.GetAxisRaw("Horizontal"); // A/D
        float movimientoZ = Input.GetAxisRaw("Vertical");   // W/S

        // Calcular dirección de movimiento relativa al jugador
        Vector3 direccion = transform.right * movimientoX + transform.forward * movimientoZ;
        direccion.Normalize();

        // Mover al jugador
        transform.position += direccion * velocidad * Time.deltaTime;

        // Salto con Espacio
        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            rb.velocity = new Vector3(rb.velocity.x, fuerzaSalto, rb.velocity.z);
            enSuelo = false;
        }

        // Rotar cámara con el mouse
        RotarCamara();
    }

    /// <summary>
    /// Controla la rotación de la cámara con el mouse
    /// </summary>
    private void RotarCamara()
    {
        // Obtener movimiento del mouse
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        // Rotar el jugador horizontalmente (izquierda/derecha)
        transform.Rotate(Vector3.up * mouseX);

        // Rotar la cámara verticalmente (arriba/abajo)
        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -90f, 90f); // Limitar la rotación

        if (camara != null)
        {
            camara.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
        }
    }

    /// <summary>
    /// Detecta cuando el jugador toca el suelo
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // Si toca algo, puede saltar de nuevo
        enSuelo = true;
    }

}
