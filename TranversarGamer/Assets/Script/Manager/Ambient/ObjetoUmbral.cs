using UnityEngine;

public class ObjetoUmbral : MonoBehaviour
{
    private Collider miCollider;
    private bool yaAparecio = false;

    void Start()
    {
        // 1. Apagamos las físicas al empezar para que no choques con el aire invisible
        miCollider = GetComponent<Collider>();
        if (miCollider != null)
        {
            miCollider.enabled = false;
        }
    }

    void Update()
    {
        // Si ya apareció, no hacemos nada más
        if (yaAparecio) return;

        // 2. Le preguntamos al mundo de Unity: ¿Por dónde va la onda del jugador?
        float radioDeLaOnda = Shader.GetGlobalFloat("_RadioExpansion");
        Vector3 centroDeLaOnda = Shader.GetGlobalVector("_PosicionOnda");

        // 3. Si la onda ya empezó a crecer...
        if (radioDeLaOnda > 0.1f)
        {
            // Calculamos a qué distancia estamos del centro de la onda
            float miDistancia = Vector3.Distance(transform.position, centroDeLaOnda);

            // 4. Si la onda nos alcanzó, ¡nos volvemos sólidos!
            if (miDistancia <= radioDeLaOnda)
            {
                HacerSolido();
            }
        }
    }

    void HacerSolido()
    {
        yaAparecio = true;
        if (miCollider != null)
        {
            miCollider.enabled = true; // ¡Ya puedes pisar el objeto!
        }
        Debug.Log("¡Un objeto acaba de ser revelado por la onda!");
    }
}