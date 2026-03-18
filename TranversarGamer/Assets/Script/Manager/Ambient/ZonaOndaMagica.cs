using UnityEngine;
using System.Collections.Generic;

public class ZonaOndaMagica : MonoBehaviour
{
    [Header("Configuración de la Onda")]
    public float radioMaximo = 50f; // ¡Aumentado a 50 para que cubra todo a la vista!
    public float segundosExpansion = 2.5f;
    public float grosorLaser = 0.5f;

    [Header("Colores del Láser")]
    [ColorUsage(true, true)] public Color colorV = Color.green * 3f; // Multiplicado para que brille más
    [ColorUsage(true, true)] public Color colorC = new Color(0.6f, 0f, 1f, 1f) * 3f;

    [Header("--- NATURALEZA (Tecla V) ---")]
    public List<GameObject> aparecenConV;
    public List<GameObject> desaparecenConV;

    [Header("--- CORRUPCIÓN (Tecla C) ---")]
    public List<GameObject> aparecenConC;
    public List<GameObject> desaparecenConC;

    private bool expandiendo = false;
    private bool esOndaV = false;
    private float radioActual = 0f;
    private Vector3 centroOnda;

    // LA SOLUCIÓN DEFINITIVA: Guardamos todos los materiales en una lista para obligar a Unity a actualizarlos
    private List<Material> todosLosMateriales = new List<Material>();

    void Start()
    {
        todosLosMateriales.Clear();

        // Clasificamos quién es quién y recolectamos sus materiales
        AsignarMagiaAutomatica(aparecenConV, 0f);
        AsignarMagiaAutomatica(desaparecenConV, 1f);
        AsignarMagiaAutomatica(aparecenConC, 2f);
        AsignarMagiaAutomatica(desaparecenConC, 3f);

        // Apagamos la onda inicial
        ActualizarMateriales(0f, 1f, Vector3.zero, Color.black);
        ActualizarColisionesIniciales();
    }

    void Update()
    {
        // YA NO HACE FALTA ESTAR DENTRO DE UNA ZONA. FUNCIONARÁ EN TODO EL MAPA PARA PROBARLO.
        if (Input.GetKeyDown(KeyCode.V) && !expandiendo)
        {
            Debug.Log("¡V Pulsada! Onda de Naturaleza en camino...");
            PrepararOnda(true);
        }
        else if (Input.GetKeyDown(KeyCode.C) && !expandiendo)
        {
            Debug.Log("¡C Pulsada! Onda de Corrupción en camino...");
            PrepararOnda(false);
        }

        if (expandiendo)
        {
            radioActual += (radioMaximo / segundosExpansion) * Time.deltaTime;

            // FORZAMOS LA ACTUALIZACIÓN EN CADA MATERIAL (100% libre de bugs)
            ActualizarMateriales(radioActual, esOndaV ? 1f : 0f, centroOnda, esOndaV ? colorV : colorC);
            ActualizarColisionesEnTiempoReal();

            if (radioActual >= radioMaximo)
            {
                expandiendo = false;
                Debug.Log("La onda ha llegado a su fin.");
            }
        }
    }

    private void PrepararOnda(bool esV)
    {
        expandiendo = true;
        radioActual = 0f;

        // Buscamos al jugador. Si no lo encuentra, usa la cámara como centro.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        centroOnda = player != null ? player.transform.position : Camera.main.transform.position;

        esOndaV = esV;
        ActualizarColisionesIniciales();
    }

    private void ActualizarMateriales(float radio, float ondaEsV, Vector3 centro, Color colorLaser)
    {
        foreach (Material mat in todosLosMateriales)
        {
            if (mat != null)
            {
                mat.SetFloat("_RadioOnda", radio);
                mat.SetFloat("_OndaEsV", ondaEsV);
                mat.SetVector("_CentroOnda", centro);
                mat.SetFloat("_GrosorLaser", grosorLaser);
                mat.SetColor("_ColorLaserOnda", colorLaser);
            }
        }
    }

    private void ActualizarColisionesIniciales()
    {
        CambiarColision(aparecenConV, false);
        CambiarColision(aparecenConC, false);
        CambiarColision(desaparecenConV, true);
        CambiarColision(desaparecenConC, true);
    }

    private void ActualizarColisionesEnTiempoReal()
    {
        if (esOndaV)
        {
            RevisarColisionConOnda(aparecenConV, true);
            RevisarColisionConOnda(desaparecenConV, false);
        }
        else
        {
            RevisarColisionConOnda(aparecenConC, true);
            RevisarColisionConOnda(desaparecenConC, false);
        }
    }

    private void RevisarColisionConOnda(List<GameObject> lista, bool estadoObjetivo)
    {
        foreach (GameObject obj in lista)
        {
            if (obj != null && Vector3.Distance(obj.transform.position, centroOnda) <= radioActual)
            {
                Collider col = obj.GetComponent<Collider>();
                if (col != null) col.enabled = estadoObjetivo;
            }
        }
    }

    private void CambiarColision(List<GameObject> lista, bool estado)
    {
        foreach (GameObject obj in lista)
        {
            if (obj != null)
            {
                Collider col = obj.GetComponent<Collider>();
                if (col != null) col.enabled = estado;
            }
        }
    }

    private void AsignarMagiaAutomatica(List<GameObject> lista, float tipoObjeto)
    {
        foreach (GameObject obj in lista)
        {
            if (obj != null)
            {
                Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
                foreach (Renderer ren in renderers)
                {
                    foreach (Material mat in ren.materials)
                    {
                        mat.SetFloat("_TipoObjeto", tipoObjeto);
                        // Guardamos el material en nuestra lista blindada
                        if (!todosLosMateriales.Contains(mat)) todosLosMateriales.Add(mat);
                    }
                }
            }
        }
    }
}