using UnityEngine;

public class OndaExpansiva : MonoBehaviour
{
    [Header("Configuración de la Expansión")]
    public float radioMaximo = 15f;
    public float velocidadExpansion = 10f;

    [Header("Visual (Opcional)")]
    public Transform esferaVisual; // La esfera 3D semitransparente

    private float radioActual = 0f;
    private bool estaExpandiendo = false;

    void Start()
    {
        Shader.SetGlobalFloat("_RadioExpansion", 0f);
        if (esferaVisual != null) esferaVisual.localScale = Vector3.zero;
    }

    void Update()
    {
        // Escuchamos las mismas teclas que tu HabilidadShaderController
        if (Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.C))
        {
            EmpezarExpansion();
        }

        if (estaExpandiendo)
        {
            radioActual += velocidadExpansion * Time.deltaTime;

            if (radioActual >= radioMaximo)
            {
                radioActual = radioMaximo;
                estaExpandiendo = false;
            }

            if (esferaVisual != null)
            {
                esferaVisual.localScale = Vector3.one * (radioActual * 2);
            }

            // Aquí enviamos la info a TODOS los shaders a la vez
            Shader.SetGlobalVector("_PosicionOnda", transform.position);
            Shader.SetGlobalFloat("_RadioExpansion", radioActual);
        }
    }

    public void EmpezarExpansion()
    {
        estaExpandiendo = true;
        radioActual = 0f;

        if (esferaVisual != null)
            esferaVisual.gameObject.SetActive(true);
    }
}