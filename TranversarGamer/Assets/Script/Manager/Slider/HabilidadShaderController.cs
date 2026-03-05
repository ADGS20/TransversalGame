using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HabilidadShaderController : MonoBehaviour
{
    [Header("Configuración de Naturaleza (Tecla V)")]
    public List<Renderer> objetosCuracion = new List<Renderer>();
    public Material materialCuracion;
    public float duracionCuracion = 2f;
    public List<GameObject> caminosCuracion = new List<GameObject>();

    [Header("Configuración de Corrupción (Tecla C)")]
    public List<Renderer> objetosCorrupcion = new List<Renderer>();
    public Material materialCorrupcion;
    public float duracionCorrupcion = 2f;
    public List<GameObject> caminosCorrupcion = new List<GameObject>();

    [Header("Tipo de Zona Permitida")]
    public TipoZona tipoZona;
    public enum TipoZona { Curacion, Corrupcion, Ambas }

    private bool jugadorEnZona = false;
    private bool habilidadUsada = false;

    // Memoria para guardar los materiales originales
    private Dictionary<Renderer, Material> materialesOriginales = new Dictionary<Renderer, Material>();

    void Start()
    {
        GuardarMaterialesOriginales(objetosCuracion);
        GuardarMaterialesOriginales(objetosCorrupcion);
    }

    private void GuardarMaterialesOriginales(List<Renderer> renderers)
    {
        foreach (Renderer rend in renderers)
        {
            if (rend != null && !materialesOriginales.ContainsKey(rend))
            {
                materialesOriginales.Add(rend, rend.material);
            }
        }
    }

    void Update()
    {
        if (jugadorEnZona && !habilidadUsada)
        {
            // --- NATURALEZA (V) ---
            if (Input.GetKeyDown(KeyCode.V) && (tipoZona == TipoZona.Curacion || tipoZona == TipoZona.Ambas))
            {
                ModificarInfluencia(15f);
                ProcesarNaturaleza();
                habilidadUsada = true;
            }
            // --- CORRUPCIÓN (C) ---
            else if (Input.GetKeyDown(KeyCode.C) && (tipoZona == TipoZona.Corrupcion || tipoZona == TipoZona.Ambas))
            {
                ModificarInfluencia(-15f);
                ProcesarCorrupcion();
                habilidadUsada = true;
            }
        }
    }

    private void ProcesarNaturaleza()
    {
        int cantidad = Mathf.Min(caminosCuracion.Count, objetosCuracion.Count);
        for (int i = 0; i < cantidad; i++)
        {
            GameObject camino = caminosCuracion[i];
            Renderer rend = objetosCuracion[i];

            if (camino == null || rend == null) continue;

            if (!camino.activeSelf)
            {
                // INACTIVO a ACTIVO: Muestra el efecto del shader (1 a 0) y luego restaura el original
                StartCoroutine(EfectoAparecer(camino, rend, materialCuracion, duracionCuracion));
            }
            else
            {
                // ACTIVO a INACTIVO: Se oculta directamente
                camino.SetActive(false);
            }
        }
    }

    private void ProcesarCorrupcion()
    {
        int cantidad = Mathf.Min(caminosCorrupcion.Count, objetosCorrupcion.Count);
        for (int i = 0; i < cantidad; i++)
        {
            GameObject camino = caminosCorrupcion[i];
            Renderer rend = objetosCorrupcion[i];

            if (camino == null || rend == null) continue;

            if (camino.activeSelf)
            {
                // ACTIVO a INACTIVO: Muestra el efecto de corrupción (0 a 1) y luego se oculta
                StartCoroutine(EfectoDesaparecer(camino, rend, materialCorrupcion, duracionCorrupcion));
            }
            else
            {
                // INACTIVO a ACTIVO: Aparece de golpe respetando su material original
                camino.SetActive(true);
                if (materialesOriginales.ContainsKey(rend))
                {
                    rend.material = materialesOriginales[rend];
                }
            }
        }
    }

    // --- CORRUTINA PARA APARECER (Usada por Naturaleza) ---
    IEnumerator EfectoAparecer(GameObject camino, Renderer rend, Material matHabilidad, float duracion)
    {
        camino.SetActive(true);         // 1. Encendemos el objeto
        rend.material = matHabilidad;   // 2. Le ponemos el shader del efecto

        // 3. Animamos de 1 (invisible) a 0 (visible)
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Lerp(1f, 0f, tiempo / duracion);
            rend.material.SetFloat("_DissolveAmount", progreso);
            yield return null;
        }

        // 4. AL TERMINAR: Le devolvemos su textura/material normal
        if (materialesOriginales.ContainsKey(rend))
        {
            rend.material = materialesOriginales[rend];
        }
    }

    // --- CORRUTINA PARA DESAPARECER (Usada por Corrupción) ---
    IEnumerator EfectoDesaparecer(GameObject camino, Renderer rend, Material matHabilidad, float duracion)
    {
        rend.material = matHabilidad;   // 1. Le ponemos el shader de corrupción

        // 2. Animamos de 0 (visible) a 1 (invisible)
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Lerp(0f, 1f, tiempo / duracion);
            rend.material.SetFloat("_DissolveAmount", progreso);
            yield return null;
        }

        camino.SetActive(false);        // 3. Lo apagamos

        // 4. Lo dejamos listo con su material original para cuando vuelva a encenderse
        if (materialesOriginales.ContainsKey(rend))
        {
            rend.material = materialesOriginales[rend];
        }
    }

    private void ModificarInfluencia(float cantidad)
    {
        var state = InfluenceState.EnsureInstance();
        if (state != null) state.ModifyValue(cantidad);
    }

    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) jugadorEnZona = true; }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) jugadorEnZona = false; }
}