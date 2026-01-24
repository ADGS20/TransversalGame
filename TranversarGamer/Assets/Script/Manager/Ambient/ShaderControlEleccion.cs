//------------Creador de este script------------//
//---- Hecho por: Andres Diaz Guerrero Soto ----//
//----------------------------------------------//

using UnityEngine;
using System.Collections;

public class ShaderControlEleccion : MonoBehaviour
{
    public Material materialCuracion;      // Material con shader de curación
    public Material materialCorrupcion;    // Material con shader de corrupción
    public SpriteRenderer objetoObjetivo;  // Sprite al que se le aplicará el shader
    public float duracionEfecto = 2f;      // Duración de la animación del shader

    // Activa el shader de curación en el objeto objetivo
    public void ActivarCuracion()
    {
        if (objetoObjetivo != null && materialCuracion != null)
        {
            objetoObjetivo.material = materialCuracion;

            // Animar el parámetro del shader desde 0 a 1
            StartCoroutine(AnimarShader(materialCuracion, 0f, 1f));
        }
    }

    // Activa el shader de corrupción en el objeto objetivo
    public void ActivarCorrupcion()
    {
        if (objetoObjetivo != null && materialCorrupcion != null)
        {
            objetoObjetivo.material = materialCorrupcion;

            // Animar el parámetro del shader desde 0 a 1
            StartCoroutine(AnimarShader(materialCorrupcion, 0f, 1f));
        }
    }

    // Corrutina que anima el valor del shader "_Progreso"
    IEnumerator AnimarShader(Material mat, float inicio, float fin)
    {
        float tiempo = 0f;

        while (tiempo < duracionEfecto)
        {
            tiempo += Time.deltaTime;

            float progreso = Mathf.Lerp(inicio, fin, tiempo / duracionEfecto);

            // Actualizar parámetro del shader
            mat.SetFloat("_Progreso", progreso);

            yield return null;
        }

        // Asegurar valor final
        mat.SetFloat("_Progreso", fin);
    }
}
