using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HabilidadShaderController : MonoBehaviour
{
    [Header("Configuración de Curación (Tecla V)")]
    public List<SpriteRenderer> objetosCuracion = new List<SpriteRenderer>();
    public Material materialCuracion;
    public float duracionCuracion = 2f;

    [Header("Configuración de Corrupción (Tecla C)")]
    public List<SpriteRenderer> objetosCorrupcion = new List<SpriteRenderer>();
    public Material materialCorrupcion;
    public float duracionCorrupcion = 2f;

    [Header("Tipo de Zona Permitida")]
    public TipoZona tipoZona;

    public enum TipoZona
    {
        Curacion,   // Solo permitirá la tecla V
        Corrupcion, // Solo permitirá la tecla C
        Ambas       // Permitirá elegir entre V o C
    }

    private bool jugadorEnZona = false;
    private bool habilidadUsada = false; // Evita que el jugador use la habilidad más de una vez

    void Update()
    {
        // Solo verificamos si el jugador está dentro de la zona y no ha usado la habilidad
        if (jugadorEnZona && !habilidadUsada)
        {
            // --- VIDA / NATURALEZA (V) ---
            if (Input.GetKeyDown(KeyCode.V))
            {
                if (tipoZona == TipoZona.Curacion || tipoZona == TipoZona.Ambas)
                {
                    UsarHabilidadCuracion();
                    ModificarInfluencia(15f); // Suma la mitad del valor original
                    habilidadUsada = true;
                }
            }
            // --- CORRUPCIÓN (C) ---
            else if (Input.GetKeyDown(KeyCode.C))
            {
                if (tipoZona == TipoZona.Corrupcion || tipoZona == TipoZona.Ambas)
                {
                    UsarHabilidadCorrupcion();
                    ModificarInfluencia(-15f); // Resta la mitad del valor original
                    habilidadUsada = true;
                }
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
    }

    // Función nueva para afectar el Slider / Estado global de influencia
    private void ModificarInfluencia(float cantidad)
    {
        var state = InfluenceState.EnsureInstance();
        if (state != null)
        {
            state.ModifyValue(cantidad);
            Debug.Log($"[HabilidadShader] Influencia modificada en: {cantidad}");
        }
        else
        {
            Debug.LogWarning("[HabilidadShader] InfluenceState no encontrado.");
        }
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
            jugadorEnZona = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = false;
        }
    }
}