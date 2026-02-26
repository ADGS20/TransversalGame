using UnityEngine;
using System.Collections;

public class TransparentObstacle : MonoBehaviour
{
    private Renderer myRenderer;
    private Coroutine currentCoroutine;

    [Header("Configuración del Fade Suave")]
    [Range(0f, 1f)] public float transparencyTarget = 0.2f; // Hasta qué punto se vuelve invisible (0 = nada, 1 = opaco)
    public float fadeSpeed = 5.0f; // Qué tan rápido ocurre el desvanecimiento

    private float fadeProgress = 1.0f;
    private Material[] materiales;
    private float[] grosoresOriginales;

    private void Start()
    {
        myRenderer = GetComponent<Renderer>();

        // Guardamos todos los materiales (el árbol y el contorno)
        materiales = myRenderer.materials;
        grosoresOriginales = new float[materiales.Length];

        // Guardamos el tamaño original del contorno por si acaso
        for (int i = 0; i < materiales.Length; i++)
        {
            if (materiales[i].HasProperty("_Grosor"))
            {
                grosoresOriginales[i] = materiales[i].GetFloat("_Grosor");
            }
        }
    }

    public void StartFadeOut()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(DoFade(transparencyTarget));
    }

    public void StartFadeIn()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(DoFade(2.0f));
    }

    private IEnumerator DoFade(float targetAlpha)
    {
        float startAlpha = fadeProgress;
        float elapsed = 0f;

        while (elapsed < 1.0f)
        {
            elapsed += Time.deltaTime * fadeSpeed;
            // Interpolación matemática para que el cambio sea súper suave
            fadeProgress = Mathf.Lerp(startAlpha, targetAlpha, elapsed);

            AplicarFadeATodos(fadeProgress);

            yield return null; // Esperamos al siguiente frame
        }

        // Aseguramos que termine en el valor exacto
        fadeProgress = targetAlpha;
        AplicarFadeATodos(fadeProgress);
    }

    private void AplicarFadeATodos(float alphaGlobal)
    {
        // Revisamos TODOS los materiales uno por uno (Árbol y Contorno)
        for (int i = 0; i < materiales.Length; i++)
        {
            Material mat = materiales[i];

            // 1. Si el material tiene "Opacidad" (ahora ambos la tienen), se desvanece suavemente
            if (mat.HasProperty("_Opacidad"))
            {
                mat.SetFloat("_Opacidad", alphaGlobal);
            }

            // 2. Si el material tiene "Grosor", lo encogemos un poco para que el borde no se vea feo al ser transparente
            if (mat.HasProperty("_Grosor"))
            {
                float porcentaje = (alphaGlobal - transparencyTarget) / (1.0f - transparencyTarget);
                porcentaje = Mathf.Clamp01(porcentaje);
                mat.SetFloat("_Grosor", grosoresOriginales[i] * porcentaje);
            }
        }
    }
}