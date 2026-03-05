using UnityEngine;

public class ObstaculoInteligente : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;

    [Header("Ajustes de Transparencia")]
    public float opacidadNormal = 2.0f;     // Estado sólido
    public float opacidadTapa = 1f;         // Estado semi-transparente (rejilla)
    public float velocidadFade = 5f;        // Velocidad de la transición

    [Header("Ajustes de Shader Graph")]
    [Tooltip("El nombre exacto del Reference en el Blackboard de tu Shader Graph (ej. _Opacity o _Opacidad)")]
    public string propiedadShader = "_Opacity";

    private Material[] materiales;
    private Collider miCollider;
    private float opacidadActual;

    void Start()
    {
        // Al usar .materials (en plural), Unity crea una instancia de todos los materiales 
        // asignados al objeto, permitiéndonos modificar el base y el contorno a la vez.
        materiales = GetComponent<Renderer>().materials;
        miCollider = GetComponent<Collider>();
        opacidadActual = opacidadNormal;
    }

    void Update()
    {
        // Medida de seguridad por si no se ha asignado el jugador
        if (jugador == null) return;

        Transform cam = Camera.main.transform;
        Vector3 dir = jugador.position - cam.position;
        float distAJugador = Vector3.Distance(cam.position, jugador.position);

        bool debeSerTransparente = false;

        // 1. COMPROBACIÓN: ¿La cámara está DENTRO de mi collider?
        if (miCollider.bounds.Contains(cam.position))
        {
            debeSerTransparente = true;
        }
        else
        {
            // 2. COMPROBACIÓN: ¿Estoy estorbando la vista mediante Raycast?
            if (Physics.Raycast(cam.position, dir.normalized, out RaycastHit hit, distAJugador))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    debeSerTransparente = true;
                }
            }
        }

        // Definimos el objetivo de opacidad y lo suavizamos con Lerp
        float target = debeSerTransparente ? opacidadTapa : opacidadNormal;
        opacidadActual = Mathf.Lerp(opacidadActual, target, Time.deltaTime * velocidadFade);

        // 3. APLICAR EL EFECTO A TODOS LOS MATERIALES (Base y Contorno)
        foreach (Material mat in materiales)
        {
            if (mat.HasProperty(propiedadShader))
            {
                mat.SetFloat(propiedadShader, opacidadActual);
            }
        }
    }
}