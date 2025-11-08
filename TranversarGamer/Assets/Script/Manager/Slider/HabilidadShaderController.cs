//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

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
        // Cerrar el canvas con ESC
        if (Input.GetKeyDown(KeyCode.Escape) && canvasHabilidades != null && canvasHabilidades.activeSelf)
        {
            canvasHabilidades.SetActive(false);
        }
    }

    // Método para activar la habilidad de curación
    public void UsarHabilidadCuracion()
    {
        if (objetosCuracion.Count > 0 && materialCuracion != null)
        {
            foreach (SpriteRenderer obj in objetosCuracion)
            {
                if (obj != null)
                {
                    obj.material = materialCuracion;
                    // Aparece: de 1 (invisible) a 0 (visible)
                    StartCoroutine(AnimarShader(obj.material, 1f, 0f, duracionCuracion));
                }
            }
            Debug.Log("🌿 Habilidad de Curación activada - Objetos apareciendo");
        }

        if (canvasHabilidades != null)
            canvasHabilidades.SetActive(false);
    }

    // Método para activar la habilidad de corrupción
    public void UsarHabilidadCorrupcion()
    {
        if (objetosCorrupcion.Count > 0 && materialCorrupcion != null)
        {
            foreach (SpriteRenderer obj in objetosCorrupcion)
            {
                if (obj != null)
                {
                    obj.material = materialCorrupcion;
                    // Desaparece: de 0 (visible) a 1 (invisible)
                    StartCoroutine(AnimarShader(obj.material, 0f, 1f, duracionCorrupcion));
                }
            }
            Debug.Log("🔥 Habilidad de Corrupción activada - Objetos desapareciendo");
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
            mat.SetFloat("_DissolveAmount", progreso); // Cambiado a DissolveAmount
            yield return null;
        }
        mat.SetFloat("_DissolveAmount", fin);
    }

    // Para 3D
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canvasHabilidades != null)
            {
                canvasHabilidades.SetActive(true);

                // Mostrar solo el botón correspondiente a la zona
                if (tipoZona == TipoZona.Curacion)
                {
                    if (botonCuracion != null) botonCuracion.SetActive(true);
                    if (botonCorrupcion != null) botonCorrupcion.SetActive(false);
                    Debug.Log("Entraste en la Zona de Curación");
                }
                else if (tipoZona == TipoZona.Corrupcion)
                {
                    if (botonCuracion != null) botonCuracion.SetActive(false);
                    if (botonCorrupcion != null) botonCorrupcion.SetActive(true);
                    Debug.Log("Entraste en la Zona de Corrupción");
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
                Debug.Log($"Saliste de la Zona de {tipoZona}");
            }
        }
    }

    
}