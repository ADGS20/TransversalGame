//------------Creador de este script------------//
//---- Hecho por: Andres Diaz Guerrero Soto ----//
//----------------------------------------------//
using UnityEngine;

public class AimModeController : MonoBehaviour
{
    [Header("Cámaras")]
    public CameraOrbital camaraOrbital;
    public Camera fpsCamera;

    [Header("Jugador y disparo")]
    public Mov_Player3D playerMovement;
    public Transform firePoint;
    public GameObject stonePrefab;
    public float fuerzaDisparo = 15f;

    [Header("Jugador visual")]
    public GameObject jugadorVisual; // sprite o modelo del jugador

    [Header("Trayectoria")]
    public LineRenderer lineRenderer;
    public int puntos = 30;
    public float tiempoEntrePuntos = 0.1f;

    [Header("Rotación FPS")]
    public float sensibilidadX = 2f;
    public float sensibilidadY = 2f;
    public float limiteYMin = -60f;
    public float limiteYMax = 60f;

    private bool apuntando = false;
    private float rotacionY = 0f; // yaw relativo
    private float rotacionX = 0f; // pitch

    void Start()
    {
        if (fpsCamera != null)
            fpsCamera.gameObject.SetActive(false);

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            lineRenderer.useWorldSpace = true; // 🔥 importante para que los puntos en mundo se vean bien
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            ActivarModoApuntar();

        if (Input.GetMouseButtonUp(1))
            DesactivarModoApuntar();

        if (apuntando)
        {
            RotarFPS();
            MostrarTrayectoria();

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

        if (playerMovement != null)
        {
            playerMovement.controlesBloqueados = true;
            playerMovement.ForzarIdle();
        }

        // 🔥 Solo ocultar jugadorVisual si NO es el mismo objeto que el LineRenderer
        if (jugadorVisual != null && (lineRenderer == null || jugadorVisual != lineRenderer.gameObject))
            jugadorVisual.SetActive(false);

        // Obtener rotación REAL de la cámara orbital
        float yaw = camaraOrbital.ObtenerAnguloActual();

        // No movemos la cámara, solo la rotamos
        fpsCamera.transform.localRotation = Quaternion.identity;
        fpsCamera.transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        rotacionX = 0f;
        rotacionY = 0f;

        camaraOrbital.gameObject.SetActive(false);
        fpsCamera.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void DesactivarModoApuntar()
    {
        apuntando = false;

        if (playerMovement != null)
            playerMovement.controlesBloqueados = false;

        // 🔥 Solo reactivar jugadorVisual si NO es el mismo objeto que el LineRenderer
        if (jugadorVisual != null && (lineRenderer == null || jugadorVisual != lineRenderer.gameObject))
            jugadorVisual.SetActive(true);

        fpsCamera.gameObject.SetActive(false);
        camaraOrbital.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void RotarFPS()
    {
        if (fpsCamera == null) return;

        float mouseX = Input.GetAxis("Mouse X") * sensibilidadX;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadY;

        rotacionY += mouseX;
        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, limiteYMin, limiteYMax);

        // Rotar jugador en Y
        playerMovement.transform.rotation = Quaternion.Euler(0f, camaraOrbital.ObtenerAnguloActual() + rotacionY, 0f);

        // Rotar cámara solo en X local
        fpsCamera.transform.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        if (firePoint != null)
            firePoint.rotation = fpsCamera.transform.rotation;
    }

    void Disparar()
    {
        if (stonePrefab == null)
        {
            Debug.LogError("❌ stonePrefab NO está asignado.");
            return;
        }

        GameObject piedra = Instantiate(stonePrefab, firePoint.position, firePoint.rotation);

        if (piedra == null)
        {
            Debug.LogError("❌ Error al instanciar la piedra.");
            return;
        }

        Rigidbody rb = piedra.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("❌ El prefab de la piedra NO tiene Rigidbody.");
            return;
        }

        rb.AddForce(firePoint.forward * fuerzaDisparo, ForceMode.Impulse);
    }

    void MostrarTrayectoria()
    {
        if (lineRenderer == null || firePoint == null) return;

        lineRenderer.enabled = true;

        Vector3 posInicial = firePoint.position;
        Vector3 velocidadInicial = firePoint.forward * fuerzaDisparo;

        Vector3[] puntosTrayectoria = new Vector3[puntos];

        for (int i = 0; i < puntos; i++)
        {
            float t = i * tiempoEntrePuntos;

            Vector3 punto = posInicial +
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
