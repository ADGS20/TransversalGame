using UnityEngine;
using System.Collections;

public class TransparentObstacle : MonoBehaviour
{
    private Renderer myRenderer;
    private Coroutine currentCoroutine;
    private Material[] materiales;

    [Header("Configuración del Recorte")]
    public float tamanoMaximo = 0.2f;
    public float velocidadFade = 2.0f;

    [Header("Configuración del Contorno")]
    public bool esconderContornoAlRecortar = true;
    private float[] grosoresOriginales;

    [HideInInspector]
    public Transform personajeOculto;
    private float tamanoActual = 0f;

    private void Start()
    {
        myRenderer = GetComponent<Renderer>();
        materiales = myRenderer.materials;
        grosoresOriginales = new float[materiales.Length];

        // Guardamos los grosores iniciales para poder restaurarlos
        for (int i = 0; i < materiales.Length; i++)
        {
            if (materiales[i].HasProperty("_Grosor"))
            {
                grosoresOriginales[i] = materiales[i].GetFloat("_Grosor");
            }
        }
    }

    private void Update()
    {
        if (personajeOculto != null && tamanoActual > 0.01f)
        {
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(personajeOculto.position);
            viewportPos.y += 0.05f;

            foreach (Material mat in materiales)
            {
                if (mat.HasProperty("_PosicionPantalla"))
                    mat.SetVector("_PosicionPantalla", (Vector2)viewportPos);
            }
        }
    }

    public void StartFadeOut()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(AnimarEfecto(tamanoMaximo));
    }

    public void StartFadeIn()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(AnimarEfecto(0f));
    }

    private IEnumerator AnimarEfecto(float targetTamano)
    {
        while (!Mathf.Approximately(tamanoActual, targetTamano))
        {
            tamanoActual = Mathf.MoveTowards(tamanoActual, targetTamano, Time.deltaTime * velocidadFade);

            for (int i = 0; i < materiales.Length; i++)
            {
                // 1. Actualizamos el recorte (Stencil/X-Ray)
                if (materiales[i].HasProperty("_TamanoCuadro"))
                    materiales[i].SetFloat("_TamanoCuadro", tamanoActual);

                // 2. Solo si está activado, ocultamos el contorno proporcionalmente
                if (esconderContornoAlRecortar && materiales[i].HasProperty("_Grosor"))
                {
                    // Si el hueco crece, el grosor baja a 0
                    float factor = 1.0f - (tamanoActual / tamanoMaximo);
                    materiales[i].SetFloat("_Grosor", grosoresOriginales[i] * factor);
                }
            }
            yield return null;
        }
    }
}