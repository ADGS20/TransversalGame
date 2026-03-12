using UnityEngine;

public class OndaExpansiva : MonoBehaviour
{
    [Header("Configuración de la Expansión")]
    public float radioMaximo = 15f;
    public float velocidadExpansion = 10f;

    [Header("Visual (Opcional pero recomendado)")]
    public Transform esferaVisual; // Un objeto 3D Esfera con un material semitransparente

    private float radioActual = 0f;
    private bool estaExpandiendo = false;

    void Start()
    {
        // Asegurarnos de que el radio global empieza en 0 para los shaders
        Shader.SetGlobalFloat("_RadioExpansion", 0f);

        if (esferaVisual != null)
            esferaVisual.localScale = Vector3.zero;
    }

    void Update()
    {
        // Sincronizamos las teclas V y C con tu otro script para lanzar la onda al mismo tiempo
        if (Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.C))
        {
            EmpezarExpansion();
        }

        // Lógica matemática de la esfera creciendo
        if (estaExpandiendo)
        {
            radioActual += velocidadExpansion * Time.deltaTime;

            if (radioActual >= radioMaximo)
            {
                radioActual = radioMaximo;
                estaExpandiendo = false; // Se detiene al llegar al máximo
            }

            // 1. Actualizamos el tamaño del objeto visual
            if (esferaVisual != null)
            {
                esferaVisual.localScale = Vector3.one * (radioActual * 2);
            }

            // 2. LA MAGIA: Enviamos la posición y el radio a TODOS los shaders a la vez
            Shader.SetGlobalVector("_PosicionOnda", transform.position);
            Shader.SetGlobalFloat("_RadioExpansion", radioActual);
        }
        else if (radioActual > 0 && Input.GetKeyUp(KeyCode.V) == false)
        {
            // Opcional: Si quieres que la onda se contraiga al volver a pulsar, 
            // puedes añadir lógica aquí para reducir el radio.
        }
    }

    public void EmpezarExpansion()
    {
        estaExpandiendo = true;
        radioActual = 0f; // Reiniciamos el radio para que empiece desde el jugador

        if (esferaVisual != null)
        {
            esferaVisual.gameObject.SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {
        // Para ver hasta dónde llega el radio máximo en el editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioMaximo);
    }
}