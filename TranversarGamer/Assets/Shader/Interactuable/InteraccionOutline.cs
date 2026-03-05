using System.Collections.Generic;
using UnityEngine;

public class InteraccionOutline : MonoBehaviour
{
    public Transform jugador;

    [Header("Configuración")]
    public float distanciaActivacion = 2.0f;
    public KeyCode botonHabilidad = KeyCode.E;
    public Color colorIndicador = Color.red;

    [Header("Animación")]
    public float grosorMaximo = 0.03f;
    public float velocidadAparicion = 5f;

    private Renderer miRenderer;
    private Collider miCollider;
    private Material matOutline;

    // Guardaremos dos configuraciones de materiales
    private Material[] materialesConOutline;
    private Material[] materialesSinOutline;

    private bool habilidadUsada = false;
    private float grosorActual = 0f;
    private bool estaMostrandoOutline = true;

    void Start()
    {
        if (jugador == null) jugador = GameObject.FindGameObjectWithTag("Player").transform;

        miCollider = GetComponent<Collider>();
        miRenderer = GetComponent<Renderer>();

        // Obtenemos los materiales que le pusiste en el Inspector (deberían ser 2)
        materialesConOutline = miRenderer.materials;
        List<Material> listaSinOutline = new List<Material>();

        // Buscamos cuál es el material del Outline
        foreach (Material m in materialesConOutline)
        {
            if (m.HasProperty("_OutlineThickness"))
            {
                matOutline = m;
                matOutline.SetFloat("_OutlineThickness", 0f);
                matOutline.SetColor("_OutlineColor", colorIndicador);
            }
            else
            {
                // Si no es el outline, lo guardamos en la lista limpia (tu rejilla/textura)
                listaSinOutline.Add(m);
            }
        }

        // Convertimos la lista a Array para usarla luego
        materialesSinOutline = listaSinOutline.ToArray();

        if (matOutline != null)
        {
            // AL EMPEZAR: Le quitamos el material de Outline para que sea 100% invisible
            miRenderer.materials = materialesSinOutline;
            estaMostrandoOutline = false;
        }
        else
        {
            Debug.LogError("¡No se encontró ningún material con Outline en este objeto!");
        }
    }

    void Update()
    {
        if (matOutline == null || miCollider == null) return;

        float targetGrosor = 0f;

        if (!habilidadUsada)
        {
            // Calculamos la distancia hasta la superficie del collider
            Vector3 puntoEnSuperficie = miCollider.ClosestPoint(jugador.position);
            float distanciaAlBorde = Vector3.Distance(jugador.position, puntoEnSuperficie);

            // Si está dentro del rango
            if (distanciaAlBorde <= distanciaActivacion)
            {
                targetGrosor = grosorMaximo;

                if (Input.GetKeyDown(botonHabilidad))
                {
                    UsarHabilidad();
                }
            }
        }

        // Suavizado del grosor
        grosorActual = Mathf.Lerp(grosorActual, targetGrosor, Time.deltaTime * velocidadAparicion);

        // --- LA MAGIA DEL INTERCAMBIO DE MATERIALES ---

        // Si el grosor empieza a crecer y el material estaba apagado -> LO ENCENDEMOS
        if (grosorActual > 0.001f && !estaMostrandoOutline)
        {
            miRenderer.materials = materialesConOutline;
            estaMostrandoOutline = true;
        }
        // Si el grosor llega a 0 y el material seguía encendido -> LO APAGAMOS TOTALMENTE
        else if (grosorActual <= 0.001f && estaMostrandoOutline)
        {
            miRenderer.materials = materialesSinOutline;
            estaMostrandoOutline = false;
        }

        // Si el material está puesto, le actualizamos el grosor
        if (estaMostrandoOutline)
        {
            matOutline.SetFloat("_OutlineThickness", grosorActual);
        }
    }

    void UsarHabilidad()
    {
        habilidadUsada = true;
        Debug.Log("¡Habilidad ejecutada en el objeto " + gameObject.name + "!");
        // Aquí llamas al código que quieras (dar vida, abrir puerta, etc.)
    }
}