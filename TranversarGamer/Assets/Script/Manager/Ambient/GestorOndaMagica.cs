using UnityEngine;

public class GestorOndaMagica : MonoBehaviour
{
    [Header("Onda de Creación (Tecla V)")]
    public float radioMaximoV = 15f;
    [Tooltip("Tiempo exacto en segundos que tarda en expandirse la onda V")]
    public float segundosExpansionV = 2f;
    private float radioActualV = 0f;
    private bool expandiendoV = false;

    [Header("Onda de Corrupción (Tecla C)")]
    public float radioMaximoC = 15f;
    [Tooltip("Tiempo exacto en segundos que tarda en expandirse la onda C")]
    public float segundosExpansionC = 2f;
    private float radioActualC = 0f;
    private bool expandiendoC = false;

    void Start()
    {
        // Al empezar el juego, nos aseguramos de que ambas ondas estén en 0
        Shader.SetGlobalFloat("_RadioCreacion", 0f);
        Shader.SetGlobalFloat("_RadioCorrupcion", 0f);
    }

    void Update()
    {
        // --- INICIAR ONDA V ---
        if (Input.GetKeyDown(KeyCode.V))
        {
            expandiendoV = true;
            radioActualV = 0f;
            Shader.SetGlobalVector("_PosOndaCreacion", transform.position);
        }

        // --- INICIAR ONDA C ---
        if (Input.GetKeyDown(KeyCode.C))
        {
            expandiendoC = true;
            radioActualC = 0f;
            Shader.SetGlobalVector("_PosOndaCorrupcion", transform.position);
        }

        // --- ANIMAR ONDA V (Matemática por segundos) ---
        if (expandiendoV)
        {
            radioActualV += (radioMaximoV / segundosExpansionV) * Time.deltaTime;
            if (radioActualV >= radioMaximoV)
            {
                radioActualV = radioMaximoV;
                expandiendoV = false;
            }
            Shader.SetGlobalFloat("_RadioCreacion", radioActualV);
        }

        // --- ANIMAR ONDA C (Matemática por segundos) ---
        if (expandiendoC)
        {
            radioActualC += (radioMaximoC / segundosExpansionC) * Time.deltaTime;
            if (radioActualC >= radioMaximoC)
            {
                radioActualC = radioMaximoC;
                expandiendoC = false;
            }
            Shader.SetGlobalFloat("_RadioCorrupcion", radioActualC);
        }
    }
}