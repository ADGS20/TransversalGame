using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HabilidadShaderController : MonoBehaviour
{
    [Header("Configuración de Curación")]
    public List<SpriteRenderer> objetosCuracion = new List<SpriteRenderer>();
    public Material materialCuracion;
    public float duracionCuracion = 2f;

    [Header("Configuración de Corrupción")]
    public List<SpriteRenderer> objetosCorrupcion = new List<SpriteRenderer>();
    public Material materialCorrupcion;
    public float duracionCorrupcion = 2f;

    [Header("UI")]
    public GameObject canvasHabilidades;
    public GameObject botonCuracion;
    public GameObject botonCorrupcion;

    [Header("Tipo de Zona")]
    public TipoZona tipoZona;

    public enum TipoZona
    {
        Curacion,
        Corrupcion
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canvasHabilidades != null && canvasHabilidades.activeSelf)
        {
            canvasHabilidades.SetActive(false);
        }

        // Usar el estado global de influencia en lugar de buscar InfluenceUIController
        if (InfluenceState.Instance != null)
        {
            var estado = InfluenceState.Instance.CurrentEstado;

            // Ejemplo de uso: si quieres mostrar/ocultar botones según estado global
            // (aquí solo se deja como referencia; no cambia la lógica de zona)
            if (estado == InfluenceState.EstadoInfluencia.Corrupto)
            {
                // lógica específica si hace falta
            }
        }
    }

    public void UsarHabilidadCuracion()
    {
        if (objetosCuracion.Count > 0 && materialCuracion != null)
        {
            foreach (SpriteRenderer obj in objetosCuracion)
            {
                if (obj != null)
                {
                    obj.material = materialCuracion;
                    StartCoroutine(AnimarShader(obj.material, 1f, 0f, duracionCuracion));
                }
            }
        }

        if (canvasHabilidades != null)
            canvasHabilidades.SetActive(false);
    }

    public void UsarHabilidadCorrupcion()
    {
        if (objetosCorrupcion.Count > 0 && materialCorrupcion != null)
        {
            foreach (SpriteRenderer obj in objetosCorrupcion)
            {
                if (obj != null)
                {
                    obj.material = materialCorrupcion;
                    StartCoroutine(AnimarShader(obj.material, 0f, 1f, duracionCorrupcion));
                }
            }
        }

        if (canvasHabilidades != null)
            canvasHabilidades.SetActive(false);
    }

    IEnumerator AnimarShader(Material mat, float inicio, float fin, float duracion)
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Lerp(inicio, fin, tiempo / duracion);
            mat.SetFloat("_DissolveAmount", progreso);
            yield return null;
        }

        mat.SetFloat("_DissolveAmount", fin);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canvasHabilidades != null)
            {
                canvasHabilidades.SetActive(true);

                if (tipoZona == TipoZona.Curacion)
                {
                    if (botonCuracion != null) botonCuracion.SetActive(true);
                    if (botonCorrupcion != null) botonCorrupcion.SetActive(false);
                }
                else if (tipoZona == TipoZona.Corrupcion)
                {
                    if (botonCuracion != null) botonCuracion.SetActive(false);
                    if (botonCorrupcion != null) botonCorrupcion.SetActive(true);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canvasHabilidades != null)
            {
                canvasHabilidades.SetActive(false);
            }
        }
    }
}
