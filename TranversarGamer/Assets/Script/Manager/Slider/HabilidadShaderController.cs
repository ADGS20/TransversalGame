using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HabilidadShaderController : MonoBehaviour
{
    [Header("Configuración de Naturaleza (Tecla V)")]
    // Usamos Renderer para que funcione con MeshRenderer (3D) y SpriteRenderer (2D)
    public List<Renderer> objetosCuracion = new List<Renderer>();
    public Material materialCuracion;
    public float duracionCuracion = 2f;

    [Header("Configuración de Corrupción (Tecla C)")]
    public List<Renderer> objetosCorrupcion = new List<Renderer>();
    public Material materialCorrupcion;
    public float duracionCorrupcion = 2f;

    [Header("Caminos / Obstáculos (Físicos)")]
    [Tooltip("Aquí debes arrastrar los objetos que quieres que aparezcan o desaparezcan")]
    public List<GameObject> caminosAModificar = new List<GameObject>();

    [Header("Tipo de Zona Permitida")]
    public TipoZona tipoZona;

    public enum TipoZona { Curacion, Corrupcion, Ambas }

    private bool jugadorEnZona = false;
    private bool habilidadUsada = false;

    void Update()
    {
        if (jugadorEnZona && !habilidadUsada)
        {
            // --- NATURALEZA (V): Primero habilita el objeto, luego hace el efecto ---
            if (Input.GetKeyDown(KeyCode.V))
            {
                if (tipoZona == TipoZona.Curacion || tipoZona == TipoZona.Ambas)
                {
                    StartCoroutine(SecuenciaNaturaleza());
                    habilidadUsada = true;
                }
            }
            // --- CORRUPCIÓN (C): Primero hace el efecto, luego deshabilita el objeto ---
            else if (Input.GetKeyDown(KeyCode.C))
            {
                if (tipoZona == TipoZona.Corrupcion || tipoZona == TipoZona.Ambas)
                {
                    StartCoroutine(SecuenciaCorrupcion());
                    habilidadUsada = true;
                }
            }
        }
    }

    IEnumerator SecuenciaNaturaleza()
    {
        // 1. Activa el "cuadrito" (GameObject) primero
        SetEstadoCaminos(true);
        ModificarInfluencia(15f);

        // 2. Hace el efecto visual (Aparecer: de 1 a 0)
        yield return StartCoroutine(AnimarEfecto(objetosCuracion, materialCuracion, 1f, 0f, duracionCuracion));
    }

    IEnumerator SecuenciaCorrupcion()
    {
        ModificarInfluencia(-15f);

        // 1. Hace el efecto visual primero (Desaparecer: de 0 a 1)
        yield return StartCoroutine(AnimarEfecto(objetosCorrupcion, materialCorrupcion, 0f, 1f, duracionCorrupcion));

        // 2. Desactiva el "cuadrito" (GameObject) al final
        SetEstadoCaminos(false);
    }

    private void SetEstadoCaminos(bool estado)
    {
        foreach (GameObject camino in caminosAModificar)
        {
            if (camino != null)
            {
                camino.SetActive(estado);
                Debug.Log($"[Habilidad] Objeto {camino.name} ahora está {(estado ? "ACTIVO" : "INACTIVO")}");
            }
        }
    }

    IEnumerator AnimarEfecto(List<Renderer> lista, Material matHabilidad, float inicio, float fin, float duracion)
    {
        if (lista.Count == 0 || matHabilidad == null) yield break;

        foreach (Renderer rend in lista)
        {
            if (rend != null) rend.material = matHabilidad;
        }

        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = Mathf.Lerp(inicio, fin, tiempo / duracion);
            foreach (Renderer rend in lista)
            {
                if (rend != null) rend.material.SetFloat("_DissolveAmount", progreso);
            }
            yield return null;
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