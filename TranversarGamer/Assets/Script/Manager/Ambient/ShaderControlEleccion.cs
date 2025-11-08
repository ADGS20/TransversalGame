using UnityEngine;
using System.Collections;

public class ShaderControlEleccion : MonoBehaviour
{
    public Material materialCuracion;
    public Material materialCorrupcion;
    public SpriteRenderer objetoObjetivo; // Cambiado a SpriteRenderer para 2D
    public float duracionEfecto = 2f;

    public void ActivarCuracion()
    {
        if (objetoObjetivo != null && materialCuracion != null)
        {
            objetoObjetivo.material = materialCuracion;
            StartCoroutine(AnimarShader(materialCuracion, 0f, 1f));
            Debug.Log("🌿 Shader de Curación activado");
        }
    }

    public void ActivarCorrupcion()
    {
        if (objetoObjetivo != null && materialCorrupcion != null)
        {
            objetoObjetivo.material = materialCorrupcion;
            StartCoroutine(AnimarShader(materialCorrupcion, 0f, 1f));
            Debug.Log("🔥 Shader de Corrupción activado");
        }
    }

    IEnumerator AnimarShader(Material mat, float inicio, float fin)
    {
        float tiempo = 0f;
        while (tiempo < duracionEfecto)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Lerp(inicio, fin, tiempo / duracionEfecto);
            mat.SetFloat("_Progreso", progreso); // Nombre de la propiedad en el shader
            yield return null;
        }
        mat.SetFloat("_Progreso", fin);
    }
}