using UnityEngine;

/// <summary>
/// Sistema de cámara orbital que gira en incrementos de 90 grados con teclas E y Q
/// CORREGIDO: Ahora anguloActual = 0 significa rotación Y = 0
/// </summary>
public class CameraOrbital : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    [Tooltip("Distancia de la cámara al objetivo")]
    [SerializeField] private float distancia = 10f;

    [Tooltip("Altura de la cámara sobre el objetivo")]
    [SerializeField] private float altura = 8f;

    [Header("Configuración de Rotación")]
    [Tooltip("Tecla para rotar a la derecha")]
    [SerializeField] private KeyCode teclaRotarDerecha = KeyCode.E;

    [Tooltip("Tecla para rotar a la izquierda")]
    [SerializeField] private KeyCode teclaRotarIzquierda = KeyCode.Q;

    [Tooltip("Velocidad de rotación suave")]
    [SerializeField] private float velocidadRotacion = 5f;

    [Header("Suavizado")]
    [Tooltip("Velocidad de seguimiento del objetivo")]
    [SerializeField] private float velocidadSeguimiento = 5f;

    [Tooltip("¿Usar suavizado en el seguimiento?")]
    [SerializeField] private bool usarSuavizado = true;

    // Variables privadas
    private Transform objetivo;
    private float anguloActual = 0f;
    private float anguloObjetivo = 0f;
    private bool estaRotando = false;
    private Vector3 posicionObjetivoSuavizada;

    void Start()
    {
        // No buscar objetivo automáticamente
    }

    void Update()
    {
        if (objetivo == null) return;

        // Detectar input de teclas para rotar
        DetectarRotacion();

        // Actualizar posición suavizada del objetivo
        if (usarSuavizado)
        {
            posicionObjetivoSuavizada = Vector3.Lerp(
                posicionObjetivoSuavizada,
                objetivo.position,
                velocidadSeguimiento * Time.deltaTime
            );
        }
        else
        {
            posicionObjetivoSuavizada = objetivo.position;
        }

        // Interpolar ángulo actual hacia el objetivo
        anguloActual = Mathf.LerpAngle(anguloActual, anguloObjetivo, velocidadRotacion * Time.deltaTime);

        // Actualizar posición de la cámara
        ActualizarPosicionCamara();

        // Verificar si terminó la rotación
        if (estaRotando && Mathf.Abs(Mathf.DeltaAngle(anguloActual, anguloObjetivo)) < 0.1f)
        {
            anguloActual = anguloObjetivo;
            estaRotando = false;
        }
    }

    private void DetectarRotacion()
    {
        // Solo detectar si no está rotando actualmente
        if (estaRotando) return;

        // Rotar a la derecha con tecla E
        if (Input.GetKeyDown(teclaRotarDerecha))
        {
            RotarCamara(90f);
        }
        // Rotar a la izquierda con tecla Q
        else if (Input.GetKeyDown(teclaRotarIzquierda))
        {
            RotarCamara(-90f);
        }
    }

    private void RotarCamara(float incremento)
    {
        anguloObjetivo += incremento;
        anguloObjetivo = NormalizarAngulo(anguloObjetivo);
        estaRotando = true;

        Debug.Log($"🎥 Cámara rotando a {anguloObjetivo}°");
    }

    private void ActualizarPosicionCamara()
    {
        // IMPORTANTE: Restar 180° para que anguloActual=0 resulte en Y=0
        float anguloCorregido = anguloActual - 180f;
        float anguloRad = anguloCorregido * Mathf.Deg2Rad;

        // Calcular offset de la cámara
        Vector3 offset = new Vector3(
            Mathf.Sin(anguloRad) * distancia,
            altura,
            Mathf.Cos(anguloRad) * distancia
        );

        transform.position = posicionObjetivoSuavizada + offset;

        // Mirar hacia el objetivo
        Vector3 direccion = posicionObjetivoSuavizada - transform.position;
        Quaternion rotacionMirar = Quaternion.LookRotation(direccion);
        transform.rotation = rotacionMirar;

        Debug.Log($"📐 Ángulo interno: {anguloActual}° | Rotación Y cámara: {transform.eulerAngles.y}°");
    }

    private float NormalizarAngulo(float angulo)
    {
        angulo = angulo % 360f;
        if (angulo < 0f)
        {
            angulo += 360f;
        }
        return angulo;
    }

    public void CambiarObjetivo(Transform nuevoObjetivo)
    {
        objetivo = nuevoObjetivo;
        if (objetivo != null)
        {
            posicionObjetivoSuavizada = objetivo.position;
        }
    }

    public float ObtenerAnguloActual()
    {
        return anguloActual;
    }

    public bool EstaRotando()
    {
        return estaRotando;
    }

    public Vector3 ObtenerDireccionForward()
    {
        float anguloRad = anguloActual * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(anguloRad), 0, Mathf.Cos(anguloRad));
    }

    public Vector3 ObtenerDireccionRight()
    {
        float anguloRad = anguloActual * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(anguloRad), 0, -Mathf.Sin(anguloRad));
    }
}