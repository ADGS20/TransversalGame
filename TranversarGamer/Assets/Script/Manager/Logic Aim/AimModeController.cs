//------------Creador de este script------------//
//---- Hecho por: Andres Diaz Guerrero Soto ----//
//----------------------------------------------//

using UnityEngine;

public class AimModeController : MonoBehaviour
{
    [Header("Cámaras")]
    public CameraOrbital camaraOrbital;   // Cámara orbital usada normalmente
    public Camera fpsCamera;              // Cámara usada en modo apuntado

    [Header("Jugador y disparo")]
    public Mov_Player3D playerMovement;   // Controlador del jugador
    public Transform firePoint;           // Punto desde donde se dispara la piedra
    public GameObject stonePrefab;        // Prefab del proyectil
    public float fuerzaDisparo = 15f;     // Fuerza aplicada al proyectil

    [Header("Jugador visual")]
    public GameObject jugadorVisual;      // Modelo o sprite del jugador visible en tercera persona

    [Header("Trayectoria")]
    public LineRenderer lineRenderer;     // Línea que muestra la trayectoria del disparo
    public int puntos = 30;               // Cantidad de puntos de la curva
    public float tiempoEntrePuntos = 0.1f;

    [Header("Rotación FPS")]
    public float sensibilidadX = 2f;      // Sensibilidad horizontal del ratón
    public float sensibilidadY = 2f;      // Sensibilidad vertical del ratón
    public float limiteYMin = -60f;       // Límite inferior del ángulo vertical
    public float limiteYMax = 60f;        // Límite superior del ángulo vertical

    private bool apuntando = false;       // Indica si el jugador está en modo apuntado
    private float rotacionY = 0f;         // Rotación horizontal acumulada
    private float rotacionX = 0f;         // Rotación vertical acumulada

    void Start()
    {
        // Desactivar cámara FPS al inicio
        if (fpsCamera != null)
            fpsCamera.gameObject.SetActive(false);

        // Configurar LineRenderer si existe
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            lineRenderer.useWorldSpace = true;
        }
    }

    void Update()
    {
        // Activar modo apuntado al presionar botón derecho
        if (Input.GetMouseButtonDown(1))
            ActivarModoApuntar();

        // Desactivar modo apuntado al soltar botón derecho
        if (Input.GetMouseButtonUp(1))
            DesactivarModoApuntar();

        if (apuntando)
        {
            RotarFPS();
            MostrarTrayectoria();

            // Disparo con clic izquierdo
            if (Input.GetMouseButtonDown(0))
                Disparar();
        }
        else
        {
            OcultarTrayectoria();
        }
    }

    void ActivarModoApuntar()
    {
        apuntando = true;

        // Bloquear movimiento del jugador
        if (playerMovement != null)
        {
            playerMovement.controlesBloqueados = true;
            playerMovement.ForzarIdle();
        }

        // Ocultar modelo del jugador si no es el mismo objeto que la línea
        if (jugadorVisual != null && (lineRenderer == null || jugadorVisual != lineRenderer.gameObject))
            jugadorVisual.SetActive(false);

        // Obtener rotación actual de la cámara orbital
        float yaw = camaraOrbital.ObtenerAnguloActual();

        // Configurar cámara FPS con la rotación horizontal correcta
        fpsCamera.transform.localRotation = Quaternion.identity;
        fpsCamera.transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        rotacionX = 0f;
        rotacionY = 0f;

        // Cambiar cámaras
        camaraOrbital.gameObject.SetActive(false);
        fpsCamera.gameObject.SetActive(true);

        // Bloquear cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void DesactivarModoApuntar()
    {
        apuntando = false;

        // Restaurar movimiento del jugador
        if (playerMovement != null)
            playerMovement.controlesBloqueados = false;

        // Mostrar modelo del jugador si corresponde
        if (jugadorVisual != null && (lineRenderer == null || jugadorVisual != lineRenderer.gameObject))
            jugadorVisual.SetActive(true);

        // Restaurar cámaras
        fpsCamera.gameObject.SetActive(false);
        camaraOrbital.gameObject.SetActive(true);

        // Restaurar cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void RotarFPS()
    {
        if (fpsCamera == null) return;

        // Lectura del ratón
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadX;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadY;

        // Acumular rotación
        rotacionY += mouseX;
        rotacionX -= mouseY;

        // Limitar rotación vertical
        rotacionX = Mathf.Clamp(rotacionX, limiteYMin, limiteYMax);

        // Rotar jugador horizontalmente
        playerMovement.transform.rotation =
            Quaternion.Euler(0f, camaraOrbital.ObtenerAnguloActual() + rotacionY, 0f);

        // Rotar cámara verticalmente
        fpsCamera.transform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        // Alinear firePoint con la cámara
        if (firePoint != null)
            firePoint.rotation = fpsCamera.transform.rotation;
    }

    void Disparar()
    {
        // Validar prefab
        if (stonePrefab == null)
        {
            Debug.LogError("stonePrefab no está asignado.");
            return;
        }

        // Instanciar proyectil
        GameObject piedra = Instantiate(stonePrefab, firePoint.position, firePoint.rotation);

        if (piedra == null)
        {
            Debug.LogError("Error al instanciar la piedra.");
            return;
        }

        // Obtener Rigidbody
        Rigidbody rb = piedra.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("El prefab de la piedra no tiene Rigidbody.");
            return;
        }

        // Aplicar fuerza de disparo
        rb.AddForce(firePoint.forward * fuerzaDisparo, ForceMode.Impulse);
    }

    void MostrarTrayectoria()
    {
        if (lineRenderer == null || firePoint == null) return;

        lineRenderer.enabled = true;

        Vector3 posInicial = firePoint.position;
        Vector3 velocidadInicial = firePoint.forward * fuerzaDisparo;

        Vector3[] puntosTrayectoria = new Vector3[puntos];

        // Calcular puntos de la parábola
        for (int i = 0; i < puntos; i++)
        {
            float t = i * tiempoEntrePuntos;

            Vector3 punto =
                posInicial +
                velocidadInicial * t +
                0.5f * Physics.gravity * (t * t);

            puntosTrayectoria[i] = punto;
        }

        lineRenderer.positionCount = puntos;
        lineRenderer.SetPositions(puntosTrayectoria);
    }

    void OcultarTrayectoria()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }
}