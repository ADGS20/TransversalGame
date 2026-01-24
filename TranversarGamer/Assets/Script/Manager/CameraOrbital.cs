//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;

/// <summary>
/// Cámara orbital con rotación en pasos (teclado y mando) y zoom
/// basado en altura Y y distancia en plano, sin interferir con
/// la rotación/movimiento físico de los personajes.
/// </summary>
public class CameraOrbital : MonoBehaviour
{
    [Header("Seguimiento")]
    [SerializeField] private float velocidadSeguimiento = 10f; // Velocidad a la que la cámara sigue al objetivo
    [SerializeField] private bool usarSuavizado = true;        // Si se interpola o no la posición del objetivo

    [Header("Rotación Teclado (90° pasos)")]
    [SerializeField] private KeyCode teclaRotarDerecha = KeyCode.E;
    [SerializeField] private KeyCode teclaRotarIzquierda = KeyCode.Q;
    [SerializeField] private float velocidadRotacion = 10f;    // Velocidad de interpolación del ángulo

    [Header("Rotación Mando (botones LB / RB)")]
    [Tooltip("Botón para rotar a la derecha (RB)")]
    [SerializeField] private KeyCode botonRotarDerechaMando = KeyCode.JoystickButton5;
    [Tooltip("Botón para rotar a la izquierda (LB)")]
    [SerializeField] private KeyCode botonRotarIzquierdaMando = KeyCode.JoystickButton4;

    [Header("Zoom (riel)")]
    [Tooltip("Altura mínima (más cerca)")]
    [SerializeField] private float yCerca = 2.75f;
    [Tooltip("Altura máxima (más lejos)")]
    [SerializeField] private float yLejos = 6.0f;

    [Tooltip("Distancia en plano mínima (más cerca). Usa valores positivos")]
    [SerializeField] private float planoCerca = 3.64f;
    [Tooltip("Distancia en plano máxima (más lejos). Usa valores positivos")]
    [SerializeField] private float planoLejos = 11.0f;

    [Tooltip("Factor de zoom inicial (0 = lejos, 1 = cerca)")]
    [Range(0f, 1f)]
    [SerializeField] private float zoomFactorInicial = 0.5f;

    [Header("Zoom Teclado/Ratón")]
    [SerializeField] private float velocidadZoom = 1.5f;   // Escala de cambio de zoom con ratón
    [SerializeField] private float suavizadoZoom = 10f;    // Suavizado del factor de zoom

    [Header("Zoom Mando (stick derecho vertical)")]
    [Tooltip("Nombre del eje vertical del stick derecho (Input Manager)")]
    [SerializeField] private string ejeStickDerechoVertical = "RightStickVertical";
    [SerializeField] private float velocidadZoomMando = 1.0f;
    [SerializeField] private float deadzoneStick = 0.2f;   // Zona muerta para el stick

    private Transform objetivo;            // Transform del objetivo a seguir

    // Estado rotación
    private float anguloActual = 0f;       // Ángulo interpolado actual
    private float anguloObjetivo = 0f;     // Ángulo objetivo (en pasos de 90°)
    private bool estaRotando = false;      // Indica si está en transición de rotación

    // Estado seguimiento
    private Vector3 posSuavizada;          // Posición suavizada del objetivo

    // Estado zoom
    private float zoomFactorObjetivo;      // Factor de zoom deseado
    private float zoomFactorActual;        // Factor de zoom interpolado

    // Flag para saber si el eje del stick está configurado
    private bool tieneStickDerechoVertical = false;

    void Start()
    {
        // Inicializar zoom
        zoomFactorObjetivo = zoomFactorActual = Mathf.Clamp01(zoomFactorInicial);

        // Detectar si el eje del stick derecho vertical existe
        tieneStickDerechoVertical = ComprobarEje(ejeStickDerechoVertical);
        if (!tieneStickDerechoVertical)
            Debug.LogWarning($"CameraOrbital: Eje '{ejeStickDerechoVertical}' no configurado. Zoom con mando deshabilitado.");
    }

    void Update()
    {
        if (objetivo == null) return;

        // Entrada de rotación (teclado y mando)
        DetectarRotacionTecladoYMando();

        // Entrada de zoom (ratón y mando)
        DetectarZoom();

        // Suavizar posición del objetivo
        posSuavizada = usarSuavizado
            ? Vector3.Lerp(posSuavizada, objetivo.position, velocidadSeguimiento * Time.deltaTime)
            : objetivo.position;

        // Suavizar rotación de la cámara (solo el ángulo interno)
        anguloActual = Mathf.LerpAngle(anguloActual, anguloObjetivo, velocidadRotacion * Time.deltaTime);

        // Suavizar zoom
        zoomFactorActual = Mathf.Lerp(zoomFactorActual, zoomFactorObjetivo, suavizadoZoom * Time.deltaTime);

        // Actualizar posición y rotación de la cámara
        ActualizarCamara();

        // Considerar que ha terminado la rotación cuando la diferencia es mínima
        if (estaRotando && Mathf.Abs(Mathf.DeltaAngle(anguloActual, anguloObjetivo)) < 0.1f)
        {
            anguloActual = anguloObjetivo;
            estaRotando = false;
        }
    }

