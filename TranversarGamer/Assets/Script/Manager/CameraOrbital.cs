//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

/// Cámara orbital con rotación en pasos (E/Q) y zoom en riel Y + distancia en plano,
/// sin interferir con la rotación/movimiento físico de los personajes.
public class CameraOrbital : MonoBehaviour
{
    [Header("Seguimiento")]
    [SerializeField] private float velocidadSeguimiento = 10f;
    [SerializeField] private bool usarSuavizado = true;

    [Header("Rotación (90°)")]
    [SerializeField] private KeyCode teclaRotarDerecha = KeyCode.E;
    [SerializeField] private KeyCode teclaRotarIzquierda = KeyCode.Q;
    [SerializeField] private float velocidadRotacion = 10f;

    [Header("Zoom (riel)")]
    [Tooltip("Altura mínima (más cerca)")]
    [SerializeField] private float yCerca = 2.75f;
    [Tooltip("Altura máxima (más lejos)")]
    [SerializeField] private float yLejos = 6.0f;

    [Tooltip("Distancia en plano mínima (más cerca). Usa valores positivos")]
    [SerializeField] private float planoCerca = 3.64f; // equivalente a |Z| 3.64
    [Tooltip("Distancia en plano máxima (más lejos). Usa valores positivos")]
    [SerializeField] private float planoLejos = 11.0f; // equivalente a |Z| 11

    [Tooltip("Factor de zoom inicial (0 = lejos, 1 = cerca)")]
    [Range(0f, 1f)]
    [SerializeField] private float zoomFactorInicial = 0.5f;

    [SerializeField] private float velocidadZoom = 1.5f;
    [SerializeField] private float suavizadoZoom = 10f;
    [SerializeField] private KeyCode teclaAcercar = KeyCode.Z;
    [SerializeField] private KeyCode teclaAlejar = KeyCode.X;

    private Transform objetivo;

    // Estado rotación
    private float anguloActual = 0f;
    private float anguloObjetivo = 0f;
    private bool estaRotando = false;

    // Estado seguimiento
    private Vector3 posSuavizada;

    // Estado zoom
    private float zoomFactorObjetivo;
    private float zoomFactorActual;

    void Start()
    {
        zoomFactorObjetivo = zoomFactorActual = Mathf.Clamp01(zoomFactorInicial);
    }

    void Update()
    {
        if (objetivo == null) return;

        DetectarRotacion();
        DetectarZoom();

        // Suavizar target
        posSuavizada = usarSuavizado
            ? Vector3.Lerp(posSuavizada, objetivo.position, velocidadSeguimiento * Time.deltaTime)
            : objetivo.position;

        // Suavizar rotación de cámara (solo el ángulo interno, no el transform del objetivo)
        anguloActual = Mathf.LerpAngle(anguloActual, anguloObjetivo, velocidadRotacion * Time.deltaTime);

        // Suavizar zoom
        zoomFactorActual = Mathf.Lerp(zoomFactorActual, zoomFactorObjetivo, suavizadoZoom * Time.deltaTime);

        // Actualizar cámara con cálculos puros
        ActualizarCamara();

        if (estaRotando && Mathf.Abs(Mathf.DeltaAngle(anguloActual, anguloObjetivo)) < 0.1f)
        {
            anguloActual = anguloObjetivo;
            estaRotando = false;
        }
    }

    private void DetectarRotacion()
    {
        if (estaRotando) return;

        if (Input.GetKeyDown(teclaRotarDerecha))
            SetRotacionObjetivo(anguloObjetivo + 90f);
        else if (Input.GetKeyDown(teclaRotarIzquierda))
            SetRotacionObjetivo(anguloObjetivo - 90f);
    }

    private void SetRotacionObjetivo(float deg)
    {
        anguloObjetivo = NormalizarAngulo(deg);
        estaRotando = true;
    }

    private void DetectarZoom()
    {
        // Rueda del ratón: + acercar, - alejar
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
            zoomFactorObjetivo += scroll * (velocidadZoom * 0.1f);

        if (Input.GetKey(teclaAcercar))
            zoomFactorObjetivo += velocidadZoom * Time.deltaTime;
        if (Input.GetKey(teclaAlejar))
            zoomFactorObjetivo -= velocidadZoom * Time.deltaTime;

        zoomFactorObjetivo = Mathf.Clamp01(zoomFactorObjetivo);
    }

    private void ActualizarCamara()
    {
        // Interpolamos altura y distancia en plano según factor
        float y = Mathf.Lerp(yLejos, yCerca, zoomFactorActual);
        float plano = Mathf.Lerp(planoLejos, planoCerca, zoomFactorActual); // positivo

        // Calcular forward plano a partir del yaw interno (sin tocar transforms externos)
        // Nuestro sistema previo usa anguloActual=0 alineado con Y=0 mirando +Z (corregido en versiones previas).
        float yawRad = (anguloActual) * Mathf.Deg2Rad;
        Vector3 forwardPlano = new Vector3(Mathf.Sin(yawRad), 0, Mathf.Cos(yawRad)); // normalizado
        Vector3 rightPlano = new Vector3(Mathf.Cos(yawRad), 0, -Mathf.Sin(yawRad));  // por si se necesita

        // Offset: nos colocamos "plano" unidades detrás del objetivo respecto a su forward (alejado de él)
        // Detrás = -forwardPlano
        Vector3 offsetPlano = -forwardPlano * plano;

        // Posición final: riel Y y desplazamiento en plano con el yaw actual
        Vector3 camPos = posSuavizada + offsetPlano;
        camPos.y = posSuavizada.y + y; // altura relativa al objetivo

        transform.position = camPos;

        // Mirar al objetivo sin tocar su rotación
        Vector3 dir = posSuavizada - transform.position;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    private float NormalizarAngulo(float angulo)
    {
        angulo %= 360f;
        if (angulo < 0f) angulo += 360f;
        return angulo;
    }

    // API pública
    public void CambiarObjetivo(Transform nuevoObjetivo)
    {
        objetivo = nuevoObjetivo;
        if (objetivo != null)
            posSuavizada = objetivo.position;
    }

    public float ObtenerAnguloActual() => anguloActual;

    public bool EstaRotando() => estaRotando;

    public Vector3 ObtenerDireccionForward()
    {
        float yawRad = anguloActual * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(yawRad), 0, Mathf.Cos(yawRad));
    }

    public Vector3 ObtenerDireccionRight()
    {
        float yawRad = anguloActual * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(yawRad), 0, -Mathf.Sin(yawRad));
    }
}