//---------------Creador de este script-------------------------//
//--------- Hecho por: Andres Diaz Guerrero Soto --------------//
//-------------------------------------------------------------//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HabilidadShaderController : MonoBehaviour
{
    [Header("Configuración de Curación")]
    public List<SpriteRenderer> objetosCuracion = new List<SpriteRenderer>(); // Objetos afectados por la curación
    public Material materialCuracion;     // Material con shader de curación
    public float duracionCuracion = 2f;   // Duración del efecto

    [Header("Configuración de Corrupción")]
    public List<SpriteRenderer> objetosCorrupcion = new List<SpriteRenderer>(); // Objetos afectados por la corrupción
    public Material materialCorrupcion;   // Material con shader de corrupción
    public float duracionCorrupcion = 2f;

    [Header("UI")]
    public GameObject canvasHabilidades;  // Canvas que muestra los botones de habilidad
    public GameObject botonCuracion;      // Botón visible solo en zonas de curación
    public GameObject botonCorrupcion;    // Botón visible solo en zonas de corrupción

    [Header("Tipo de Zona")]
    public TipoZona tipoZona;             // Define si esta zona es de curación o corrupción

    public enum TipoZona
    {
        Curacion,
        Corrupcion
    }

    void Update()
    {
        // Permite cerrar el menú de habilidades con la tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape) && canvasHabilidades != null && canvasHabilidades.activeSelf)
        {
            canvasHabilidades.SetActive(false);
        }
    }

    // Activa el shader de curación en todos los objetos asignados
    public void UsarHabilidadCuracion()
    {
        if (objetosCuracion.Count > 0 && materialCuracion != null)
        {
            foreach (SpriteRenderer obj in objetosCuracion)
            {
                if (obj != null)
                {
                    obj.material = materialCuracion;
                    // El shader se anima desde invisible (1) a visible (0)
                    StartCoroutine(AnimarShader(obj.material, 1f, 0f, duracionCuracion));
                }
            }
        }

        if (canvasHabilidades != null)
            canvasHabilidades.SetActive(false);
    }

    // Activa el shader de corrupción en todos los objetos asignados
    public void UsarHabilidadCorrupcion()
    {
        if (objetosCorrupcion.Count > 0 && materialCorrupcion != null)
        {
            foreach (SpriteRenderer obj in objetosCorrupcion)
            {
                if (obj != null)
                {
                    obj.material = materialCorrupcion;
                    // El shader se anima desde visible (0) a invisible (1)
                    StartCoroutine(AnimarShader(obj.material, 0f, 1f, duracionCorrupcion));
                }
            }
        }

        if (canvasHabilidades != null)
            canvasHabilidades.SetActive(false);
    }

    // Corrutina que anima el valor del shader "_DissolveAmount"
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

    // Cuando el jugador entra en la zona, se muestra el menú de habilidades
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canvasHabilidades != null)
            {
                canvasHabilidades.SetActive(true);

                // Mostrar solo el botón correspondiente al tipo de zona
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

    // Al salir de la zona, se oculta el menú
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