    /// <summary>
    /// Detecta entrada de rotación tanto de teclado como de mando.
    /// </summary>
    private void DetectarRotacionTecladoYMando()
    {
        if (estaRotando) return;

        // Teclado: E (derecha), Q (izquierda)
        if (Input.GetKeyDown(teclaRotarDerecha))
        {
            SetRotacionObjetivo(anguloObjetivo + 90f);
            return;
        }
        if (Input.GetKeyDown(teclaRotarIzquierda))
        {
            SetRotacionObjetivo(anguloObjetivo - 90f);
            return;
        }

        // Mando: RB (derecha), LB (izquierda)
        if (Input.GetKeyDown(botonRotarDerechaMando))
        {
            SetRotacionObjetivo(anguloObjetivo + 90f);
            return;
        }
        if (Input.GetKeyDown(botonRotarIzquierdaMando))
        {
            SetRotacionObjetivo(anguloObjetivo - 90f);
            return;
        }
    }

    /// <summary>
    /// Establece un nuevo ángulo objetivo normalizado y marca que se está rotando.
    /// </summary>
    private void SetRotacionObjetivo(float deg)
    {
        anguloObjetivo = NormalizarAngulo(deg);
        estaRotando = true;
    }

    /// <summary>
    /// Detecta entrada de zoom desde ratón y mando, y ajusta el factor objetivo.
    /// </summary>
    private void DetectarZoom()
    {
        // Zoom con rueda del ratón
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
            zoomFactorObjetivo += scroll * (velocidadZoom * 0.1f);

        // Zoom con mando: stick derecho vertical
        if (tieneStickDerechoVertical)
        {
            float padY = ObtenerEje(ejeStickDerechoVertical);

            if (Mathf.Abs(padY) > deadzoneStick)
            {
                zoomFactorObjetivo += padY * velocidadZoomMando * Time.deltaTime;
            }
        }

        // Limitar el factor de zoom entre 0 y 1
        zoomFactorObjetivo = Mathf.Clamp01(zoomFactorObjetivo);
    }

    /// <summary>
    /// Calcula y aplica la posición y rotación finales de la cámara.
    ///</summary>
    private void ActualizarCamara()
    {
        // Interpolamos altura y distancia en plano según el factor de zoom
        float y = Mathf.Lerp(yLejos, yCerca, zoomFactorActual);
        float plano = Mathf.Lerp(planoLejos, planoCerca, zoomFactorActual); // positivo

        // Calcular forward en el plano XZ a partir del yaw interno
        float yawRad = anguloActual * Mathf.Deg2Rad;
        Vector3 forwardPlano = new Vector3(Mathf.Sin(yawRad), 0, Mathf.Cos(yawRad)); // normalizado

        // Offset: nos colocamos "plano" unidades detrás del objetivo
        Vector3 offsetPlano = -forwardPlano * plano;

        // Posición final de la cámara
        Vector3 camPos = posSuavizada + offsetPlano;
        camPos.y = posSuavizada.y + y; // altura relativa al objetivo

        transform.position = camPos;

        // Mirar al objetivo sin modificar su rotación
        Vector3 dir = posSuavizada - transform.position;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    /// <summary>
    /// Normaliza un ángulo a [0, 360).
    /// </summary>
    private float NormalizarAngulo(float angulo)
    {
        angulo %= 360f;
        if (angulo < 0f) angulo += 360f;
        return angulo;
    }

    // ---- HELPERS PARA EJES DEL MANDO ----

    /// <summary>
    /// Comprueba si un eje existe en el Input Manager.
    /// </summary>
    private bool ComprobarEje(string nombreEje)
    {
        try
        {
            Input.GetAxis(nombreEje);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Obtiene el valor de un eje, devolviendo 0 si no existe.
    /// </summary>
    private float ObtenerEje(string nombreEje)
    {
        try
        {
            return Input.GetAxis(nombreEje);
        }
        catch
        {
            return 0f;
        }
    }

    // ---- API PÚBLICA (para GameplayManager, ObjetoAlineadoCamara, etc.) ----

    /// <summary>
    /// Cambia el objetivo que la cámara sigue.
    /// </summary>
    public void CambiarObjetivo(Transform nuevoObjetivo)
    {
        objetivo = nuevoObjetivo;
        if (objetivo != null)
            posSuavizada = objetivo.position;
    }

    /// <summary>
    /// Devuelve el ángulo actual de la cámara en grados.
    /// </summary>
    public float ObtenerAnguloActual() => anguloActual;

    /// <summary>
    /// Indica si la cámara está en proceso de rotación entre pasos.
    /// </summary>
    public bool EstaRotando() => estaRotando;

    /// <summary>
    /// Devuelve el vector forward en el plano XZ según el ángulo interno.
    /// </summary>
    public Vector3 ObtenerDireccionForward()
    {
        float yawRad = anguloActual * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(yawRad), 0, Mathf.Cos(yawRad));
    }

    /// <summary>
    /// Devuelve el vector right en el plano XZ según el ángulo interno.
    /// </summary>
    public Vector3 ObtenerDireccionRight()
    {
        float yawRad = anguloActual * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(yawRad), 0, -Mathf.Sin(yawRad));
    }
}
