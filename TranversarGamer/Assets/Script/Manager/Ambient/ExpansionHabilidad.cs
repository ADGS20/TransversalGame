using UnityEngine;
using System.Collections.Generic;

public class ExpansionHabilidad : MonoBehaviour
{
    [Header("Configuración de Expansión")]
    public float radioMaximo = 10f;
    public float velocidadExpansion = 15f;
    public LayerMask capaObjetosInteractuables; // Asigna aquí la capa de tus objetos de habilidad

    [Header("Visualización (Opcional)")]
    public Transform esferaVisual; // Una esfera semitransparente para ver el efecto crecer

    private float radioActual = 0f;
    private bool expandiendo = false;
    private HabilidadShaderController.TipoZona tipoHabilidadActual; // Para saber si es V o C

    // Guardamos qué objetos ya hemos procesado en esta expansión para no repetir
    private HashSet<Collider> objetosProcesados = new HashSet<Collider>();

    void Update()
    {
        // Solo para pruebas si quieres aislarlo, pero la idea es llamarlo desde HabilidadShaderController
        // if (Input.GetKeyDown(KeyCode.P)) IniciarExpansion(HabilidadShaderController.TipoZona.Curacion);

        if (expandiendo)
        {
            radioActual += velocidadExpansion * Time.deltaTime;

            if (esferaVisual != null)
            {
                esferaVisual.localScale = Vector3.one * (radioActual * 2);
            }

            DetectarObjetos();

            if (radioActual >= radioMaximo)
            {
                DetenerExpansion();
            }
        }
    }

    public void IniciarExpansion(HabilidadShaderController.TipoZona tipo)
    {
        tipoHabilidadActual = tipo;
        radioActual = 0f;
        expandiendo = true;
        objetosProcesados.Clear(); // Limpiamos la memoria de la expansión anterior

        if (esferaVisual != null)
        {
            esferaVisual.gameObject.SetActive(true);
            esferaVisual.localScale = Vector3.zero;
        }
    }

    private void DetectarObjetos()
    {
        // Buscamos todos los colliders dentro del radio actual
        Collider[] encontrados = Physics.OverlapSphere(transform.position, radioActual, capaObjetosInteractuables);

        foreach (Collider col in encontrados)
        {
            // Si es la primera vez que la esfera toca este objeto en esta expansión
            if (!objetosProcesados.Contains(col))
            {
                objetosProcesados.Add(col);

                // Buscamos si este objeto es parte de una zona de habilidad
                HabilidadShaderController controladorZona = col.GetComponentInParent<HabilidadShaderController>();

                if (controladorZona != null)
                {
                    // Le decimos al controlador que procese este objeto específico
                    controladorZona.ProcesarObjetoTocado(col.gameObject, tipoHabilidadActual);
                }
            }
        }
    }

    private void DetenerExpansion()
    {
        expandiendo = false;
        if (esferaVisual != null)
        {
            esferaVisual.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        if (expandiendo)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, radioActual);
        }
    }
}